import { useRef, useEffect } from "react"

const MAX_ZOOM = 10
const MIN_ZOOM = 1
export function useMapCameraControls({ canvasRef, mapCameraRef, onLeftMouseDown, onLeftMouseUp, onMouseMove, onZoom }){
    const isMouseDownRef = useRef(false)
    const isPanningRef = useRef(false)
    const lastPanPosRef = useRef({ x: 0, y: 0 })

    
    const handleContextMenu = (event) => {
        event.preventDefault()
    }
    const clampCameraToMap = () => {
        const cam = mapCameraRef.current
            
        const HOLE_LEFT = -3, HOLE_RIGHT = 3
        const HOLE_BOTTOM = -1, HOLE_TOP = 1
        const MAP_LEFT = -3, MAP_RIGHT = 3
        const MAP_BOTTOM = -1, MAP_TOP = 1
            
        const cx = (cam.left + cam.right) / 2
        const cy = (cam.top + cam.bottom) / 2
        const dx = (cam.right - cam.left) / (2 * cam.zoom)
        const dy = (cam.top - cam.bottom) / (2 * cam.zoom)
         
        const leftFrac = (HOLE_LEFT - cam.left) / (cam.right - cam.left)
        const rightFrac = (HOLE_RIGHT - cam.left) / (cam.right - cam.left)
        const bottomFrac = (HOLE_BOTTOM - cam.bottom) / (cam.top - cam.bottom)
        const topFrac = (HOLE_TOP - cam.bottom) / (cam.top - cam.bottom)
            
        const leftOffset = cx - dx + 2 * dx * leftFrac
        const rightOffset = cx - dx + 2 * dx * rightFrac
        const bottomOffset = cy - dy + 2 * dy * bottomFrac
        const topOffset = cy - dy + 2 * dy * topFrac
            
        const minX = MAP_LEFT - leftOffset
        const maxX = MAP_RIGHT - rightOffset
        const minY = MAP_BOTTOM - bottomOffset
        const maxY = MAP_TOP - topOffset
            
        cam.position.x = Math.max(minX, Math.min(maxX, cam.position.x))
        cam.position.y = Math.max(minY, Math.min(maxY, cam.position.y))
    }

    const handleMouseDown = (event) => {
        if (event.button === 2) {
            isPanningRef.current = true
            lastPanPosRef.current = { x: event.clientX, y: event.clientY }
        } else if (event.button === 0) {
            isMouseDownRef.current = true
            onLeftMouseDown?.(event)
        }
    }
    const handleMouseMove = (event) => {
        if (isPanningRef.current) {
            const dx = event.clientX - lastPanPosRef.current.x
            const dy = event.clientY - lastPanPosRef.current.y
            lastPanPosRef.current = { x: event.clientX, y: event.clientY }
            
            const cam = mapCameraRef.current
            const worldPerPixelX = (cam.right - cam.left) / canvasRef.current.clientWidth / cam.zoom
            const worldPerPixelY = (cam.top - cam.bottom) / canvasRef.current.clientHeight / cam.zoom
            
            cam.position.x -= dx * worldPerPixelX
            cam.position.y += dy * worldPerPixelY
            clampCameraToMap() 
        }
        onMouseMove?.(event)
    }
    const handleMouseUp = (event) => {
        if (event.button === 2) {
            isPanningRef.current = false
             event.preventDefault() 
        } else if (event.button === 0) {
            isMouseDownRef.current = false
            onLeftMouseUp?.(event)
        }
    }
    const handleWheel = (event) => {
        event.preventDefault()  
        
        const zoomSpeed = 0.001
        const zoomDelta = -event.deltaY * zoomSpeed
        const currentZoom = mapCameraRef.current.zoom
        if(currentZoom === MAX_ZOOM) return
        const newZoom = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, currentZoom + zoomDelta))
        
        mapCameraRef.current.zoom = newZoom
        mapCameraRef.current.updateProjectionMatrix()

        clampCameraToMap()
        onZoom()
    }

    useEffect(() => {
            const canvas = canvasRef.current
    
        canvas.addEventListener('mousedown', handleMouseDown)
            window.addEventListener('mousemove', handleMouseMove)
            window.addEventListener('mouseup', handleMouseUp)
            canvasRef.current.addEventListener('wheel', handleWheel)
            document.addEventListener('contextmenu', handleContextMenu);
        
            return () => {
                canvas.removeEventListener('mousedown', handleMouseDown)
                window.removeEventListener('mousemove', handleMouseMove)
                window.removeEventListener('mouseup', handleMouseUp)
                canvasRef.current.removeEventListener('wheel', handleWheel)
                document.removeEventListener('contextmenu', handleContextMenu);
            }
        }, [])

}