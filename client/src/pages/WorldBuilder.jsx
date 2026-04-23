import HeaderLinks from "../components/HeaderLinks"
import { useState, useEffect } from "react"
import { generateSphere } from "../builder/Icosphere"
import { buildGeometry } from "../builder/projection"
import BuilderMapCanvas from "../components/BuilderMapCanvas"




function WorldBuilder() {
  const [cellMap, setCellMap] = useState(null)
  const [geometry, setGeometry] = useState(null)


  useEffect(() => {
    const map = generateSphere()
    const geo = buildGeometry(map)
    setCellMap(map)
    setGeometry(geo)
  }, [])

    if (!cellMap) return <div>Generating world...</div>

  return(
    <div className="bg-base text-ink min-h-screen flex  flex-col">
      <HeaderLinks/>
      <div className="flex-1">
        <BuilderMapCanvas geometry={geometry} cellMap={cellMap} />
      </div>
    </div>
  ) 
}

export default WorldBuilder