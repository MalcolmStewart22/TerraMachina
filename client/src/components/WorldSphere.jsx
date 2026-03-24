import { useEffect, useRef } from "react";
import * as THREE from 'three';
import useEngineHub from "../hooks/useEngineHub";
import { OrbitControls } from "three/examples/jsm/Addons.js";

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


        const controls = new OrbitControls(cameraRef.current, canvasRef.current)
        controls.minDistance = 1.25
        controls.maxDistance = 5
        controls.rotateSpeed = .5


        let animationId
        const animate = () => {
            animationId = requestAnimationFrame(animate)
            rendererRef.current.render(sceneRef.current,cameraRef.current)
            controls.update()
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
            const meshMaterial = new THREE.MeshBasicMaterial({color: 0x4444aa, side: THREE.DoubleSide})
            const mesh = new THREE.Mesh(geometry,meshMaterial)
            sceneRef.current.add(mesh)

            const wireframe = new THREE.WireframeGeometry(geometry)
            const lineMaterial = new THREE.LineBasicMaterial({ color: 0x000000})
            const lines = new THREE.LineSegments(wireframe, lineMaterial)
            sceneRef.current.add(lines)
        }
    }, [latestUpdate])

    return (
        <div>
            <canvas ref={canvasRef} />
        </div>
    )

}

export default WorldSphere;