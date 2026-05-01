const ELEVATION_STOPS = [
    { elevation: -5000, color: [0x2b/255, 0x5f/255, 0x7a/255] },
    { elevation: -100,  color: [0x6c/255, 0xa6/255, 0xc6/255] },
    { elevation: 0,     color: [0x9d/255, 0x95/255, 0x7b/255] },
    { elevation: 1500,   color: [0x3d/255, 0x36/255, 0x1f/255] },
    { elevation: 5000,  color: [0x51/255, 0x51/255, 0x51/255] },
    { elevation: 10000,  color: [0xd5/255, 0xd5/255, 0xd5/255] },
]
const BIOME_COLORS = {
    TropicalRainforest:           [0x0D/255, 0x63/255, 0x20/255], //0D6320
    TropicalSeasonalRainforest:   [0x3E/255, 0x70/255, 0x3A/255], //3e703a
    TropicalWetland:              [0x09/255, 0x7E/255, 0x2E/255], //097E2E

    TemperateRainforest:          [0x27/255, 0x53/255, 0x33/255], //275333
    TemperateSeasonalRainforest:  [0x46/255, 0x68/255, 0x36/255], //466836
    TemperateWetland:             [0x22/255, 0x5E/255, 0x3C/255], //225E3C
    Shrubland:                    [0x65/255, 0x7F/255, 0x4D/255], //657f4d

    BorealForest:                 [0x1F/255, 0x3F/255, 0x26/255], //1f3f26
    BorealWetland:                [0x14/255, 0x46/255, 0x2C/255], //14462c

    Savanna:                      [0xAF/255, 0xB6/255, 0x75/255], //afb675 
    TemperateGrassland:           [0x76/255, 0x7F/255, 0x46/255], //767f46
    Tundra:                       [0x3C/255, 0x44/255, 0x18/255], //3c4418

    HotDesert:                    [0xBB/255, 0x9E/255, 0x6F/255], //BB9E6F
    ColdDesert:                   [0xA7/255, 0x95/255, 0x6F/255], //A7956F
    PolarDesert:                  [0xD5/255, 0xDC/255, 0xDB/255], //d5dcdb
    
    None:                         [0x8A/255, 0x8A/255, 0x8A/255]  //8a8a8a
}

const EDITABLE_LATITUDE = 55 * Math.PI / 180 
const EDITABLE_LONGITUDE = (180 - 10) * Math.PI / 180


export function computeCellElevationColor(elevation, seaLevel) {
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
export function computeBiomeColor(cell, seaLevel) {
    if (cell.elevation < seaLevel) return [0x2b/255, 0x5f/255, 0x7a/255]
    if (!cell.biome) return BIOME_COLORS.None
    return BIOME_COLORS[cell.biome] ?? BIOME_COLORS.None
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
    if(cell.elevation > options.seaLevel) cell.isOcean = false
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