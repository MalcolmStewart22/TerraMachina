import { useEffect, useRef, useState } from 'react'
import * as THREE from 'three'

const raycaster = new THREE.Raycaster()
const mouse = new THREE.Vector2()

function MapCanvas({ geometry, cellMap, onCellClick }) {
    const canvasRef = useRef(null)
    const cameraRef = useRef(null)
    const sceneRef = useRef(null)
    const rendererRef = useRef(null)
    const meshRef = useRef(null)
    

    const buildFrame = () => {
        if (sceneRef.current.children.some(c => c.userData.isFrame)) return
        const outerRect = new THREE.Shape()
        outerRect.moveTo(-3.2, -1.2)
        outerRect.lineTo(3.2, -1.2)
        outerRect.lineTo(3.2, 1.2)
        outerRect.lineTo(-3.2, 1.2)
        outerRect.lineTo(-3.2, -1.2)

        const hole = new THREE.Path()
        hole.moveTo(-3, -1)
        hole.lineTo(3, -1)
        hole.lineTo(3, 1)
        hole.lineTo(-3, 1)
        hole.lineTo(-3, -1)

        outerRect.holes.push(hole)

        const frameGeometry = new THREE.ShapeGeometry(outerRect)
        const frameMaterial = new THREE.MeshBasicMaterial({ color: 0xd4c4a0  })
        const frameMesh = new THREE.Mesh(frameGeometry, frameMaterial)
        frameMesh.position.z = 0.01
        frameMesh.userData.isFrame = true
        sceneRef.current.add(frameMesh)
        

        const borderPoints = [
            new THREE.Vector3(-3.1, -1.1, 0),
            new THREE.Vector3(3.1, -1.1, 0),
            new THREE.Vector3(3.1, 1.1, 0),
            new THREE.Vector3(-3.1, 1.1, 0),
        ]
        const borderGeometry = new THREE.BufferGeometry().setFromPoints(borderPoints)
        const borderMaterial = new THREE.LineBasicMaterial({ color: 0x4a3a28 })
        const border = new THREE.LineLoop(borderGeometry, borderMaterial)
        border.position.z = 0.02
        sceneRef.current.add(border)

        const innerBorderPoints = [
            new THREE.Vector3(-3, -1, 0),
            new THREE.Vector3(3, -1, 0),
            new THREE.Vector3(3, 1, 0),
            new THREE.Vector3(-3, 1, 0),
        ]
        const innerBorderGeometry = new THREE.BufferGeometry().setFromPoints(innerBorderPoints)
        const innerBorder = new THREE.LineLoop(innerBorderGeometry, borderMaterial)
        innerBorder.position.z = 0.02
        sceneRef.current.add(innerBorder)

        const activeBorderPoints = [
            new THREE.Vector3(-2.95, -.95, 0),
            new THREE.Vector3(2.95, -.95, 0),
            new THREE.Vector3(2.95, .95, 0),
            new THREE.Vector3(-2.95, .95, 0),
        ]
        const activeBorderGeometry = new THREE.BufferGeometry().setFromPoints(activeBorderPoints)
        const activeBorder = new THREE.LineLoop(activeBorderGeometry, borderMaterial)
        activeBorder.position.z = 0.02
        sceneRef.current.add(activeBorder)
    }
    const handleResize = () => {
        if (!canvasRef.current) return
        rendererRef.current.setSize(canvasRef.current.clientWidth, canvasRef.current.clientHeight, false)
        
        const aspect = canvasRef.current.clientWidth / canvasRef.current.clientHeight
        const halfHeight = 1.8
        const halfWidth = halfHeight * aspect
        cameraRef.current.left = -halfWidth
        cameraRef.current.right = halfWidth
        cameraRef.current.top = halfHeight - .5
        cameraRef.current.bottom = -halfHeight
        cameraRef.current.updateProjectionMatrix()
    }
    const handleClick = (event) => {
        const rect = canvasRef.current.getBoundingClientRect()
        
        // Convert pixel coords to NDC
        mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
        mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1
        
        raycaster.setFromCamera(mouse, cameraRef.current)
        const intersects = raycaster.intersectObject(meshRef.current)
        
        if (intersects.length === 0) return
        
        const faceIndex = intersects[0].faceIndex
        const cellId = geometry.faceIndexToCellId[faceIndex]
        
        onCellClick(cellId)
    }

    useEffect(() => {
        cameraRef.current = new THREE.OrthographicCamera(-4, 4, 2.5, -2.5, .01, 100)
        sceneRef.current = new THREE.Scene()
        rendererRef.current = new THREE.WebGLRenderer({canvas:canvasRef.current})
        rendererRef.current.setClearColor(0x2a1f15)
        

        cameraRef.current.position.z = 2
        handleResize()
        window.addEventListener('resize', handleResize)
        canvasRef.current.addEventListener('click', handleClick)

        let animationId
        
        const animate = () => {
            animationId = requestAnimationFrame(animate)
            rendererRef.current.render(sceneRef.current, cameraRef.current )
        }
        animate()
        return () => {
            cancelAnimationFrame(animationId)
            rendererRef.current.dispose()
        }
    }, [])

    useEffect(() => {
        let positionAttribute = new THREE.BufferAttribute(geometry.vertices, 3)
        positionAttribute.setUsage(THREE.DynamicDrawUsage)

        let colorBuffer = new Float32Array(geometry.vertices.length)
        for (let i = 0; i < colorBuffer.length; i += 3) {
            colorBuffer[i] = 43/255      // R
            colorBuffer[i + 1] = 95/255  // G
            colorBuffer[i + 2] = 122/255  // B
        }
        let colorAttribute = new THREE.BufferAttribute(colorBuffer, 3)

        const meshGeometry = new THREE.BufferGeometry()
        meshGeometry.setAttribute('position', positionAttribute)
        meshGeometry.setAttribute('color', colorAttribute)
        const meshMaterial = new THREE.MeshBasicMaterial({side: THREE.DoubleSide, vertexColors: true})
        meshRef.current = new THREE.Mesh(meshGeometry,meshMaterial)
        sceneRef.current.add(meshRef.current)

        const lineMaterial = new THREE.MeshBasicMaterial({ color: 0x000000, wireframe: true, side: THREE.DoubleSide})
        let lines = new THREE.Mesh(meshGeometry, lineMaterial)
        sceneRef.current.add(lines)

        return () => {
            sceneRef.current.remove(meshRef.current)
            sceneRef.current.remove(lines)
            meshGeometry.dispose()
            meshMaterial.dispose()
            lineMaterial.dispose()
        }
    }, [geometry])

    useEffect(() => {
        if (!sceneRef.current) return
        buildFrame()
    }, [])

    return(
        <canvas ref={canvasRef} className="absolute inset-0 w-full h-full" />
    )
}

export default MapCanvas