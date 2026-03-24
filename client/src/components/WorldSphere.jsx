import { useEffect, useRef } from "react";
import * as THREE from 'three';
import useEngineHub from "../hooks/useEngineHub";

function WorldSphere() {
    
    const canvasRef = useRef(null)
    const cameraRef = useRef(null)
    const sceneRef = useRef(null)
    const rendererRef = useRef(null)
    const latestUpdate = useEngineHub()
    
    useEffect(() => {
        cameraRef.current = new THREE.PerspectiveCamera(75, window.innerWidth/ window.innerHeight, 0.1,1000)
        sceneRef.current = new THREE.Scene()
        rendererRef.current = new THREE.WebGLRenderer({canvas:canvasRef.current})
        rendererRef.current.setSize(window.innerWidth, window.innerHeight)
        rendererRef.current.setPixelRatio(window.devicePixelRatio)
        cameraRef.current.position.z = 3
        let animationId
        
        const animate = () => {
            animationId = requestAnimationFrame(animate)
            rendererRef.current.render(sceneRef.current,cameraRef.current)
        }
        animate()
        return () => {
            cancelAnimationFrame(animationId)
            rendererRef.current.dispose()
        }
    }, [])

    useEffect(() => {
        console.log(latestUpdate)
        if (latestUpdate?.payload){
            const positions = []
            for (const cell of latestUpdate.payload.sphere){
                for (const vertex of cell.vertices){
                    positions.push(vertex.x, vertex.y, vertex.z)
                }
            }
            const positionArray = new Float32Array(positions)

            const geometry = new THREE.BufferGeometry()
            geometry.setAttribute('position', new THREE.BufferAttribute(positionArray,3))
            const meshMaterial = new THREE.MeshBasicMaterial({wireframe:true})
            const mesh = new THREE.Mesh(geometry,meshMaterial)
            sceneRef.current.add(mesh)
        }
    }, [latestUpdate])

    return (
        <div>
            <canvas ref={canvasRef} />
        </div>
    )

}

export default WorldSphere;