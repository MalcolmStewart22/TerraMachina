import { useEffect, useRef } from 'react'
import * as THREE from 'three'

function MapCanvas({ geometry, cellMap }) {
    const canvasRef = useRef(null)
    const cameraRef = useRef(null)
    const sceneRef = useRef(null)
    const rendererRef = useRef(null)
  
    useEffect(() => {
        cameraRef.current = new THREE.OrthographicCamera(-4, 4, 2.5, -2.5, .01, 100)
        sceneRef.current = new THREE.Scene()
        rendererRef.current = new THREE.WebGLRenderer({canvas:canvasRef.current})
        const canvas = canvasRef.current
        rendererRef.current.setSize(canvas.clientWidth, canvas.clientHeight, false)
        
        cameraRef.current.position.z = 2
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
        const mesh = new THREE.Mesh(meshGeometry,meshMaterial)
        sceneRef.current.add(mesh)

        const lineMaterial = new THREE.MeshBasicMaterial({ color: 0x000000, wireframe: true, side: THREE.DoubleSide,})
        let lines = new THREE.Mesh(meshGeometry, lineMaterial)
        sceneRef.current.add(lines)
        
    }, [geometry])

    
  
  return <canvas ref={canvasRef} className="w-full h-full" />
}

export default MapCanvas