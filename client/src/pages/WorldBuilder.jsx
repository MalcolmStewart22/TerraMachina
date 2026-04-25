import HeaderLinks from "../components/HeaderLinks"
import { useState, useEffect } from "react"
import { generateSphere } from "../builder/Icosphere"
import { buildGeometry } from "../builder/projection"
import MapCanvas from "../components/builder/MapCanvas"
import ToolPanel from "../components/builder/ToolPanel"


const EDITABLE_LATITUDE = 55 * Math.PI / 180 
const EDITABLE_LONGITUDE = (180 - 10) * Math.PI / 180

function WorldBuilder() {
  const [cellMap, setCellMap] = useState(null)
  const [geometry, setGeometry] = useState(null)
  const [activeBrush, setActiveBrush] = useState(null)
  const [brushSize, setBrushSize] = useState(5)
  const [brushPower, setBrushPower] = useState(5)
  const [activeTool, setActiveTool] = useState(null)

  useEffect(() => {
    const map = generateSphere()
    const geo = buildGeometry(map)
    setCellMap(map)
    setGeometry(geo)
  }, [])

  const handleCellClick = (cellId) =>{
    const cell = cellMap.cellById.get(cellId)
    const position = cell.geometry.position
    
    const latitude = Math.asin(position.y)
    if (Math.abs(latitude) > EDITABLE_LATITUDE) return
    
    const longitude = Math.atan2(position.z, position.x)
    if (Math.abs(longitude) > EDITABLE_LONGITUDE) return

    console.log("Clicked Cell: ", cellId, "Latitude: ", latitude)
  }
    if (!cellMap) return <div>Generating world...</div>

  return(
    <div className="bg-base text-ink min-h-screen flex flex-col">
      <HeaderLinks/>
      <div className=" flex-col flex-1 relative">
        <MapCanvas geometry={geometry} cellMap={cellMap} onCellClick={handleCellClick}/>
        <ToolPanel 
          activeBrush={activeBrush}
          setActiveBrush={setActiveBrush}
          activeTool={activeTool}
          setActiveTool={setActiveTool}
          brushSize={brushSize}
          setBrushSize={setBrushSize}
          brushPower={brushPower}
          setBrushPower={setBrushPower}
        />
      </div>
    </div>
  ) 
}

export default WorldBuilder