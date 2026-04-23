import { Vector3 } from "three";
import { createCellMap } from "./world";
import { createSpatialNode } from "./world";
import { createCellGeometry } from "./world";
import { createCell } from "./world";

const goldRatio = (1 + Math.sqrt(5)) / 2

export function generateSphere(){
    const cellMap = createCellMap()
    const vertices = []
    const triangles = []
    const midpointLookup = new Map()
    let nextCellId = 1, nextNodeId = 1


    generateIcosahedron()
        
    subdivideIcosahedron(5, triangles)
    projectVerticesToSphere()
        
    const edgeLookup = new Map()    
    const tempRoot = createSpatialNode()
    buildCellMap(triangles, tempRoot)

    return cellMap

    function generateIcosahedron()
    {
        //create vertices
        // XY Plane
        vertices.push(new Vector3(-1, goldRatio, 0))
        vertices.push(new Vector3(1, goldRatio, 0))
        vertices.push(new Vector3(-1, -goldRatio, 0))
        vertices.push(new Vector3(1, -goldRatio, 0))
        // YZ Plane
        vertices.push(new Vector3(0, -1, goldRatio))
        vertices.push(new Vector3(0, 1, goldRatio))
        vertices.push(new Vector3(0, -1, -goldRatio))
        vertices.push(new Vector3(0, 1, -goldRatio))
        // XZ Plane
        vertices.push(new Vector3(goldRatio, 0, -1))
        vertices.push(new Vector3(goldRatio, 0, 1))
        vertices.push(new Vector3(-goldRatio, 0, -1))
        vertices.push(new Vector3(-goldRatio, 0, 1))  
        

        //create Triangles
        // Top cap
        triangles.push(createTriangle(0, 11, 5, 0))
        triangles.push(createTriangle(0, 5, 1, 0))
        triangles.push(createTriangle(0, 1, 7, 0))
        triangles.push(createTriangle(0, 7, 10, 0))
        triangles.push(createTriangle(0, 10, 11, 0))
        // Upper middle
        triangles.push(createTriangle(1, 5, 9, 0))
        triangles.push(createTriangle(5, 11, 4, 0))
        triangles.push(createTriangle(11, 10, 2, 0))
        triangles.push(createTriangle(10, 7, 6, 0))
        triangles.push(createTriangle(7, 1, 8, 0))
        // Bottom cap
        triangles.push(createTriangle(3, 9, 4, 0))
        triangles.push(createTriangle(3, 4, 2, 0))
        triangles.push(createTriangle(3, 2, 6, 0))
        triangles.push(createTriangle(3, 6, 8, 0))
        triangles.push(createTriangle(3, 8, 9, 0))
        // Lower middle
        triangles.push(createTriangle(4, 9, 5, 0))
        triangles.push(createTriangle(2, 4, 11, 0))
        triangles.push(createTriangle(6, 2, 10, 0))
        triangles.push(createTriangle(8, 6, 7, 0))
        triangles.push(createTriangle(9, 8, 1, 0))
    }

    function subdivideIcosahedron(subdivisionLevels, currentTriangles)
    {
        const currentLeaves = []
        for (const tri of currentTriangles)
        {
            let a = findOrCreateMidPoint(tri.a, tri.b)
            let b = findOrCreateMidPoint(tri.b, tri.c)
            let c = findOrCreateMidPoint(tri.c, tri.a)

            let triW = createTriangle(tri.a, a, c, tri.level + 1)
            let triX = createTriangle(tri.b, b, a, tri.level + 1)
            let triY = createTriangle(tri.c, c, b, tri.level + 1)
            let triZ = createTriangle(a, b, c, tri.level + 1)

            tri.children.push(triW)
            currentLeaves.push(triW)
            tri.children.push(triX)
            currentLeaves.push(triX)
            tri.children.push(triY)
            currentLeaves.push(triY)
            tri.children.push(triZ)
            currentLeaves.push(triZ)
        }

        if ( subdivisionLevels - 1 > 0)
        {
            subdivideIcosahedron(subdivisionLevels - 1, currentLeaves)
        }
    }

    function projectVerticesToSphere()
    {
        for (let i = 0; i < vertices.length; i++)
        {
            vertices[i].normalize()
        }
    }

    function buildCellMap(currentTriangles, currentNode){
        for (let i = 0; i < currentTriangles.length; i++)
        {
            
            if (currentTriangles[i].children.length > 0)
            {
                if (currentTriangles[i].level >= 2 && currentTriangles[i].level % 2 == 0)
                {
                    const x = createSpatialNode()
                    x.nodeID = nextNodeId
                    nextNodeId++
                    x.vertices.push(vertices[currentTriangles[i].a])
                    x.vertices.push(vertices[currentTriangles[i].b])
                    x.vertices.push(vertices[currentTriangles[i].c])
                    currentNode.children.push(x)

                    buildCellMap(currentTriangles[i].children, x)

                    if(currentTriangles[i].level == 2)
                    {
                        cellMap.worldTree.push(x)
                    }

                }
                else
                {
                    buildCellMap(currentTriangles[i].children, currentNode)
                }
            }
            else
            {
               const pos = vertices[currentTriangles[i].a].clone()
                    .add(vertices[currentTriangles[i].b])
                    .add(vertices[currentTriangles[i].c])
                    .divideScalar(3)
                    .normalize()

                const verts = []
                verts.push(vertices[currentTriangles[i].a])
                verts.push(vertices[currentTriangles[i].b])
                verts.push(vertices[currentTriangles[i].c])
                const geo = createCellGeometry(nextCellId, pos, verts)
                const newCell = createCell(geo)
                cellMap.cells.push(newCell)
                cellMap.cellById.set(nextCellId, newCell)
                nextCellId++
                findNeighbors(currentTriangles[i].a, currentTriangles[i].b, newCell)
                findNeighbors(currentTriangles[i].a, currentTriangles[i].c, newCell)
                findNeighbors(currentTriangles[i].b, currentTriangles[i].c, newCell)

                currentNode.children.push(newCell)
            }
        }
    }
    
    function findOrCreateMidPoint(a, b){
        const key = a < b ?  `${a},${b}` : `${b},${a}`

        if (midpointLookup.has(key)) {
            return midpointLookup.get(key)
        }

        const midpoint = vertices[a].clone().add(vertices[b]).multiplyScalar(0.5)
        vertices.push(midpoint)
        const newIndex = vertices.length - 1
        midpointLookup.set(key, newIndex)
        return newIndex
    } 

    function findNeighbors(a, b, cell){
        const key = a < b ?  `${a},${b}` : `${b},${a}`

        if (edgeLookup.has(key)){
            const index = edgeLookup.get(key)

            for (let i = 0; i < 3; i++){
                if (cell.geometry.neighborsById[i] === 0)
                {
                    cell.geometry.neighborsById[i] = index.geometry.cellId
                    break
                }
            }
            for (let i = 0; i < 3; i++){
                if (index.geometry.neighborsById[i] === 0)
                {
                    index.geometry.neighborsById[i] = cell.geometry.cellId
                    break
                }
            }
            return
        }
        edgeLookup.set(key, cell)
    }
}

function createTriangle(a, b, c, level){
    return{
        a, b, c, level,
        children: []
    }
}
