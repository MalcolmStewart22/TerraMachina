import { useEffect, useRef } from 'react'
import * as THREE from 'three'

export function useThreeJSRenderer(canvasRef){
    const frameCameraRef = useRef(null)
    const frameSceneRef = useRef(null)
    const mapCameraRef = useRef(null)
    const mapSceneRef = useRef(null)
    const rendererRef = useRef(null)
    
        useEffect(() => {
            mapSceneRef.current = new THREE.Scene()
            mapCameraRef.current = new THREE.OrthographicCamera(-4, 4, 2.5, -2.5, 0.01, 100)
            mapCameraRef.current.position.z = 2
            
            frameSceneRef.current = new THREE.Scene()
            frameCameraRef.current = new THREE.OrthographicCamera(-4, 4, 2.5, -2.5, 0.01, 100)
            frameCameraRef.current.position.z = 2
            
            rendererRef.current = new THREE.WebGLRenderer({ canvas: canvasRef.current })
            rendererRef.current.setClearColor(0x2a1f15)
            
            const handleResize = () => {
                if (!canvasRef.current) return
                rendererRef.current.setSize(canvasRef.current.clientWidth, canvasRef.current.clientHeight, false)
                
                const aspect = canvasRef.current.clientWidth / canvasRef.current.clientHeight
                const halfHeight = 1.8
                const halfWidth = halfHeight * aspect
                
                for (const cam of [mapCameraRef.current, frameCameraRef.current]) {
                    cam.left = -halfWidth
                    cam.right = halfWidth
                    cam.top = halfHeight - 0.5
                    cam.bottom = -halfHeight
                    cam.updateProjectionMatrix()
                }
            }
            
            handleResize()
            window.addEventListener('resize', handleResize)



            let animationId
            const animate = () => {
                animationId = requestAnimationFrame(animate)
                rendererRef.current.autoClear = true
                rendererRef.current.render(mapSceneRef.current, mapCameraRef.current)
                rendererRef.current.autoClear = false
                rendererRef.current.render(frameSceneRef.current, frameCameraRef.current)
                rendererRef.current.autoClear = true
            }
            animate()
            
            return () => {
                cancelAnimationFrame(animationId)
                rendererRef.current.dispose()
            }
        }, [])


    return { mapSceneRef, mapCameraRef, frameSceneRef}
}