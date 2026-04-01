import { useEffect, useState } from "react";


function WorldGenGeometryParameters({onParametersChange}) {
    const [seed, setSeed] = useState(Math.floor(Math.random() * 900_000) + 100_000);
    const [subdivisionLevel, setSubdivisionLevel] = useState(6);
    const [isValid, setIsValid] = useState(true)

    const cellCount = 20 * 4 ** subdivisionLevel
    
    function rerollSeed(){
        let newSeed = Math.floor(Math.random() * 900_000) + 100_000
        setIsValid(true)
        setSeed(newSeed)
        onParametersChange("geometry", {newSeed, subdivisionLevel}, true)
    }

    function handleSeedChange(e){
        let parsedValue = parseInt(e.target.value)
        
        if(e.target.value >= 1_000_000){
            parsedValue = Math.floor(parsedValue / 10) //Seed is capped at 6 digits. the 7th digit is removed.
        }

        setSeed(parsedValue)
    }
    function validateSeed(e){
        let parsedValue = parseInt(e.target.value)
        let newValid

        if(Number.isNaN(parsedValue)){
            newValid = false
        }
        if(parsedValue < 100_000)
        {
            newValid = false
        }
        if(parsedValue > 99_999 && parsedValue < 1_000_000)
        {
            newValid = true
        }
        setIsValid(newValid) 
        setSeed(parsedValue)
        onParametersChange("geometry", {parsedValue, subdivisionLevel}, newValid)
    }
    function handleSubdivisionChange(e){
        newLevel = parseInt(e.target.value)
        setSubdivisionLevel(newLevel)
        onParametersChange("geometry", {seed, newLevel}, isValid)
    }


    useEffect(() => {
        console.log(seed + " " + subdivisionLevel)
        onParametersChange("geometry", {seed, subdivisionLevel}, isValid) //upload 
        return () => {}
    }, [])
    return (
        <div>
            <label htmlFor="seed">Seed:</label>
            <input className={isValid ? "" : "invalid"} id="seed" type="number" value={seed} onChange={handleSeedChange} onBlur={validateSeed} title="Seed must be 6 digits"/>
            <button onClick={rerollSeed}>New Seed</button>
            <label htmlFor="subdivision">Cell Count: {cellCount}</label>
            <input id="subdivision" type="range" min="5" max="8" step="1" value={subdivisionLevel} onChange={handleSubdivisionChange} />
        </div>
    )
}
export default WorldGenGeometryParameters;