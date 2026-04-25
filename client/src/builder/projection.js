import { Vector3 } from "three";

const VISIBLE_LATITUDE = 60 * Math.PI / 180 

export function projectToEquirectangular(position, scale){
    const lat = Math.asin(position.y)
    const long = Math.atan2(position.z, position.x)

    const screenX = long * scale
    const screenY = lat * scale

    return {screenX, screenY, lat}
}

export function buildGeometry(cellMap) {
    const vertexArray = []
    const cellIdsToOffset = new Map()
    let cellOffset = 0;
    const faceIndexToCellId = []
    
    for(const cell of cellMap.cells){
        
        if(Math.abs(Math.asin(cell.geometry.position.y)) >= VISIBLE_LATITUDE) continue
        let screenPos = []

        for(const vertex of cell.geometry.vertices){
            screenPos.push(projectToEquirectangular(vertex, 1))
        }

        const xs = [screenPos[0].screenX, screenPos[1].screenX, screenPos[2].screenX]
        const spanX = Math.max(...xs) - Math.min(...xs)
        if (spanX > Math.PI) continue 

        cellIdsToOffset.set(cell.geometry.cellId, cellOffset)
        faceIndexToCellId.push(cell.geometry.cellId)
        cellOffset += 9
        for(let i = 0; i < 3; i++){
            vertexArray.push(screenPos[i].screenX)
            vertexArray.push(screenPos[i].screenY)
            vertexArray.push(0)
        }
    }
    return {vertices: new Float32Array(vertexArray), cellIdsToOffset, faceIndexToCellId}
}

