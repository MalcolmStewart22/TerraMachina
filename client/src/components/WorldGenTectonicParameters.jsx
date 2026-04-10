import { useEffect, useState } from "react";


function WorldGenTectonicParameters({onParametersChange}){

    const [tectonicParameters, setTectonicParameters] = useState({
        planetRadius: 6371,
        tectonicSimulationLength: 400,
        seaLevel: 3500,
        totalSeedPlumes: 20,
        minPlumeSize: 1500,
        maxPlumeSize: 2500,
        plumeCenterSize: .4,
        plumeFrequency: .3,
        plumeBaseStrength: .5,
        plumeConnectivityDistance: 1.5,
        plumeMaxAge: 150,
        ridgeWidth: 1500,
        maxLateralRidgeSway: 1,
        volcanoFrequency: .5,
        eulerPoleTolerance: .5,
        basePlateSpeed: 3
    })

    function onTectonicChange(e){
        setTectonicParameters(prev => ({ ...prev, [e.target.name]: parseFloat(e.target.value)}))
        passUp()
    }
    function passUp(){
        onParametersChange("tectonic", tectonicParameters, true)
    }
    useEffect(() => {
        passUp() //upload 
        return () => {}
    }, [])
    return(
        <div className="flex flex-col">
            <label htmlFor="planetRadius">Planet Radius: {tectonicParameters.planetRadius}</label>
            <input name="planetRadius" id="planetRadius" type="range" min="6000" max="10000" step="1" value={tectonicParameters.planetRadius} onChange={onTectonicChange}/>
            <label htmlFor="tectonicSimulationLength">Tectonic Phase Length: {tectonicParameters.tectonicSimulationLength} Million Years</label>
            <input name="tectonicSimulationLength" id="tectonicSimulationLength" type="range" min="100" max="1000" step="1" value={tectonicParameters.tectonicSimulationLength} onChange={onTectonicChange}/>
            <label htmlFor="seaLevel">Sea Level: {tectonicParameters.seaLevel}</label>
            <input name="seaLevel" id="seaLevel" type="range" min="10" max="4000" step="1" value={tectonicParameters.seaLevel} onChange={onTectonicChange}/>

            <label htmlFor="totalSeedPlumes">Total Seed Plumes: {tectonicParameters.totalSeedPlumes}</label>
            <input name="totalSeedPlumes" id="totalSeedPlumes" type="range" min="10" max="20" step="1" value={tectonicParameters.totalSeedPlumes} onChange={onTectonicChange}/>

            <label htmlFor="minPlumeSize">Min Plume Size: {tectonicParameters.minPlumeSize}</label>
            <input name="minPlumeSize" id="minPlumeSize" type="range" min="1000" max="2000" step="1" value={tectonicParameters.minPlumeSize} onChange={onTectonicChange}/>

            <label htmlFor="maxPlumeSize">Max Plume Size: {tectonicParameters.maxPlumeSize}</label>
            <input name="maxPlumeSize" id="maxPlumeSize" type="range" min="2000" max="3000" step="1" value={tectonicParameters.maxPlumeSize} onChange={onTectonicChange}/>

            <label htmlFor="plumeCenterSize">Plume Center Size: {(tectonicParameters.plumeCenterSize * 100).toFixed(0)}%</label>
            <input name="plumeCenterSize" id="plumeCenterSize" type="range" min="0" max="1" step="0.01" value={tectonicParameters.plumeCenterSize} onChange={onTectonicChange}/>

            <label htmlFor="plumeFrequency">Plume Frequency: {tectonicParameters.plumeFrequency}</label>
            <input name="plumeFrequency" id="plumeFrequency" type="range" min="0" max="1" step="0.01" value={tectonicParameters.plumeFrequency} onChange={onTectonicChange}/>

            <label htmlFor="plumeBaseStrength">Plume Base Strength: {(tectonicParameters.plumeBaseStrength * 10).toFixed(0)}</label>
            <input name="plumeBaseStrength" id="plumeBaseStrength" type="range" min="0" max="1" step="0.01" value={tectonicParameters.plumeBaseStrength} onChange={onTectonicChange}/>

            <label htmlFor="plumeConnectivityDistance">Plume Connectivity Distance: {(tectonicParameters.plumeConnectivityDistance * 100).toFixed(0)}%</label>
            <input name="plumeConnectivityDistance" id="plumeConnectivityDistance" type="range" min=".75" max="2" step="0.1" value={tectonicParameters.plumeConnectivityDistance} onChange={onTectonicChange}/>

            <label htmlFor="plumeMaxAge">Plume Max Age: {tectonicParameters.plumeMaxAge} Million Years</label>
            <input name="plumeMaxAge" id="plumeMaxAge" type="range" min="10" max="100" step="1" value={tectonicParameters.plumeMaxAge} onChange={onTectonicChange}/>

            <label htmlFor="ridgeWidth">Ridge Width: {tectonicParameters.ridgeWidth}</label>
            <input name="ridgeWidth" id="ridgeWidth" type="range" min="500" max="3000" step="1" value={tectonicParameters.ridgeWidth} onChange={onTectonicChange}/>

            <label htmlFor="maxLateralRidgeSway">Max Lateral Ridge Sway: {tectonicParameters.maxLateralRidgeSway}</label>
            <input name="maxLateralRidgeSway" id="maxLateralRidgeSway" type="range" min="0" max="3" step="0.1" value={tectonicParameters.maxLateralRidgeSway} onChange={onTectonicChange}/>
            
            <label htmlFor="volcanoFrequency">Volcano Frequency: {(tectonicParameters.volcanoFrequency * 10).toFixed(0)}</label>
            <input name="volcanoFrequency" id="volcanoFrequency" type="range" min="0" max="1" step="0.01" value={tectonicParameters.volcanoFrequency} onChange={onTectonicChange}/>

            <label htmlFor="eulerPoleTolerance">Euler Pole Tolerance: {tectonicParameters.eulerPoleTolerance}</label>
            <input name="eulerPoleTolerance" id="eulerPoleTolerance" type="range" min="0" max="1" step="0.01" value={tectonicParameters.eulerPoleTolerance} onChange={onTectonicChange}/>

            <label htmlFor="basePlateSpeed">Base Plate Speed: {tectonicParameters.basePlateSpeed} cm per Year</label>
            <input name="basePlateSpeed" id="basePlateSpeed" type="range" min="1" max="10" step="0.5" value={tectonicParameters.basePlateSpeed} onChange={onTectonicChange}/>
        </div>
    )
}
export default WorldGenTectonicParameters


