using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Enums;
using TerraMachina.Abstractions.Surface;


namespace TerraMachina.WorldGen.Geometry;

public class IcoSphereGenerator
{
    private static readonly double GoldRatio = (1 + Math.Sqrt(5)) / 2;
    private List<Vector3> Vertices = new();
    private List<Triangle> Triangles = new();
    private Dictionary<(int, int), int> MidpointLookup = new();
    private int progressCount = 1;
    int nextID;

    public async void GenerateSphere(CellMap cellMap, IProgress<WorldGenProgressUpdate> progress, int subdivisionLevel)
    {
        Vertices.Clear();
        Triangles.Clear();
        MidpointLookup.Clear();
        nextID = 1;

        progress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Geometry,
            CurrentStep = WorldGenStepTypes.GeneratingIcosahedron,
            StageProgress = 0,
        });
        
        GenerateIcosahedron();
        progress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Geometry,
            CurrentStep = WorldGenStepTypes.Subdividing,
            StageProgress = .33f,
        });
        
        SubdivideIcosahedron(subdivisionLevel, Triangles);
        progress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Geometry,
            CurrentStep = WorldGenStepTypes.ProjectingToSphere,
            StageProgress = .66f,
        });
        
        ProjectVerticesToSphere();
        progress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Geometry,
            CurrentStep = WorldGenStepTypes.BuildingCells,
            StageProgress = .90f,
        });
        
        Dictionary<(int, int), Cell> edgeLookup = new();
        
        SpatialNode tempRoot = new();
        await BuildCellMap(cellMap, progress, Triangles, edgeLookup, tempRoot);
        
        if(cellMap.WorldTree.Count < 0)
        {
            progress.Report(new WorldGenProgressUpdate
            {
                CurrentStage = WorldGenStageTypes.Geometry,
                StageProgress = 1f,
                Payload = new GeometryProgressPayload(cellMap)
            });
        }
        else
        {
            progress.Report(new WorldGenProgressUpdate
            {
                CurrentStage = WorldGenStageTypes.Geometry,
                StageProgress = 1f,
            });
        }
    }
    private void GenerateIcosahedron()
    {
        //create vertices
        // XY Plane
        Vertices.Add(new Vector3(-1, (float)GoldRatio, 0));
        Vertices.Add(new Vector3(1, (float)GoldRatio, 0));
        Vertices.Add(new Vector3(-1, -(float)GoldRatio, 0));
        Vertices.Add(new Vector3(1, -(float)GoldRatio, 0));
        // YZ Plane
        Vertices.Add(new Vector3(0, -1, (float)GoldRatio));
        Vertices.Add(new Vector3(0, 1, (float)GoldRatio));
        Vertices.Add(new Vector3(0, -1, -(float)GoldRatio));
        Vertices.Add(new Vector3(0, 1, -(float)GoldRatio));
        // XZ Plane
        Vertices.Add(new Vector3((float)GoldRatio, 0, -1));
        Vertices.Add(new Vector3((float)GoldRatio, 0, 1));
        Vertices.Add(new Vector3(-(float)GoldRatio, 0, -1));
        Vertices.Add(new Vector3(-(float)GoldRatio, 0, 1));  
        

        //create Triangles
        // Top cap
        Triangles.Add(new Triangle(0, 11, 5, 0));
        Triangles.Add(new Triangle(0, 5, 1, 0));
        Triangles.Add(new Triangle(0, 1, 7, 0));
        Triangles.Add(new Triangle(0, 7, 10, 0));
        Triangles.Add(new Triangle(0, 10, 11, 0));
        // Upper middle
        Triangles.Add(new Triangle(1, 5, 9, 0));
        Triangles.Add(new Triangle(5, 11, 4, 0));
        Triangles.Add(new Triangle(11, 10, 2, 0));
        Triangles.Add(new Triangle(10, 7, 6, 0));
        Triangles.Add(new Triangle(7, 1, 8, 0));
        // Bottom cap
        Triangles.Add(new Triangle(3, 9, 4, 0));
        Triangles.Add(new Triangle(3, 4, 2, 0));
        Triangles.Add(new Triangle(3, 2, 6, 0));
        Triangles.Add(new Triangle(3, 6, 8, 0));
        Triangles.Add(new Triangle(3, 8, 9, 0));
        // Lower middle
        Triangles.Add(new Triangle(4, 9, 5, 0));
        Triangles.Add(new Triangle(2, 4, 11, 0));
        Triangles.Add(new Triangle(6, 2, 10, 0));
        Triangles.Add(new Triangle(8, 6, 7, 0));
        Triangles.Add(new Triangle(9, 8, 1, 0));
    }
    private void SubdivideIcosahedron(int subdivisionLevels, List<Triangle> currentTriangles)
    {
        List<Triangle> currentLeaves = new();
        foreach (var tri in currentTriangles)
        {
            int a = FindOrCreateMidPoint(tri.a, tri.b);
            int b = FindOrCreateMidPoint(tri.b, tri.c);
            int c = FindOrCreateMidPoint(tri.c, tri.a);

            Triangle w = new Triangle(tri.a, a, c, tri.Level + 1);
            Triangle x = new Triangle(tri.b, b, a, tri.Level + 1);
            Triangle y = new Triangle(tri.c, c, b, tri.Level + 1);
            Triangle z = new Triangle(a, b, c, tri.Level + 1);

            tri.Children.Add(w);
            currentLeaves.Add(w);
            tri.Children.Add(x);
            currentLeaves.Add(x);
            tri.Children.Add(y);
            currentLeaves.Add(y);
            tri.Children.Add(z);
            currentLeaves.Add(z);
        }

        if ( subdivisionLevels - 1 > 0)
        {
            SubdivideIcosahedron(subdivisionLevels - 1, currentLeaves);
        }
    }
    private void ProjectVerticesToSphere()
    {
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vector3 newVtx = Vector3.Normalize(Vertices[i]);
            Vertices[i] = newVtx;
        }
    }
    private async Task BuildCellMap(CellMap cellMap, IProgress<WorldGenProgressUpdate> progress, List<Triangle> currentTriangles, Dictionary<(int, int), Cell> edges, SpatialNode currentNode)
    {
        for (int i = 0; i < currentTriangles.Count; i++)
        {
            
            if (currentTriangles[i].Children.Count > 0)
            {
                if (currentTriangles[i].Level >= 2 && currentTriangles[i].Level % 2 == 0)
                {
                    SpatialNode x = new();
                    x.NodeID = nextID;
                    nextID++;
                    x.Vertices[0] = Vertices[currentTriangles[i].a];
                    x.Vertices[1] = Vertices[currentTriangles[i].b];
                    x.Vertices[2] = Vertices[currentTriangles[i].c];
                    currentNode.Children.Add(x);

                    await BuildCellMap(cellMap, progress, currentTriangles[i].Children, edges, x);

                    if(currentTriangles[i].Level == 2)
                    {
                        cellMap.WorldTree.Add(x);
                        
                        progress.Report(new WorldGenProgressUpdate
                        {
                            CurrentStage = WorldGenStageTypes.Geometry,
                            StageProgress = progressCount,
                            CurrentStep = WorldGenStepTypes.BuildingCells,
                            Payload = new GeometryProgressPayload(x)
                        });
                        await Task.Delay(100);
                        Console.WriteLine(progressCount);
                        progressCount++;
                    }

                }
                else
                {
                    await BuildCellMap(cellMap, progress, currentTriangles[i].Children, edges, currentNode);
                }
            }
            else
            {
                Vector3 pos = Vector3.Normalize((Vertices[currentTriangles[i].a] + Vertices[currentTriangles[i].b] + Vertices[currentTriangles[i].c]) / 3);
                Vector3[] verts = new Vector3[3];
                verts[0] = Vertices[currentTriangles[i].a];
                verts[1] = Vertices[currentTriangles[i].b];
                verts[2] = Vertices[currentTriangles[i].c];
                CellGeometry geo = new(nextID, pos, verts);
                Cell newCell = new(geo);
                cellMap.Cells.Add(newCell);
                cellMap.CellById.Add(nextID, newCell);
                nextID++;
                FindNeighbors(currentTriangles[i].a, currentTriangles[i].b, newCell, edges);
                FindNeighbors(currentTriangles[i].a, currentTriangles[i].c, newCell, edges);
                FindNeighbors(currentTriangles[i].b, currentTriangles[i].c, newCell, edges);
                
                currentNode.Children.Add(new SpatialLeaf(newCell));
            }
        }
    }
    private int FindOrCreateMidPoint(int a, int b)
    {
        var key = a < b ? (a, b) : (b, a);

        if (MidpointLookup.TryGetValue(key, out int index))
        {
            return index;
        }

        Vector3 midpoint = (Vertices[a] + Vertices[b]) / 2;
        Vertices.Add(midpoint);
        int newIndex = Vertices.Count - 1;
        MidpointLookup.Add(key, newIndex);
        return newIndex;
    }

    private void FindNeighbors(int a, int b, Cell c, Dictionary<(int, int), Cell> lookup)
    {
        var key = a < b ? (a, b) : (b, a);

        if (lookup.TryGetValue(key, out Cell? index))
        {
            for (int i = 0; i < 3; i++)
            {
                if (c.Geometry.NeighborsById[i] == 0)
                {
                    c.Geometry.NeighborsById[i] = index.Geometry.CellId;
                    break;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (index.Geometry.NeighborsById[i] == 0)
                {
                    index.Geometry.NeighborsById[i] = c.Geometry.CellId;
                    break;
                }
            }
            return;
        }
        lookup.Add(key, c);
    }

}
