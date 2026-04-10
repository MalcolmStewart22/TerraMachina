import WorldGenGeometryParameters from "./WorldGenGeometryParameters"
import WorldGenTectonicParameters from "./WorldGenTectonicParameters"
import { startWorldGen, resetWorldGen } from "../api/engineApi";
import { useState } from "react";



function WorldControls(){
    const[parameters, setParameters] = useState({})
    const[valid, setValid] = useState({})
    const isFormValid = Object.values(valid).every(v => v === true)

    
    async function submitChanges(){
        await startWorldGen(parameters)
    }

    function handleParameterChange(phase, values, isValid){
        setParameters(prev => ({...prev, [phase]: values}))
        setValid(prev => ({...prev, [phase]: isValid}))
    }
    async function reset(){
        await resetWorldGen()
    }
    return(
        <div className="h-screen">
            <h1 className="text-3xl font-bold px-10">Input Parameters</h1>
            <div className=" px-10 py-4 items-center border-solid border-5">
                <div className="flex flex-col overflow-y-auto">
                    <details>
                        <summary className="text-2xl font-bold">Geometry Parameters</summary>
                        <WorldGenGeometryParameters onParametersChange={handleParameterChange}/>
                    </details>
                    <details>
                        <summary className="text-2xl font-bold">Tectonic Parameters</summary>
                        <WorldGenTectonicParameters onParametersChange={handleParameterChange}/>
                    </details>
                </div>
                <button className="button-85" role="button" disabled={!isFormValid} onClick={submitChanges}>Generate World</button>
                <button className="button-85" role="button" onClick={reset}>Reset</button>
            </div>
        </div>
    )
}
export default WorldControls