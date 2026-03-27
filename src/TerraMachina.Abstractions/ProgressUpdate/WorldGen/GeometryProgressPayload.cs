using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

public class GeometryProgressPayload : IWorldGenPayload
{
    public List<CellGeometryMinimum> Sphere { get; init; } = new();

    public GeometryProgressPayload(CellMap cellMap)
    {
        foreach (Cell c in cellMap.Cells)
        {
            Sphere.Add(new CellGeometryMinimum(c.Geometry));
        }
        Console.WriteLine("Cells Generated: " + Sphere.Count);
    }
    public GeometryProgressPayload(SpatialNode node)
    {
        Sphere = new List<CellGeometryMinimum>(CollectGeometry(node));
        Console.WriteLine("Cells Generated: " + Sphere.Count);
    }

    private List<CellGeometryMinimum> CollectGeometry(SpatialNode root)
    {
        List<CellGeometryMinimum> result = new();
        foreach(var node in root.Children)
        {
            if(node.Children.Count > 0)
            {
                result.AddRange(CollectGeometry(node));
            }
            else
            {
                SpatialLeaf leaf = (SpatialLeaf)node;
                result.Add(new CellGeometryMinimum(leaf.Cell.Geometry));
            }
        }
        return result;
    }
}
