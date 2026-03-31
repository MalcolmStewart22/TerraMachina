import WorldSphere from "./WorldSphere"
import useEngineHub from "../hooks/useEngineHub";
import { useEffect, useRef } from "react";

function WorldView(){
    const sphereRef = useRef(null)
    const latestUpdate = useEngineHub()


    useEffect(() => {
        if (!latestUpdate || !sphereRef.current) return
        console.log(latestUpdate)
        sphereRef.current.newUpdate(latestUpdate)
    }, [latestUpdate])



    return(
        <div>
            <WorldSphere ref={sphereRef} />
        </div>
    )
}
export default WorldView