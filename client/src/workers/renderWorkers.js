import * as THREE from 'three';
import { OrbitControls } from "three/examples/jsm/Addons.js";
import { ProxyManager } from './ElementProxyReceiver.js';


let scene, camera, renderer, controls, animationId
let mesh, lines
let cellcount, positionBuffer, positionAttribute, offset
let pendingBatches = []
const proxyManager = new ProxyManager()


const handlers = {
    init: handleInit,
    geometry: handleGeometry,
    cleanup: handleCleanup,
    makeProxy: (data) => proxyManager.makeProxy(data),
    event: proxyManager.handleEvent,
    prepare: handlePrepare,

}

self.onmessage = (e) => {
    const fn = handlers[e.data.type];
    if (!fn) throw new Error('Unknown message type: ' + e.data.type);
    fn(e.data);
}

function handleInit(data){
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
    controls.rotateSpeed = .5
 
    const animate = () => {
        animationId = requestAnimationFrame(animate)
        renderer.render(scene,camera)
        controls.update()
    }
    animate()
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
    positionBuffer = new Float32Array(cellcount * 9)
    positionAttribute = new THREE.BufferAttribute(positionBuffer, 3)
    positionAttribute.setUsage(THREE.DynamicDrawUsage)

    scene.remove(mesh)
    const geometry = new THREE.BufferGeometry()
    geometry.setAttribute('position', positionAttribute)
    const meshMaterial = new THREE.MeshBasicMaterial({color: 0x7F7F7F , side: THREE.DoubleSide})
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

