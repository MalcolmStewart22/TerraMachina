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
    private List<(int a, int b, int c)> Triangles = new();
    private Dictionary<(int, int), int> MidpointLookup = new();

    public void GenerateSphere(CellMap cellMap, IProgress<WorldGenProgressUpdate> progress, int subdivisionLevel)
    {
        Vertices.Clear();
        Triangles.Clear();
        MidpointLookup.Clear();

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
        SubdivideIcosahedron(subdivisionLevel);
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
            StageProgress = .99f,
        });
        BuildCells(cellMap);
        progress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Geometry,
            StageProgress = 1f,
            Payload = new GeometryProgressPayload(cellMap)
        });
    }
    private void GenerateIcosahedron()
    {
        //create vertices
        // YZ Plane
        Vertices.Add(new Vector3(0, 1, (float)GoldRatio));   // 0
        Vertices.Add(new Vector3(0, -1, (float)GoldRatio));  // 1
        Vertices.Add(new Vector3(0, 1, -(float)GoldRatio));  // 2
        Vertices.Add(new Vector3(0, -1, -(float)GoldRatio)); // 3
        // XZ Plane
        Vertices.Add(new Vector3((float)GoldRatio, 0, 1));   // 4
        Vertices.Add(new Vector3((float)GoldRatio, 0, -1));  // 5
        Vertices.Add(new Vector3(-(float)GoldRatio, 0, 1));  // 6
        Vertices.Add(new Vector3(-(float)GoldRatio, 0, -1)); // 7
        // XY Plane
        Vertices.Add(new Vector3(1, (float)GoldRatio, 0));   // 8
        Vertices.Add(new Vector3(-1, (float)GoldRatio, 0));  // 9
        Vertices.Add(new Vector3(1, -(float)GoldRatio, 0));  // 10
        Vertices.Add(new Vector3(-1, -(float)GoldRatio, 0)); // 11

        //create Triangles
        // Top cap
        Triangles.Add((0, 8, 4));
        Triangles.Add((0, 4, 1));
        Triangles.Add((0, 1, 6));
        Triangles.Add((0, 6, 9));
        Triangles.Add((0, 9, 8));
        // Upper middle
        Triangles.Add((8, 5, 4));
        Triangles.Add((4, 10, 1));
        Triangles.Add((1, 11, 6));
        Triangles.Add((6, 7, 9));
        Triangles.Add((9, 2, 8));
        // Lower middle
        Triangles.Add((5, 8, 2));
        Triangles.Add((10, 5, 3));
        Triangles.Add((11, 10, 7));
        Triangles.Add((7, 11, 2));
        Triangles.Add((2, 9, 7));
        // Bottom cap
        Triangles.Add((3, 5, 10));
        Triangles.Add((3, 10, 11));
        Triangles.Add((3, 11, 7));
        Triangles.Add((3, 7, 2));
        Triangles.Add((3, 2, 5));
    }
    private void SubdivideIcosahedron(int subdivisionLevels)
    {
        List<(int a, int b, int c)> oldTriangles = new(Triangles);
        Triangles.Clear();

        foreach (var tri in oldTriangles)
        {
            int vertexA = tri.a;
            int vertexB = tri.b;
            int vertexC = tri.c;

            int vertexD = FindOrCreateMidPoint(vertexA, vertexB);
            int vertexE = FindOrCreateMidPoint(vertexA, vertexC);
            int vertexF = FindOrCreateMidPoint(vertexB, vertexC);

            Triangles.Add((vertexA, vertexD, vertexE));
            Triangles.Add((vertexB, vertexD, vertexF));
            Triangles.Add((vertexC, vertexE, vertexF));
            Triangles.Add((vertexD, vertexE, vertexF));
        }

        if ( subdivisionLevels - 1 > 0)
        {
            SubdivideIcosahedron(subdivisionLevels - 1);
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
    private void BuildCells(CellMap cellMap)
    {
        Dictionary<(int, int), Cell> edgeLookup = new();

        for (int i = 0; i < Triangles.Count; i++)
        {
            Vector3 pos = Vector3.Normalize((Vertices[Triangles[i].a] + Vertices[Triangles[i].b] + Vertices[Triangles[i].c]) / 3);
            Vector3[] verts = new Vector3[3];
            verts[0] = Vertices[Triangles[i].a];
            verts[1] = Vertices[Triangles[i].b];
            verts[2] = Vertices[Triangles[i].c];
            CellGeometry geo = new(i+1,pos,verts);
            Cell newCell = new(geo);
            cellMap.Cells.Add(newCell);
            cellMap.CellById.Add(i+1, newCell);
            FindNeighbors(Triangles[i].a, Triangles[i].b, newCell, edgeLookup);
            FindNeighbors(Triangles[i].a, Triangles[i].c, newCell, edgeLookup);
            FindNeighbors(Triangles[i].b, Triangles[i].c, newCell, edgeLookup);
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

        if (lookup.TryGetValue(key, out Cell index))
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
