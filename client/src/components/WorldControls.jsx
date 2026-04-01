import WorldGenGeometryParameters from "./WorldGenGeometryParameters"
import { startWorldGen } from "../api/engineApi";
import { useState } from "react";



function WorldControls(){
    const[parameters, setParameters] = useState({})
    const[valid, setValid] = useState({})
    const isFormValid = Object.values(valid).every(v => v === true)

    
    async function submitChanges(){
        console.log(parameters)
        await startWorldGen(parameters)
    }

    function handleParameterChange(phase, values, isValid){
        setParameters(prev => ({...prev, [phase]: values}))
        setValid(prev => ({...prev, [phase]: isValid}))
    }

    return(
        <div>
            <WorldGenGeometryParameters onParametersChange={handleParameterChange}/>
            <button disabled={!isFormValid} onClick={submitChanges}>Generate World</button>
        </div>
    )
}
export default WorldControls