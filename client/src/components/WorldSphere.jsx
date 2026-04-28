import { useEffect, useRef, forwardRef, useImperativeHandle  } from "react";
import {elementProxy, eventHandlers} from "../workers/elementProxy";

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
        'Tectonic': handleTectonic,
        'PlateColors' : handlePlateColors,
        'Reset' : handleReset 
    }

    function handleStarting(latestUpdate){
        workerRef.current.postMessage({type: 'prepare', cellCount: latestUpdate.stageProgress})
    }
    function handleGeometry(latestUpdate){
        if (latestUpdate?.payload){
                workerRef.current.postMessage({type: 'geometry', payload: latestUpdate.payload })
        }
        else if(latestUpdate.stageProgress === 1){
            workerRef.current.postMessage({type: 'lod', payload: latestUpdate.payload })
        }
    }

    function handleTectonic(latestUpdate){
        if (latestUpdate?.payload){
                workerRef.current.postMessage({type: 'plate', payload: latestUpdate.payload })
        }
        else if(latestUpdate.stageProgress === 0)
            {
                workerRef.current.postMessage({type: 'prepareTectonic'})
            }
    }

    function handlePlateColors(latestUpdate){
        if (latestUpdate?.payload){
            workerRef.current.postMessage({type: 'plateColors', payload: latestUpdate.payload })
        }
    }

    function handleReset(latestUpdate){
        workerRef.current.postMessage({type: 'reset' })

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