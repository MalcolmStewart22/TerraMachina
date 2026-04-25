const ELEVATION_STOPS = [
    { elevation: -5000, color: [0x2b/255, 0x5f/255, 0x7a/255] },
    { elevation: -100,  color: [0x6c/255, 0xa6/255, 0xc6/255] },
    { elevation: 0,     color: [0x9d/255, 0x95/255, 0x7b/255] },
    { elevation: 500,   color: [0x51/255, 0x4c/255, 0x3c/255] },
    { elevation: 2000,  color: [0x5e/255, 0x5c/255, 0x55/255] },
    { elevation: 5000,  color: [0xab/255, 0xab/255, 0xab/255] },
]
const EDITABLE_LATITUDE = 55 * Math.PI / 180 
const EDITABLE_LONGITUDE = (180 - 10) * Math.PI / 180


export function computeCellColor(elevation, seaLevel) {
    const adjusted = elevation - seaLevel
    
    if (adjusted <= ELEVATION_STOPS[0].elevation) return ELEVATION_STOPS[0].color
    if (adjusted >= ELEVATION_STOPS[ELEVATION_STOPS.length - 1].elevation) {
        return ELEVATION_STOPS[ELEVATION_STOPS.length - 1].color
    }
    
    for (let i = 0; i < ELEVATION_STOPS.length - 1; i++) {
        const a = ELEVATION_STOPS[i]
        const b = ELEVATION_STOPS[i + 1]
        if (adjusted >= a.elevation && adjusted <= b.elevation) {
        const t = (adjusted - a.elevation) / (b.elevation - a.elevation)
        return [
            a.color[0] + (b.color[0] - a.color[0]) * t,
            a.color[1] + (b.color[1] - a.color[1]) * t,
            a.color[2] + (b.color[2] - a.color[2]) * t,
        ]
        }
    }
}

export function applyElevationChange(cell, tool, power, options = {}) {
    const delta = power * 75  // In Meters
    const lerpAmount = power / 20 
    let target

    switch (tool) {
        case 'raise':
            cell.elevation += delta
            break
        case 'lower':
            cell.elevation -= delta
            break
        case 'smooth':
            target = options.smoothTarget
                if (target == null) break
                
                cell.elevation += (target - cell.elevation) * lerpAmount
        break
        case 'align':
            target = options.alignTarget
                if (target == null) break
                cell.elevation += (target - cell.elevation) * lerpAmount
        break
        default:
        // do nothing
        break
    }
}

export function getCellsInRadius(cellMap, centerCellId, radius) {
    const visited = new Map()  // cellId → distance from center
    const queue = [{ cellId: centerCellId, distance: 0 }]
    visited.set(centerCellId, 0)
    
    while (queue.length > 0) {
        const { cellId, distance } = queue.shift()
        
        const cell = cellMap.cellById.get(cellId)
        for (const neighborId of cell.geometry.neighborsById) {
        if (neighborId === 0) continue
        if (visited.has(neighborId)) continue
        if (distance + 1 > radius) continue
        
        visited.set(neighborId, distance + 1)
        queue.push({ cellId: neighborId, distance: distance + 1 })
        }
    }
    let avgElevation = 0
    for (const [cellId] of visited) {
        avgElevation += cellMap.cellById.get(cellId).elevation
    }
    avgElevation /= visited.size

    return {visited, avgElevation}
}

export function isCellEditable(cell) {
    const lat = Math.asin(cell.geometry.position.y)
    const lon = Math.atan2(cell.geometry.position.z, cell.geometry.position.x)
    if (Math.abs(lat) > EDITABLE_LATITUDE) return false
    if (Math.abs(lon) > EDITABLE_LONGITUDE) return false
    return true
}