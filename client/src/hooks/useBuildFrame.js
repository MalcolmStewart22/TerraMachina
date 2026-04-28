import { useEffect } from 'react'
import * as THREE from 'three'

export function useBuildFrame(frameSceneRef){

    const buildFrame = () => {
            if (frameSceneRef.current.children.some(c => c.userData.isFrame)) return
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
            frameSceneRef.current.add(frameMesh)
            
    
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
            frameSceneRef.current.add(border)
    
            const innerBorderPoints = [
                new THREE.Vector3(-3, -1, 0),
                new THREE.Vector3(3, -1, 0),
                new THREE.Vector3(3, 1, 0),
                new THREE.Vector3(-3, 1, 0),
            ]
            const innerBorderGeometry = new THREE.BufferGeometry().setFromPoints(innerBorderPoints)
            const innerBorder = new THREE.LineLoop(innerBorderGeometry, borderMaterial)
            innerBorder.position.z = 0.02
            frameSceneRef.current.add(innerBorder)
    
            const activeBorderPoints = [
                new THREE.Vector3(-2.95, -.95, 0),
                new THREE.Vector3(2.95, -.95, 0),
                new THREE.Vector3(2.95, .95, 0),
                new THREE.Vector3(-2.95, .95, 0),
            ]
            const activeBorderGeometry = new THREE.BufferGeometry().setFromPoints(activeBorderPoints)
            const activeBorder = new THREE.LineLoop(activeBorderGeometry, borderMaterial)
            activeBorder.position.z = 0.02
            frameSceneRef.current.add(activeBorder)
    
            const bgRect = new THREE.Shape()
            bgRect.moveTo(-50, -50)
            bgRect.lineTo(50, -50)
            bgRect.lineTo(50, 50)
            bgRect.lineTo(-50, 50)
            bgRect.lineTo(-50, -50)
    
            const bgHole = new THREE.Path()
            bgHole.moveTo(-3, -1)
            bgHole.lineTo(3, -1)
            bgHole.lineTo(3, 1)
            bgHole.lineTo(-3, 1)
            bgHole.lineTo(-3, -1)
            bgRect.holes.push(bgHole)
    
            const bgGeometry = new THREE.ShapeGeometry(bgRect)
            const bgMaterial = new THREE.MeshBasicMaterial({ color: 0x2a1f15 })
            const bgMesh = new THREE.Mesh(bgGeometry, bgMaterial)
            bgMesh.position.z = 0 
            frameSceneRef.current.add(bgMesh)
        }

    useEffect(() => {
        if (!frameSceneRef.current) return
        buildFrame()
    }, [])
}