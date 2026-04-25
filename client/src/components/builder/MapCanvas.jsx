import { useEffect, useRef } from 'react'
import * as THREE from 'three'
import { computeCellColor, applyElevationChange, getCellsInRadius, isCellEditable } from '../../builder/painting'

const RAYCASTER = new THREE.Raycaster()
const MOUSE = new THREE.Vector2()
const CELL_SIZE_APPROX = 0.018

function MapCanvas({ geometry, cellMap, activeTool, brushPower, brushSize, seaLevel }) {
    const activeToolRef = useRef(activeTool)
    const brushPowerRef = useRef(brushPower)
    const brushSizeRef = useRef(brushSize)
    const seaLevelRef = useRef(seaLevel)

    const canvasRef = useRef(null)
    const cameraRef = useRef(null)
    const sceneRef = useRef(null)
    const rendererRef = useRef(null)
    const meshRef = useRef(null)

    const isMouseDownRef = useRef(false)
    const hoveredCellRef = useRef(null)
    const tickIntervalRef = useRef(null)
    const cursorRingRef = useRef(null)
    const alignTargetRef = useRef(null)

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
    const buildRingCursor = () =>{
        const ringGeometry = new THREE.RingGeometry(0.95, 1.0, 32)  // inner, outer, segments
        const ringMaterial = new THREE.MeshBasicMaterial({ 
            color: 0xffffff, 
            side: THREE.DoubleSide,
            transparent: true,
            opacity: 0.7
        })
        const cursorRing = new THREE.Mesh(ringGeometry, ringMaterial)
        cursorRing.position.z = 0.05
        cursorRing.visible = false
        sceneRef.current.add(cursorRing)
        cursorRingRef.current = cursorRing
        const radius = brushSize * CELL_SIZE_APPROX
        cursorRingRef.current.scale.set(radius, radius, 1)
    }

   //#region Painting
    const updateCellColor = (cellId, elevation) => {
        const offset = geometry.cellIdsToOffset.get(cellId)
        if (offset == null) return
        
        const color = computeCellColor(elevation, seaLevelRef.current)
        const colorAttr = meshRef.current.geometry.attributes.color
        
        // each cell is 3 vertices, write same color to all 3
        for (let i = 0; i < 3; i++) {
            const vertexOffset = offset + i * 3
            colorAttr.array[vertexOffset]     = color[0]
            colorAttr.array[vertexOffset + 1] = color[1]
            colorAttr.array[vertexOffset + 2] = color[2]
        }
        
        colorAttr.needsUpdate = true
    }
    const updateHoveredCell = (event) => {
        const rect = canvasRef.current.getBoundingClientRect()
        MOUSE.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
        MOUSE.y = -((event.clientY - rect.top) / rect.height) * 2 + 1
        
        RAYCASTER.setFromCamera(MOUSE, cameraRef.current)
        const intersects = RAYCASTER.intersectObject(meshRef.current)
        
        const overMap = intersects.length > 0
        const toolActive = activeToolRef.current !== null
        
        // Cursor visibility — always runs
        const desiredCursor = (overMap && toolActive) ? 'none' : 'default'
        if (canvasRef.current.style.cursor !== desiredCursor) {
            canvasRef.current.style.cursor = desiredCursor
        }
        
        // Ring visibility based on overMap (independent of activeTool, since activeTool already controls visibility globally)
        if (cursorRingRef.current) {
            cursorRingRef.current.visible = overMap && toolActive
        }
        
        if (!overMap) {
            hoveredCellRef.current = null
            return
        }
        
        // We're over the map — update hovered cell and ring position
        hoveredCellRef.current = geometry.faceIndexToCellId[intersects[0].faceIndex]
        cursorRingRef.current.position.x = intersects[0].point.x
        cursorRingRef.current.position.y = intersects[0].point.y
    }
    const startPaintTick = () => {
        if (tickIntervalRef.current) return
        tickIntervalRef.current = setInterval(() => {
        const cellId = hoveredCellRef.current
        if (cellId == null) return
        
        const centerCell = cellMap.cellById.get(cellId)
        if (!isCellEditable(centerCell)) return  // center must be editable
        
        const cellsInRadius = getCellsInRadius(cellMap, cellId, brushSizeRef.current)
        for (const [id, distance] of cellsInRadius.visited) {
            const cell = cellMap.cellById.get(id)
            
            if (!isCellEditable(cell)) continue
            
            const falloff = (Math.cos((distance / brushSizeRef.current) * Math.PI) + 1) / 2
            applyElevationChange(cell, activeToolRef.current, brushPowerRef.current * falloff, {alignTarget: alignTargetRef.current, smoothTarget: cellsInRadius.avgElevation})
            updateCellColor(id, cell.elevation)
        }
        }, 50)
    }
    const stopPaintTick = () => {
        if (tickIntervalRef.current) {
            clearInterval(tickIntervalRef.current)
            tickIntervalRef.current = null
        }
    }
    const handleMouseDown = (event) => {
        isMouseDownRef.current = true
        updateHoveredCell(event)

        if (activeToolRef.current === 'align') {
            const cellId = hoveredCellRef.current
            if (cellId != null) {
                const cell = cellMap.cellById.get(cellId)
                alignTargetRef.current = cell.elevation
            }
        }

        startPaintTick()
    }
    const handleMouseMove = (event) => {
            updateHoveredCell(event)
    }
    const handleMouseUp = () => {
        isMouseDownRef.current = false
        alignTargetRef.current = null
        stopPaintTick()
    }
    //#endregion


    useEffect(() => {
        cameraRef.current = new THREE.OrthographicCamera(-4, 4, 2.5, -2.5, .01, 100)
        sceneRef.current = new THREE.Scene()
        rendererRef.current = new THREE.WebGLRenderer({canvas:canvasRef.current})
        rendererRef.current.setClearColor(0x2a1f15)
        

        cameraRef.current.position.z = 2
        handleResize()
        window.addEventListener('resize', handleResize)
        canvasRef.current.addEventListener('mousedown', handleMouseDown)
        window.addEventListener('mousemove', handleMouseMove)
        window.addEventListener('mouseup', handleMouseUp)

        let animationId
        
        const animate = () => {
            animationId = requestAnimationFrame(animate)
            rendererRef.current.render(sceneRef.current, cameraRef.current )
        }
        animate()
        return () => {
            cancelAnimationFrame(animationId)
            rendererRef.current.dispose()
            canvasRef.current.removeEventListener('mousedown', handleMouseDown)
            window.removeEventListener('mousemove', handleMouseMove)
            window.removeEventListener('mouseup', handleMouseUp)
            stopPaintTick()
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
        buildRingCursor()
    }, [])

    useEffect(() => { activeToolRef.current = activeTool }, [activeTool])
    useEffect(() => { brushPowerRef.current = brushPower }, [brushPower])
    useEffect(() => { 
        const radius = brushSize * CELL_SIZE_APPROX
        cursorRingRef.current.scale.set(radius, radius, 1)
        brushSizeRef.current = brushSize 
    }, [brushSize])
    useEffect(() => { seaLevelRef.current = seaLevel }, [seaLevel])
    
    return(
        <canvas ref={canvasRef} className="absolute inset-0 w-full h-full" />
    )
}

export default MapCanvas