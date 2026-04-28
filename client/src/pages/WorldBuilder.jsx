import HeaderLinks from "../components/HeaderLinks"
import { useState, useEffect } from "react"
import { generateSphere } from "../builder/Icosphere"
import { buildGeometry } from "../builder/projection"
import MapCanvas from "../components/builder/MapCanvas"
import ToolPanel from "../components/builder/ToolPanel"
import LayerPanel from '../components/builder/LayerPanel'


function WorldBuilder() {
  const [cellMap, setCellMap] = useState(null)
  const [geometry, setGeometry] = useState(null)

  const [activeBrush, setActiveBrush] = useState('elevation')
  const [activeLayer, setActiveLayer] = useState('elevation')
  const [brushSize, setBrushSize] = useState(5)
  const [brushPower, setBrushPower] = useState(5)
  const [activeTool, setActiveTool] = useState(null)

  useEffect(() => {
    const map = generateSphere()
    const geo = buildGeometry(map)
    setCellMap(map)
    setGeometry(geo)
  }, [])


    if (!cellMap) return <div>Generating world...</div>

  return(
    <div className="bg-base text-ink min-h-screen flex flex-col">
      <HeaderLinks/>
      <div className=" flex flex-1 relative">
        <div className=" flex-col">
          <LayerPanel
            activeLayer={activeLayer}
            setActiveLayer={setActiveLayer}
            activeBrush={activeBrush}
            setActiveBrush={setActiveBrush}
            setActiveTool={setActiveTool}
          />
          <MapCanvas 
            geometry={geometry} 
            cellMap={cellMap} 
            activeBrush={activeBrush}
            activeTool={activeTool}
            brushSize={brushSize}
            brushPower={brushPower}
            seaLevel={5000}
            activeLayer={activeLayer}


          />
          <ToolPanel 
            activeLayer={activeLayer}
            setActiveLayer={setActiveLayer}
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
    </div>
  ) 
}

export default WorldBuilder