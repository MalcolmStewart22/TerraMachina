import { useState } from "react";
import { startWorldGen } from "../api/engineApi";


function WorldGenForm() {
    const [seed, setSeed] = useState(0);
    const [subdivisionLevel, setSubdivisionLevel] = useState(1);

    const cellCount = 20 * 4 ** subdivisionLevel
    
    async function submitChanges(){
        await startWorldGen(seed, subdivisionLevel)
    }


    return (
        <div>
            <label htmlFor="seed">Seed:</label>
            <input id="seed" type="number" value={seed} onChange={e => setSeed(parseInt(e.target.value))} />
            <label htmlFor="subdivision">Cell Count: {cellCount}</label>
            <input id="subdivision" type="range" min="1" max="10" step="1" value={subdivisionLevel} onChange={e => setSubdivisionLevel(parseInt(e.target.value))} />
            <button onClick={submitChanges}>Generate World</button>
        </div>
    )
}
export default WorldGenForm;