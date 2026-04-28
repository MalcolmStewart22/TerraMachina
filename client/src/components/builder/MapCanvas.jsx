import { useEffect, useRef } from 'react'
import * as THREE from 'three'
import { computeCellElevationColor, applyElevationChange, getCellsInRadius, isCellEditable, computeBiomeColor } from '../../builder/painting'
import { useThreeJSRenderer } from '../../hooks/useThreeJSRenderer'
import { useBuildFrame } from '../../hooks/useBuildFrame'
import { useMapCameraControls } from '../../hooks/useMapCameraControls'

const RAYCASTER = new THREE.Raycaster()
const MOUSE = new THREE.Vector2()
const CELL_SIZE_APPROX = 0.018

function MapCanvas({ geometry, cellMap, seaLevel, activeTool, brushPower, brushSize, activeLayer }) {
    const activeLayerRef = useRef(activeLayer)
    const activeToolRef = useRef(activeTool)
    const brushPowerRef = useRef(brushPower)
    const brushSizeRef = useRef(brushSize)
    const seaLevelRef = useRef(seaLevel)

    const canvasRef = useRef(null)
    const { mapSceneRef, mapCameraRef, frameSceneRef } 
        = useThreeJSRenderer(canvasRef)
    
    const meshRef = useRef(null)
    const cursorRingRef = useRef(null)

    const hoveredCellRef = useRef(null)
    const alignTargetRef = useRef(null)
    const tickIntervalRef = useRef(null)

   //#region Painting
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
        mapSceneRef.current.add(cursorRing)
        cursorRingRef.current = cursorRing
        const radius = brushSize * CELL_SIZE_APPROX
        cursorRingRef.current.scale.set(radius, radius, 1)
    }
    const scaleRingCursor = () =>{
        const baseRadius = brushSize * CELL_SIZE_APPROX
        cursorRingRef.current.scale.set(baseRadius, baseRadius-.01, 1)
    }
    const updateCellColor = (cell) => {
         console.log('paint with layer:', activeLayerRef.current)
        const offset = geometry.cellIdsToOffset.get(cell.geometry.cellId)
        if (offset == null) return
        
        let color
        switch (activeLayerRef.current) {
            case 'elevation':
                color = computeCellElevationColor(cell.elevation, seaLevelRef.current)
                break
            case 'biome':
                color = computeBiomeColor(cell, seaLevelRef.current)
                break
            default:
                return
        }
        
        const colorAttr = meshRef.current.geometry.attributes.color
        
        for (let i = 0; i < 3; i++) {
            const vertexOffset = offset + i * 3
            colorAttr.array[vertexOffset]     = color[0]
            colorAttr.array[vertexOffset + 1] = color[1]
            colorAttr.array[vertexOffset + 2] = color[2]
        }
        
        colorAttr.needsUpdate = true
    }
    const recolorAllCells = () => {
        if (!meshRef.current || !cellMap) return
        
        const colorAttr = meshRef.current.geometry.attributes.color
        
        for (const cell of cellMap.cells) {
            const offset = geometry.cellIdsToOffset.get(cell.geometry.cellId)
            if (offset == null) continue
            
            let color
            switch (activeLayerRef.current) {
                case 'elevation':
                    color = computeCellElevationColor(cell.elevation, seaLevelRef.current)
                    break
                case 'biome':
                    color = computeBiomeColor(cell, seaLevelRef.current)
                    break
            }
            
            for (let i = 0; i < 3; i++) {
                const vertexOffset = offset + i * 3
                colorAttr.array[vertexOffset] = color[0]
                colorAttr.array[vertexOffset + 1] = color[1]
                colorAttr.array[vertexOffset + 2] = color[2]
            }
        }
        
        colorAttr.needsUpdate = true
    }
    const updateHoveredCell = (event) => {
        const rect = canvasRef.current.getBoundingClientRect()
        MOUSE.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
        MOUSE.y = -((event.clientY - rect.top) / rect.height) * 2 + 1
        
        RAYCASTER.setFromCamera(MOUSE, mapCameraRef.current)
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

        const cellsInRadius = getCellsInRadius(cellMap, cellId, brushSizeRef.current)
        for (const [id, distance] of cellsInRadius.visited) {
            const cell = cellMap.cellById.get(id)
            
            if (!isCellEditable(cell)) continue

            switch(activeLayerRef.current){
                case 'elevation':
                    const falloff = (Math.cos((distance / brushSizeRef.current) * Math.PI) + 1) / 2
                    applyElevationChange(cell, activeToolRef.current, brushPowerRef.current * falloff, {alignTarget: alignTargetRef.current, smoothTarget: cellsInRadius.avgElevation, seaLevel: seaLevelRef.current})
                    break
                case 'biome':
                    if (cell.elevation < seaLevelRef.current) continue
                    if (cell.biome === activeToolRef.current) continue
                    cell.biome = activeToolRef.current
                    break
            }
            updateCellColor(cell)
        }
        }, 50)
    }
    const stopPaintTick = () => {
        console.log('stopPaintTick called, interval is:', tickIntervalRef.current)
        if (tickIntervalRef.current) {
            clearInterval(tickIntervalRef.current)
            tickIntervalRef.current = null
            alignTargetRef.current = null
        }
    }
    const handlePaintStart = () => {
        if (!activeToolRef.current) return
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
    //#endregion

    useBuildFrame(frameSceneRef)
    useMapCameraControls({
        canvasRef,
        mapCameraRef,
        onLeftMouseDown: handlePaintStart,
        onLeftMouseUp: stopPaintTick,
        onMouseMove: updateHoveredCell,
        onZoom: scaleRingCursor 
    })

    useEffect(() => {
        buildRingCursor()
        return () => {
            stopPaintTick()
        }
    }, [])

    useEffect(() => {
        let positionAttribute = new THREE.BufferAttribute(geometry.vertices, 3)
        positionAttribute.setUsage(THREE.DynamicDrawUsage)

        const colorBuffer = new Float32Array(geometry.vertices.length)
        for (let i = 0; i < colorBuffer.length; i += 3) {
            colorBuffer[i]     = 43/255   
            colorBuffer[i + 1] = 95/255   
            colorBuffer[i + 2] = 122/255  
        }
        let colorAttribute = new THREE.BufferAttribute(colorBuffer, 3)

        const meshGeometry = new THREE.BufferGeometry()
        meshGeometry.setAttribute('position', positionAttribute)
        meshGeometry.setAttribute('color', colorAttribute)
        const meshMaterial = new THREE.MeshBasicMaterial({side: THREE.DoubleSide, vertexColors: true})
        meshRef.current = new THREE.Mesh(meshGeometry,meshMaterial)
        mapSceneRef.current.add(meshRef.current)

        const lineMaterial = new THREE.MeshBasicMaterial({ color: 0x000000, wireframe: true, side: THREE.DoubleSide})
        let lines = new THREE.Mesh(meshGeometry, lineMaterial)
        mapSceneRef.current.add(lines)

        return () => {
            mapSceneRef.current.remove(meshRef.current)
            mapSceneRef.current.remove(lines)
            meshGeometry.dispose()
            meshMaterial.dispose()
            lineMaterial.dispose()
        }
    }, [geometry])

    useEffect(() => {
        activeLayerRef.current = activeLayer 
        if (!meshRef.current || !cellMap) return        
        recolorAllCells()
    }, [activeLayer])
    
    useEffect(() => { activeToolRef.current = activeTool }, [activeTool])
    useEffect(() => { brushPowerRef.current = brushPower }, [brushPower])
    useEffect(() => { 
        scaleRingCursor()
        brushSizeRef.current = brushSize 
    }, [brushSize])
    useEffect(() => { 
        seaLevelRef.current = seaLevel
        recolorAllCells() 
    }, [seaLevel])
    
    return(
        <canvas ref={canvasRef} className="absolute inset-0 w-full h-full" />
    )
}

export default MapCanvas