using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Text;
using TerraMachina.Abstractions.Geology;
using TerraMachina.Abstractions.Parameters;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Enums;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;
using TerraMachina.Abstractions.Surface;
using TerraMachina.Abstractions.World;

namespace TerraMachina.WorldGen.Tectonics;


//Build TectonicCellMap
//Spawn seed Plumes
//Build Starting Ridges

//Grow Plates until full
//Asign Base Elevation Noise and Ridge Heights

//Simulate tectonic activity
public class TectonicGenerator
{
    private List<TectonicCell> Cells { get; init; } = new();
    private Dictionary<int, TectonicCell> TectonicCellMap { get; set; } = new();
    private Dictionary<int, MantlePlume> PlumeMap { get; init; } = new();
    private List<MantlePlume> IsolatedPlumes { get; init; } = new();
    private List<MantlePlume> RidgePlumes { get; init; } = new();
    private Dictionary<int, MantleRidge> RidgeMap { get; init; } = new();
    private List<MantleRidge> Ridges { get; init; } = new();
    private List<TectonicPlate> Plates { get; init; } = new();

    private IProgress<WorldGenProgressUpdate> currentProgress;
    private int currentRidgeWidth;
    private int nextPlumeId;
    private int nextRidgeId;
    private int nextPlateId;
    private int currentPlanetRadius;
    private Random? rng;

    public void TectonicPhaseRunner(World world, WorldGenParameters parameters, IProgress<WorldGenProgressUpdate> progress)
    {
        Initialize(parameters, progress);

        progress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Tectonic,
            StageProgress = 0,
        });

        GenerateTectonicCellMap(world.Surface);
        Console.WriteLine("Finished Tectonic CellMap!");
        GenerateStartingRidges(parameters.TectonicParameters.TotalSeedPlumes, parameters.TectonicParameters.MaxLateralRidgeSway, parameters.TectonicParameters.PlumeCenterSize);
    }
    private void Initialize(WorldGenParameters parameters, IProgress<WorldGenProgressUpdate> progress)
    {
        Cells.Clear();
        IsolatedPlumes.Clear();
        RidgePlumes.Clear();
        Ridges.Clear();
        Plates.Clear();
        PlumeMap.Clear();

        nextPlumeId = 0;
        nextRidgeId = 0;
        nextPlateId = 0;

        currentRidgeWidth = parameters.TectonicParameters.RidgeWidth;
        currentPlanetRadius = parameters.TectonicParameters.PlanetRadius;
        rng = new Random(parameters.GeometryParameters.Seed);
        currentProgress = progress;
    }

    private void GenerateTectonicCellMap(CellMap cellMap)
    {
        foreach (Cell cell in cellMap.Cells)
        {
            TectonicCell tCell = new TectonicCell(cell.Geometry);
            Cells.Add(tCell);
            TectonicCellMap.Add(tCell.Geometry.CellId, tCell);
        }
    }
    private void GenerateStartingRidges(int totalSeedPlumes, float maxLateralRidgeSway, float plumeCenterSize)
    {
        HashSet<int> usedCellIndices = new();
        List<MantlePlume> seedPlumes = new();
        List<MantlePlume> branchPlumes = new();

        for (int i = 0; i < totalSeedPlumes; i++)
        {
            int cellIndex = rng.Next(0, Cells.Count());
            while (usedCellIndices.Contains(cellIndex))
            {
                cellIndex = rng.Next(0, Cells.Count());
            }

            MantlePlume x = GeneratePlume(0, Cells[cellIndex].Geometry.Position);
            x.CellIds.Add(Cells[cellIndex].Geometry.CellId);
            x.CenterCellIds.Add(Cells[cellIndex].Geometry.CellId);

            seedPlumes.Add(x);
            RidgePlumes.Add(x);
            usedCellIndices.Add(cellIndex);
        }

        int totalBranchPlumes = rng.Next(2, (totalSeedPlumes / 5));
        for (int i = 0; i < totalBranchPlumes; i++)
        {
            int cellIndex = rng.Next(0, Cells.Count());
            while (usedCellIndices.Contains(cellIndex))
            {
                cellIndex = rng.Next(0, Cells.Count());
            }

            MantlePlume x = GeneratePlume(0, Cells[cellIndex].Geometry.Position);
            x.CellIds.Add(Cells[cellIndex].Geometry.CellId);
            x.CenterCellIds.Add(Cells[cellIndex].Geometry.CellId);

            branchPlumes.Add(x);
            RidgePlumes.Add(x);
            usedCellIndices.Add(cellIndex);
        }

        // UPDATE!
        currentProgress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Tectonic,
            StageProgress = 0,
            CurrentStep = WorldGenStepTypes.GeneratingSeedPlumes,
            Payload = TectonicsPayloadCreator.CreateTectonicPayload(RidgePlumes, TectonicCellMap)
        });
        Console.WriteLine("Finished Creating seed plumes!");

        Dictionary<MantlePlume, MantlePlume> ridgePairs = new();
        List<MantlePlume> visitedPlumes = new();

        MantlePlume currentPlume = seedPlumes[0];
        while (visitedPlumes.Count() < seedPlumes.Count() - 1)
        {
            visitedPlumes.Add(currentPlume);
            float maxDot = 0;
            MantlePlume neighbor = currentPlume;

            foreach (MantlePlume m in seedPlumes)
            {
                if (visitedPlumes.Contains(m)) continue;

                float dot = Vector3.Dot(currentPlume.Center, m.Center);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    neighbor = m;
                }
            }
            ridgePairs.Add(currentPlume, neighbor);
            currentPlume = neighbor;
        }

        visitedPlumes.Clear();
        foreach(MantlePlume plume in branchPlumes)
        {
            
            float maxDot = 0;
            MantlePlume neighbor = plume;

            foreach (MantlePlume m in seedPlumes)
            {
                if (visitedPlumes.Contains(m)) continue;

                float dot = Vector3.Dot(plume.Center, m.Center);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    neighbor = m;
                    
                }
            }
            ridgePairs.Add(plume, neighbor);
            visitedPlumes.Add(neighbor);
        }

        
        foreach(KeyValuePair<MantlePlume, MantlePlume> entry in ridgePairs)
        {
            Ridges.Add(GenerateRidge(entry.Key, entry.Value, maxLateralRidgeSway));
        }

        FindCellsInRidges(plumeCenterSize, Ridges);

        //UPDATE!
        currentProgress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Tectonic,
            StageProgress = 0,
            CurrentStep = WorldGenStepTypes.GeneratingSeedPlumes,
            Payload = TectonicsPayloadCreator.CreateTectonicPayload(Ridges, TectonicCellMap)
        });
        Console.WriteLine("Finished finding cells in the ridges!");
    }
    private MantlePlume GeneratePlume(int size, Vector3 center)
    {
        MantlePlume newPlume = new MantlePlume(nextPlumeId, center, size);
        nextPlumeId++;
        
        if(size > 0)
        {
            //find cells in plume area
            //add to cellid list
            //if within center, add to center list
        }

        PlumeMap.Add(newPlume.PlumeID, newPlume);
        return newPlume;
    }
    private MantleRidge GenerateRidge(MantlePlume plumeA,  MantlePlume plumeB, float maxLateralRidgeSway)
    {
        //create a random number of points along the line between the 2 seed plumes
        //disperse them evenly along the line.
        int segmentCount = rng.Next(10, 20);
        float spaceBetweenPoints = 1f / (segmentCount + 1);
        List<Vector3> points = new();
        points.Add(plumeA.Center);

        for (int i = 1; i < segmentCount; i++)
        {
            Vector3 point = Vector3.Normalize(Vector3.Lerp(plumeA.Center, plumeB.Center, spaceBetweenPoints * (i + 1)));
            points.Add(point);
        }

        points.Add(plumeB.Center);

        //Choose a random point to be our furthest point on the bend
        //Move it laterally a random amount within "reason"
        int index = rng.Next(3, segmentCount - 3);
        Vector3 normalVector = Vector3.Normalize(Vector3.Cross(plumeA.Center, plumeB.Center));
        Vector3 perpendicularVector = Vector3.Normalize(Vector3.Cross(normalVector, points[index]));
        float maxLateral = (currentRidgeWidth * maxLateralRidgeSway) / currentPlanetRadius;
        float lateralAngle = (rng.NextSingle() * 2f - 1f) * maxLateral;
        points[index] = Vector3.Normalize(points[index] + perpendicularVector * lateralAngle);

        //move all the other points onto a line between the plumes and the furthest point
        float inwardLateralStep = lateralAngle / (index -1);
        float outwardLateralStep = lateralAngle / (segmentCount - (index + 2));
        
        for (int i = 1; i < index; i++)
        {
            perpendicularVector = Vector3.Normalize(Vector3.Cross(normalVector, points[i]));
            points[i] = Vector3.Normalize(points[i] + perpendicularVector * (inwardLateralStep * i));
        }

        for (int i = points.Count() - 2; i > index; i--)
        {
            perpendicularVector = Vector3.Normalize(Vector3.Cross(normalVector, points[i]));
            points[i] = Vector3.Normalize(points[i] + perpendicularVector * (outwardLateralStep * ((points.Count() - 1) - i)));
        }

        //build segments
        //each segment is built of two endpoints that are a random distance between each of the previous made points. 
        List<(Vector3 start, Vector3 end)> segments = new();
        Dictionary<(Vector3, Vector3), Vector3> cachedMidpoints = new();

        for(int i = 0; i < points.Count(); i++)
        {
            Vector3 startPoint;
            Vector3 endPoint;
            if(i < points.Count() - 1)
            {
                if(i == 0)
                {
                    startPoint = points[i];
                }
                else
                {
                    startPoint = cachedMidpoints[(points[i - 1], points[i])];
                }
                

                float rngDistance = rng.NextSingle() * 0.6f + 0.2f;
                Vector3 point = Vector3.Normalize(Vector3.Lerp(points[i], points[i + 1], rngDistance));
                Vector3 segmentDirection = Vector3.Normalize(Vector3.Cross(normalVector, points[i]));

                float adjustment = Vector3.Dot(point - points[i], segmentDirection);

                endPoint = Vector3.Normalize(points[i] + segmentDirection * adjustment);

                cachedMidpoints.Add((points[i], points[i + 1]), endPoint);
            }
            else
            {
                startPoint = cachedMidpoints[(points[i - 1], points[i])];
                endPoint = points[i];
            }
            segments.Add((startPoint, endPoint));
        }

        //rough up the eveness of the segment steps to produce a more natural result
        List<RidgeSegment> segmentsFinal = new();
        Vector3 SegmentNormalVector = Vector3.Normalize(Vector3.Cross(segments[0].start, segments[0].end));
        segmentsFinal.Add(new RidgeSegment(segments[0].start, segments[0].end, SegmentNormalVector));

        for (int i = 1; i < index; i++)
        {
            SegmentNormalVector = Vector3.Normalize(Vector3.Cross(segments[i].start, segments[i].end));
            float roughMoveAdjustment = (rng.NextSingle() * 2f - 1f) * inwardLateralStep;

            Vector3 perpendicularVectorStart = Vector3.Normalize(Vector3.Cross(SegmentNormalVector, segments[i].start));
            Vector3 perpendicularVectorEnd = Vector3.Normalize(Vector3.Cross(SegmentNormalVector, segments[i].end));

            Vector3 adjustedStart = Vector3.Normalize(segments[i].start + perpendicularVectorStart * roughMoveAdjustment);
            Vector3 adjustedEnd = Vector3.Normalize(segments[i].end + perpendicularVectorEnd * roughMoveAdjustment);

            Vector3 newSegmentNormalVector = Vector3.Normalize(Vector3.Cross(adjustedStart, adjustedEnd));
            segmentsFinal.Add(new RidgeSegment(adjustedStart,adjustedEnd,newSegmentNormalVector));
        }

        SegmentNormalVector = Vector3.Normalize(Vector3.Cross(segments[index].start, segments[index].end));
        segmentsFinal.Add(new RidgeSegment(segments[index].start, segments[index].end, SegmentNormalVector));

        for (int i = segments.Count() - 2; i > index; i--)
        {
            SegmentNormalVector = Vector3.Normalize(Vector3.Cross(segments[i].start, segments[i].end));
            float roughMoveAdjustment = (rng.NextSingle() * 2f - 1f) * outwardLateralStep;

            Vector3 perpendicularVectorStart = Vector3.Normalize(Vector3.Cross(SegmentNormalVector, segments[i].start));
            Vector3 perpendicularVectorEnd = Vector3.Normalize(Vector3.Cross(SegmentNormalVector, segments[i].end));

            Vector3 adjustedStart = Vector3.Normalize(segments[i].start + perpendicularVectorStart * roughMoveAdjustment);
            Vector3 adjustedEnd = Vector3.Normalize(segments[i].end + perpendicularVectorEnd * roughMoveAdjustment);

            Vector3 newSegmentNormalVector = Vector3.Normalize(Vector3.Cross(adjustedStart, adjustedEnd));
            segmentsFinal.Add(new RidgeSegment(adjustedStart, adjustedEnd, newSegmentNormalVector));
        }

        SegmentNormalVector = Vector3.Normalize(Vector3.Cross(segments[segments.Count() - 1].start, segments[segments.Count() - 1].end));
        segmentsFinal.Add(new RidgeSegment(segments[segments.Count() - 1].start, segments[segments.Count() - 1].end, SegmentNormalVector));

        MantleRidge newRidge = new MantleRidge(nextRidgeId, plumeA.PlumeID, plumeB.PlumeID, lateralAngle, normalVector, segmentsFinal);
        nextRidgeId++;

        RidgeMap.Add(newRidge.RidgeId, newRidge);

        return newRidge;
    }
    
    private void FindCellsInRidges(float centerSize, List<MantleRidge> currentRidges)
    {
        foreach(TectonicCell cell in Cells)
        {
            foreach(MantleRidge ridge in currentRidges)
            {
                float thresholdSin = MathF.Sin((currentRidgeWidth / currentPlanetRadius) + ridge.LateralAngle);
                float distanceToArc = MathF.Abs(Vector3.Dot(ridge.Normal, cell.Geometry.Position));
                bool withinWidth = distanceToArc <= thresholdSin;

                Vector3 ridgeDirection = Vector3.Normalize(Vector3.Cross(ridge.Normal, PlumeMap[ridge.PlumeAId].Center));
                float cellT = Vector3.Dot(cell.Geometry.Position - PlumeMap[ridge.PlumeAId].Center, ridgeDirection);
                float endBT = Vector3.Dot(PlumeMap[ridge.PlumeBId].Center - PlumeMap[ridge.PlumeAId].Center, ridgeDirection);
                bool betweenEndpoints = cellT >= 0 && cellT <= endBT;

                if (withinWidth && betweenEndpoints)
                {
                    foreach (RidgeSegment segment in ridge.Segments)
                    {
                        float segmentthresholdSin = MathF.Sin(currentRidgeWidth / currentPlanetRadius);
                        float distanceToSegmentArc = MathF.Abs(Vector3.Dot(segment.Normal, cell.Geometry.Position));
                        bool withinSegmentWidth = distanceToSegmentArc <= segmentthresholdSin;

                        Vector3 segmentDirection = Vector3.Normalize(Vector3.Cross(segment.Normal, segment.StartPoint));
                        float cellSegmentT = Vector3.Dot(cell.Geometry.Position - segment.StartPoint, segmentDirection);
                        float segmentEndBT = Vector3.Dot(segment.EndPoint - segment.StartPoint, segmentDirection);
                        bool betweenSegmentEndpoints = cellSegmentT >= 0 && cellSegmentT <= segmentEndBT;

                        if (withinSegmentWidth & betweenSegmentEndpoints)
                        {
                            float centerthresholdSin = MathF.Sin((currentRidgeWidth / currentPlanetRadius) * centerSize);
                            if(distanceToSegmentArc <= centerthresholdSin)
                            {
                                ridge.CenterCellIds.Add(cell.Geometry.CellId);
                            }

                            ridge.ContainedCellIds.Add(cell.Geometry.CellId);
                            break; //can only be in one segment
                        }
                        
                    }//segment Loop
                    break; //can only be in one ridge
                }
            }//ridge loop
        }//cell loop
    }
    private void GeneratePlates()
    {

    }
    private void SimulateTectonicActivity()
    {

    }

}
