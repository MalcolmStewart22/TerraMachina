import { useEffect, useRef } from "react";
import useEngineHub from "../hooks/useEngineHub";
import {ElementProxy, eventHandlers} from "../workers/ElementProxy";

function WorldSphere() {
    
    const canvasRef = useRef(null)
    const latestUpdate = useEngineHub()
    const workerRef = useRef(null)
    const proxyRef = useRef(null)
    
    useEffect(() => {
        const worker = new Worker(
            new URL('../workers/renderWorkers.js', import.meta.url),
            {type:'module'}
        )
        workerRef.current = worker
        
        proxyRef.current = new ElementProxy(canvasRef.current, worker, eventHandlers);
        
        const offscreen = canvasRef.current.transferControlToOffscreen()
        workerRef.current.postMessage({type: 'init', canvas: offscreen, canvasId: proxyRef.current.id, width: window.innerWidth, height: window.innerHeight, pixelRatio: window.devicePixelRatio}, [offscreen])
        
        return () => {
            workerRef.current.postMessage({ type: 'cleanup' })
            workerRef.current.terminate()
        }
    }, [])

    useEffect(() => {
        console.log(latestUpdate)
        
        if(!workerRef.current) return

        //Render Setup
        if(latestUpdate?.currentStage === 'Starting')
        {
            workerRef.current.postMessage({type: 'prepare', cellCount: latestUpdate.stageProgress})
        }

        // Geometry Messages
        if(latestUpdate?.currentStage === "Geometry"){
            if (latestUpdate?.payload){
                workerRef.current.postMessage({type: 'geometry', payload: latestUpdate.payload })
            }
            if(latestUpdate?.stageProgress === 1){
                workerRef.current.postMessage({type: 'lod', payload: latestUpdate.payload })
            }
        }
        
        //Tectonic Messages
        
    }, [latestUpdate])

    return (
        <div>
            <canvas ref={canvasRef} />
        </div>
    )

}
export default WorldSphere;