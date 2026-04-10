import * as THREE from 'three';
import { OrbitControls } from "three/examples/jsm/Addons.js";
import { ProxyManager } from './ElementProxyReceiver.js';
import { sendReadySignal, resetWorldGen } from "../api/engineApi";


let scene, camera, renderer, controls, animationId, ready
let mesh, lines, positionBuffer, positionAttribute, colorBuffer, colorAttribute
let cellcount,  offset, cellOffset
let plateColorMap
let pendingBatches = []
let cellIDs = []
let mantleCells = new Set()
let centerCells = new Set()
const proxyManager = new ProxyManager()
const plumeCenterColor = [1.000, 0.333, 0.000]
const plumeEdgeColor = [1.0, 0.15, 0.0]


const handlers = {
    init: handleInit,
    geometry: handleGeometry,
    cleanup: handleCleanup,
    makeProxy: (data) => proxyManager.makeProxy(data),
    event: proxyManager.handleEvent,
    prepare: handlePrepare,
    lod: handleLODs,
    plate: handleTectonicPlates,
    tectonic: handleTectonicSimulation,
    prepareTectonic: handleTectonicSetup,
    plateColors: handlePlateColors,
    reset: handleReset
}

self.onmessage = (e) => {
    const fn = handlers[e.data.type];
    if (!fn) throw new Error('Unknown message type: ' + e.data.type);
    fn(e.data);
}

function handleInit(data){
    if(ready) return

    const canvas = data.canvas
    const width = data.width
    const height = data.height
    const pixelRatio = data.pixelRatio
    camera = new THREE.PerspectiveCamera(75, width/ height, 0.1,1000)
    camera.position.z = 3
            
    renderer = new THREE.WebGLRenderer({canvas:canvas})
    renderer.setSize(width, height, false)
    renderer.setPixelRatio(pixelRatio)
            
    scene = new THREE.Scene()


    const proxy = proxyManager.getProxy(data.canvasId);
    proxy.ownerDocument = proxy;
    self.document = {};

    controls = new OrbitControls(camera, proxy)
    controls.minDistance = 1.25
    controls.maxDistance = 5
    controls.rotateSpeed = .25
 
    const animate = () => {
        animationId = requestAnimationFrame(animate)
        renderer.render(scene,camera)
        controls.update()
    }
    animate()
    
    ready = true
}

function handleCleanup(data){
    cancelAnimationFrame(animationId)
    renderer.dispose()
}

function handleGeometry(data){
    if(!positionBuffer){
        pendingBatches.push(data)
        return
    }
    const payload = data.payload
    for (const cell of payload.sphere){
        cellIDs[cell.id] = cellOffset
        cellOffset += 9
        for (const vertex of cell.v){
            positionBuffer[offset] = vertex.x
            positionBuffer[offset + 1] = vertex.y
            positionBuffer[offset + 2] = vertex.z
            offset += 3
        }
    }                
        
    positionAttribute.needsUpdate = true

}

function handlePrepare(data){
    cellcount = data.cellCount
    console.log(cellcount)
    offset = 0
    cellOffset = 0
    cellIDs = new Int32Array(cellcount)

    positionBuffer = new Float32Array(cellcount * 9)
    positionAttribute = new THREE.BufferAttribute(positionBuffer, 3)
    positionAttribute.setUsage(THREE.DynamicDrawUsage)

    colorBuffer = new Float32Array(cellcount * 9)
    colorBuffer.fill(127 / 255)
    colorAttribute = new THREE.BufferAttribute(colorBuffer, 3)

    scene.remove(mesh)
    const geometry = new THREE.BufferGeometry()
    geometry.setAttribute('position', positionAttribute)
    geometry.setAttribute('color', colorAttribute)
    const meshMaterial = new THREE.MeshBasicMaterial({color: 0x7F7F7F, side: THREE.DoubleSide})
    mesh = new THREE.Mesh(geometry,meshMaterial)
    scene.add(mesh)

    scene.remove(lines)
    const lineMaterial = new THREE.MeshBasicMaterial({ color: 0x000000, wireframe: true})
    lines = new THREE.Mesh(geometry, lineMaterial)
    scene.add(lines)

    if(pendingBatches.length)
    {
        for(const batch of pendingBatches)
        {
            handleGeometry(batch)
        }
        pendingBatches = []
    }
}

function handleLODs(data){
    // Create LODs
    // Do work
    // We will deal with this later
    console.log("No LODS please send tectonics!")
    readyForNextPhase("geometry")
}

async function readyForNextPhase(currentPhase){
    let nextPhase
    switch(currentPhase)
    {
        case "geometry":
            nextPhase = 2;
            break;
    }
    await sendReadySignal(nextPhase)
}

function handleTectonicSetup(){
    console.log("Preparing for Tectonics!")
    const meshMaterial = new THREE.MeshBasicMaterial({})
    meshMaterial.vertexColors = true
    mesh.material.dispose()
    mesh.material = meshMaterial
    mesh.material.needsUpdate = true
}

function handlePlateColors(data){
    plateColorMap = data.payload
}

function handleTectonicPlates(data){
    const payload = data.payload
    let updateNeeded = false
    if(payload.plumes)
    {
        console.log("We have plumes!")
        for (const plume of payload.plumes){
            const centerSet = new Set(plume.centerCells.map(c => c.cellID))
            for (const cell of plume.containedCells)
            {
                let offset = cellIDs[cell.cellID]
                mantleCells.add(cell.cellID)

                if(centerSet.has(cell.cellID)){
                    applyColor(plumeCenterColor, offset)
                    centerCells.add(cell.cellID)
                }
                else if(!centerCells.has(cell.cellID)){
                    
                    applyColor(plumeEdgeColor, offset)
                }
            }
        }
        updateNeeded = true
    }
    if(payload.ridges)
    {
        console.log("We have Ridges!")
        for (const ridge of payload.ridges){
            const centerSet = new Set(ridge.centerCells.map(c => c.cellID))
            for (const cell of ridge.containedCells)
            {
                let offset = cellIDs[cell.cellID]
                mantleCells.add(cell.cellID)

                if(centerSet.has(cell.cellID)){
                    applyColor(plumeCenterColor, offset)
                    centerCells.add(cell.cellID)
                }
                else if(!centerCells.has(cell.cellID)){
                    
                    applyColor(plumeEdgeColor, offset)
                }          
            }
        }
        updateNeeded = true
    }
    if(payload.tectonicCells)
    {
        console.log("We have Cells!")
        for (const cell of payload.tectonicCells)
        {
            if(mantleCells.has(cell.cellID)) continue

            let offset = cellIDs[cell.cellID]
            let color = plateColorMap[cell.plateID]

            applyColor(color, offset)
            
        }
        updateNeeded = true
    }
    colorAttribute.needsUpdate = updateNeeded
}

function applyColor(color, offset){
    //R
    colorBuffer[offset] = color[0]
    colorBuffer[offset + 3] = color[0]
    colorBuffer[offset + 6] = color[0]
    //G
    colorBuffer[offset + 1] = color[1]
    colorBuffer[offset + 4] = color[1]
    colorBuffer[offset + 7] = color[1]
    //B
    colorBuffer[offset + 2] = color[2]
    colorBuffer[offset + 5] = color[2]
    colorBuffer[offset + 8] = color[2]
}

function handleTectonicSimulation(data){

}

function handleReset(){
    scene.remove(mesh)
    mesh.geometry.dispose()
    mesh.material.dispose()

    scene.remove(lines)
    lines.geometry.dispose()
    lines.material.dispose()

    console.log("Reset Complete!")
}