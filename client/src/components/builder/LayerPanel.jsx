function LayerPanel({ activeLayer, setActiveLayer, activeBrush, setActiveBrush, setActiveTool}) {  
    const selectedLayer = (layer) => {
        const base = "border-l border-r border-accent p-1 flex-1"
        const active = "bg-accent"
        const inactive = "text-muted hover:text-ink"
        return `${base} ${activeLayer === layer ? active : inactive}`
    }


    const changeLayer = (brush) => {
        setActiveTool(null)
        setActiveLayer(brush)
        setActiveBrush(activeBrush === brush ? null : brush)
    }
    
    return (
    <div className="absolute left-[2%] top-1/4 -translate-y-1/2 z-1 flex border-2 border-accent h-64">
      {/* button strip */}
      <div className="flex flex-col  border-b border-elevated bg-elevated">
        <button className={selectedLayer('elevation')} onClick={() => changeLayer('elevation')}>
          Elevation
        </button>
        <button className={selectedLayer('biome')}  onClick={() => changeLayer('biome')}>
          Biome
        </button>
        {/* more tool buttons later */}
      </div>
      
      {/* Tools*/}
      {activeLayer != null && (
        <div className=" flex-col bg-muted text-base border-elevated flex items-center">
            {/* Elevation*/}
            {activeLayer === 'elevation' && (
                <div className="p-2 flex gap-1">
     
                </div>
            )}
            {activeLayer === 'biome' && (
                <div className="flex flex-col mb-1">

                </div> 
            )}
            
                 
        </div>
      )}
    </div>
  )
}

export default LayerPanel