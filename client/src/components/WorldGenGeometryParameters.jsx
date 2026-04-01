import { useState } from "react";


function WorldGenGeometryParameters({onParametersChange}) {
    const [seed, setSeed] = useState(Math.floor(Math.random() * 900_000) + 100_000);
    const [subdivisionLevel, setSubdivisionLevel] = useState(6);
    const [isValid, setIsValid] = useState(true)


    const cellCount = 20 * 4 ** subdivisionLevel
    
    function rerollSeed(){
        let newSeed = Math.floor(Math.random() * 900_000) + 100_000
        setSeed(newSeed)
        onParametersChange("geometry", {seed, subdivisionLevel}, isValid)
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

        if(Number.isNaN(parsedValue)){
            setIsValid(false)
            onParametersChange("geometry", {seed, subdivisionLevel}, isValid)
        }

        if(parsedValue < 100_000)
        {
            setIsValid(false)
            onParametersChange("geometry", {seed, subdivisionLevel}, isValid)
        }

        if(parsedValue > 99_999 && parsedValue < 1_000_000)
        {
            setIsValid(true)
            onParametersChange("geometry", {seed, subdivisionLevel}, isValid)
        }
        setSeed(parsedValue)
    }
    function handleSubdivisionChange(e){
        setSubdivisionLevel(parseInt(e.target.value))
        onParametersChange("geometry", {seed, subdivisionLevel}, isValid)
    }

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