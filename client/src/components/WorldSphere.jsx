import { useEffect, useRef, forwardRef, useImperativeHandle  } from "react";
import {ElementProxy, eventHandlers} from "../workers/ElementProxy";

const WorldSphere = forwardRef((props, ref) =>{
    const canvasRef = useRef(null)
    const workerRef = useRef(null)
    const proxyRef = useRef(null)
    const handlerRef = useRef(null)

    useImperativeHandle(ref, () => ({
        newUpdate : (latestUpdate) => {
            if(!workerRef.current) return
            handlerRef.current = updateHandlers[latestUpdate?.currentStage]
            if(handlerRef.current) handlerRef.current(latestUpdate)
        }
    }))

    const updateHandlers = {
        'Starting': handleStarting,
        'Geometry': handleGeometry,
        'Tectonics': handleTectonics
    }

    function handleStarting(latestUpdate){
        workerRef.current.postMessage({type: 'prepare', cellCount: latestUpdate.stageProgress})
    }
    function handleGeometry(latestUpdate){
        if (latestUpdate?.payload){
            workerRef.current.postMessage({type: 'geometry', payload: latestUpdate.payload })
        }
        if(latestUpdate?.stageProgress === 1){
            workerRef.current.postMessage({type: 'lod', payload: latestUpdate.payload })
        }
    }

    function handleTectonics(latestUpdate){

    }


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

    return (
        <div>
            <canvas ref={canvasRef} />
        </div>
    )

})
export default WorldSphere;