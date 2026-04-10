import WorldSphere from "./WorldSphere"
import useEngineHub from "../hooks/useEngineHub";
import { useEffect, useRef, useState } from "react";

function WorldView(){
    const sphereRef = useRef(null)
    const latestUpdate = useEngineHub()
    const currentColorIndex = useState(0)
    const plateColorOptions = [
        [0.263, 0.388, 0.847], // blue
        [0.235, 0.706, 0.294], // green
        [0.580, 0.000, 0.827], // violet
        [0.259, 0.831, 0.957], // cyan
        [0.941, 0.196, 0.902], // magenta
        [0.749, 0.937, 0.271], // lime
        [0.980, 0.745, 0.831], // pink
        [0.275, 0.600, 0.565], // teal
        [0.863, 0.745, 1.000], // lavender
        [0.502, 0.502, 0.000], // olive
        [1.000, 0.847, 0.694], // apricot
        [0.000, 0.000, 0.459], // navy
        [0.667, 1.000, 0.765], // mint
        [0.663, 0.663, 0.663], // grey
        [1.000, 1.000, 1.000], // white
        [1.000, 0.882, 0.098], // yellow
        [0.569, 0.118, 0.706], // purple
        [0.961, 0.510, 0.192], // orange — flag for plume proximity
        [0.902, 0.098, 0.294], // red
        [0.180, 0.800, 0.251], // bright green
        [0.000, 0.455, 0.851], // sky blue
        [0.498, 0.859, 1.000], // light blue
        [0.694, 0.051, 0.788], // bright purple
        [0.522, 0.078, 0.294], // maroon
        [0.239, 0.600, 0.439], // dark teal
        [1.000, 0.863, 0.000], // gold
        [0.004, 1.000, 0.439], // neon green
        [0.941, 0.071, 0.745], // hot pink
        [0.000, 0.122, 0.247], // midnight
        [0.420, 0.557, 0.137], // olive drab
    ]
    const plateColorMap = useState({})
    const plateNameMap = useState({})

    useEffect(() => {
        if (!latestUpdate || !sphereRef.current) return
        //console.log(latestUpdate)

        if(latestUpdate.currentstep == 'FillingInitialPlates')
        {
            const plates = latestUpdate.payload.plates
            Object.entries(plates).forEach(([plateId, plateName]) => {

                if(!plateColorMap[plateId]){
                    plateColorMap.set(parseint(plateId), plateColorOptions[currentColorIndex])
                }
                if(!plateNameMap[plateId]){
                    plateNameMap.set(parseint(plateId), plateName)
                }
            })
            
            sphereRef.current.newUpdate({
                currentStage: 'PlateColors',
                payload: plateColorMap
            })
        }
        sphereRef.current.newUpdate(latestUpdate)
    }, [latestUpdate])



    return(
        <div>
            <WorldSphere ref={sphereRef} />
        </div>
    )
}
export default WorldView