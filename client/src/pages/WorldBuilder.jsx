import HeaderLinks from "../components/HeaderLinks"
import { useState, useEffect } from "react"
import { generateSphere } from "../builder/icosphere"
import { buildGeometry } from "../builder/projection"
import MapCanvas from "../components/builder/MapCanvas"
import ToolPanel from "../components/builder/ToolPanel"
import LayerPanel from '../components/builder/LayerPanel'
import { createWorld } from "../builder/world"


function WorldBuilder() {
  const [world, setWorld] = useState(null)
  const [geometry, setGeometry] = useState(null)

  const [activeBrush, setActiveBrush] = useState('elevation')
  const [activeLayer, setActiveLayer] = useState('elevation')
  const [brushSize, setBrushSize] = useState(5)
  const [brushPower, setBrushPower] = useState(5)
  const [activeTool, setActiveTool] = useState(null)

  useEffect(() => {
    const map = generateSphere()
    const geo = buildGeometry(map)
    setWorld(createWorld(map))
    setGeometry(geo)
  }, [])


    if (!world) return <div>Generating world...</div>

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
            activeBrush={activeBrush}
            setActiveBrush={setActiveBrush}
          />
          <MapCanvas 
            geometry={geometry} 
            world={world} 
            activeBrush={activeBrush}
            activeTool={activeTool}
            brushSize={brushSize}
            brushPower={brushPower}
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