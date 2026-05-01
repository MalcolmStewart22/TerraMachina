function ToolPanel({ activeLayer, setActiveLayer, activeBrush, setActiveBrush, activeTool, setActiveTool, brushSize, setBrushSize, brushPower, setBrushPower,}) {  
    const selectedBrush = (brush) => {
        const base = "p-1 flex-1"
        const active = "bg-accent"
        const inactive = "text-muted hover:text-ink"
        return `${base} ${activeBrush === brush ? active : inactive}`
    }

    const selectedTool = (tool) => {
        const base = "border border-accent p-1"
        const active = "bg-accent text-ink"
        const inactive = "hover:text-muted"
        return `${base} ${activeTool === tool ? active : inactive}`
    }
    
    const changeBrush = (brush) => {
        setActiveTool(null)
        setActiveBrush(activeBrush === brush ? null : brush)
        setActiveLayer(brush) 
    }
    return (
    <div className="absolute top-[81%] left-1/2 -translate-x-1/2 flex-col border-1 border-accent w-64">
        {/* button strip */}
        <div className="flex  border-b border-elevated bg-elevatedLight">
            <button className={selectedBrush('elevation')} onClick={() => changeBrush('elevation')}>
            Elevation
            </button>
            <button className={selectedBrush('biome')}  onClick={() => changeBrush('biome')}>
            Biome
            </button>
            {/* more tool buttons later */}
        </div>
      
      {/* Tools*/}
      {activeBrush != null && (
        <div className=" flex-col bg-elevatedLight text-ink border-elevatedLight flex items-center">
            {/* Elevation*/}
            {activeBrush === 'elevation' && (
                <div className="p-2 flex gap-1">
                    <button className={selectedTool('raise')} onClick={() => setActiveTool(activeTool === 'raise' ? null : 'raise')}>
                        Raise
                    </button> 
                    <button className={selectedTool('lower')} onClick={() => setActiveTool(activeTool === 'lower' ? null : 'lower')}>
                        Lower
                    </button>
                    <button className={selectedTool('smooth')} onClick={() => setActiveTool(activeTool === 'smooth' ? null : 'smooth')}>
                        Smooth
                    </button>
                    <button className={selectedTool('align')} onClick={() => setActiveTool(activeTool === 'align' ? null : 'align')}>
                        Align
                    </button>      
                </div>
            )}
            {activeBrush === 'biome' && (
                <select className="border border-accent bg-elevatedLight text-ink mt-1" onChange={e => setActiveTool(e.target.value)} value={activeTool ?? ''}>
                    <option value="">-- Select Biome --</option>
                    <option value="TropicalRainforest">Tropical Rainforest</option>
                    <option value="TropicalSeasonalRainforest">Tropical Seasonal Rainforest</option>
                    <option value="Savanna">Savanna</option>
                    <option value="TemperateRainforest">Temperate Rainforest</option>
                    <option value="TemperateSeasonalRainforest">Temperate Seasonal Rainforest</option>
                    <option value="BorealForest">Boreal Forest</option>
                    <option value="Shrubland">Shrubland</option>
                    <option value="TropicalWetland">Tropical Wetland</option>
                    <option value="TemperateWetland">Temperate Wetland</option>
                    <option value="BorealWetland">Boreal Wetland</option>
                    <option value="TemperateGrassland">Temperate Grassland</option>
                    <option value="HotDesert">Hot Desert</option>
                    <option value="ColdDesert">Cold Desert</option>
                    <option value="Tundra">Tundra</option>
                    <option value="PolarDesert">Polar Desert</option>
                </select>
        
            )}
            <div className="flex flex-col mb-1">
                <div className="flex gap-1">
                    <label> Brush Size: {brushSize}</label>
                    <input type="range" min="1" max="10" value={brushSize} 
                        onChange={(e) => setBrushSize(Number(e.target.value))} />
                </div>
                <div className="flex gap-1">
                    <label> Power: {brushPower}</label>
                    <input type="range" min="1" max="10" value={brushPower} 
                        onChange={(e) => setBrushPower(Number(e.target.value))} />
                </div>
            </div>  
                 
        </div>
      )}
    </div>
  )
}

export default ToolPanel