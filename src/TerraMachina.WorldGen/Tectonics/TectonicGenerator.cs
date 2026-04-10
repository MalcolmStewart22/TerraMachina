using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
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

// note to self: expected ranges are -11km to +12km
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
    private WorldGenParameters currentParameters;
    private int nextPlumeId;
    private int nextRidgeId;
    private int nextPlateId;
    private float normalizedRidgeWidth;
    private Random rng;

    public void TectonicPhaseRunner(World world)
    {

        currentProgress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Tectonic,
            StageProgress = 0,
        });

        GenerateTectonicCellMap(world.Surface);
        Console.WriteLine("WORLDGEN: Finished Tectonic CellMap!");
        try
        {
        GenerateStartingRidges(
            currentParameters.TectonicParameters.TotalSeedPlumes,
            currentParameters.TectonicParameters.MaxLateralRidgeSway,
            currentParameters.TectonicParameters.PlumeCenterSize
            );
        } catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
            Console.WriteLine($"Stack trace: {e.StackTrace}");
        }
        
    }
    public TectonicGenerator(WorldGenParameters parameters, IProgress<WorldGenProgressUpdate> progress)
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

        //Ridge Width is input by users as the full span requiring use of planet diameter to give correct values
        normalizedRidgeWidth = (float)parameters.TectonicParameters.RidgeWidth / (2f * parameters.TectonicParameters.PlanetRadius);
        rng = new Random(parameters.GeometryParameters.Seed);
        currentProgress = progress;
        currentParameters = parameters;
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
    //TODO: Potentially make branches longer
    private void GenerateStartingRidges(int totalSeedPlumes, float maxLateralRidgeSway, float plumeCenterSize)
    {
        
        List<MantlePlume> seedPlumes = new();
        List<MantlePlume> branchPlumes = new();
        Dictionary<MantlePlume, MantlePlume> ridgePairs = new();
        #region Seed Plumes

        //Plume sizes are input by users as full spans(diameter) requiring use of planet diameter to give correct values 
        float minDotProduct = MathF.Cos((currentParameters.TectonicParameters.MaxPlumeSize * currentParameters.TectonicParameters.PlumeConnectivityDistance) / currentParameters.TectonicParameters.PlanetRadius);
        float maxDotProduct = MathF.Cos((currentParameters.TectonicParameters.MinPlumeSize * currentParameters.TectonicParameters.PlumeConnectivityDistance) / currentParameters.TectonicParameters.PlanetRadius);

        int cellIndex = rng.Next(0, Cells.Count());

        MantlePlume newPlume = GeneratePlume(currentParameters.TectonicParameters.RidgeWidth, Cells[cellIndex].Geometry.Position);
        seedPlumes.Add(newPlume);
        RidgePlumes.Add(newPlume);

        MantlePlume currentPlume = newPlume;

        for (int i = 1; i < totalSeedPlumes; i++)
        {
            List<TectonicCell> approvedOptions = new();
            foreach (var cell in Cells)
            {
                bool acceptable = true;
                float dot = Vector3.Dot(cell.Geometry.Position, currentPlume.Center);
                if (dot <= maxDotProduct && dot >= minDotProduct)
                {
                    foreach (var plume in seedPlumes)
                    {
                        dot = Vector3.Dot(cell.Geometry.Position, plume.Center);
                        if(dot <= maxDotProduct) continue;
                        else
                        {
                            acceptable = false;
                            break;
                        }
                    }
                }
                else 
                { 
                    acceptable = false; 
                }
                
                if(acceptable) approvedOptions.Add(cell);
            }
            if (approvedOptions.Count > 0)
            {
                cellIndex = rng.Next(0, approvedOptions.Count());

                newPlume = GeneratePlume(currentParameters.TectonicParameters.RidgeWidth, approvedOptions[cellIndex].Geometry.Position);
                

                seedPlumes.Add(newPlume);
                RidgePlumes.Add(newPlume);

                ridgePairs.Add(currentPlume, newPlume);
                currentPlume = newPlume;
            }
            else break;
        }

        int totalBranchPlumes = rng.Next(2, (totalSeedPlumes / 5));
        List<MantlePlume> unbranchedPlumes = new(seedPlumes);

        for (int i = 0; i < totalBranchPlumes; i++)
        {
            

            List<(TectonicCell cell, MantlePlume plume)> potentialOptions = new();
            foreach (var cell in Cells)
            {
                MantlePlume closestPlume = unbranchedPlumes[0];
                float currentMaxDot = 0;
                bool acceptable = false;

                foreach (var plume in unbranchedPlumes)
                {
                    float dot = Vector3.Dot(cell.Geometry.Position, plume.Center);
                    if (dot >= minDotProduct)
                    {
                        acceptable = true;
                        if(dot > currentMaxDot)
                        {
                            closestPlume = plume;
                            currentMaxDot = dot;
                        }
                    }
                }

                if (acceptable) potentialOptions.Add((cell, closestPlume));
            }

            List<(TectonicCell cell, MantlePlume plume)> approvedOptions = new();
            foreach (var item in potentialOptions)
            {
                bool acceptable = true;

                foreach (var plume in seedPlumes)
                {
                    float dot = Vector3.Dot(item.cell.Geometry.Position, plume.Center);
                    if (dot <= maxDotProduct) continue;
                    else
                    {
                        acceptable = false;
                        break;
                    }
                }

                if (acceptable) approvedOptions.Add((item));
            }

            if (approvedOptions.Count > 0)
            {

                cellIndex = rng.Next(0, approvedOptions.Count());

                newPlume = GeneratePlume(currentParameters.TectonicParameters.RidgeWidth, approvedOptions[cellIndex].cell.Geometry.Position);

                seedPlumes.Add(newPlume);
                RidgePlumes.Add(newPlume);

                ridgePairs.Add(newPlume, approvedOptions[cellIndex].plume);
                unbranchedPlumes.Remove(approvedOptions[cellIndex].plume);
            }
            else break;
        }

        // UPDATE!
        currentProgress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Tectonic,
            StageProgress = 0,
            CurrentStep = WorldGenStepTypes.GeneratingSeedPlumes,
            Payload = TectonicsPayloadCreator.CreateTectonicPayload(RidgePlumes, TectonicCellMap),
            Message = $"Requested Seed Plumes: {currentParameters.TectonicParameters.TotalSeedPlumes} Created Seed Plumes: {seedPlumes.Count()}"
        });
        Console.WriteLine("WORLDGEN: Finished Creating seed plumes!");

        Console.WriteLine("==== PLUME DATA ====");
        foreach (var plume in RidgePlumes)
        {
            Console.WriteLine($"Plume ID: {plume.PlumeID} CenterCellID: {plume.CenterCellIds[0]}");
        }
        #endregion

        Console.WriteLine("==== RIDGE DATA ====");
        foreach (KeyValuePair<MantlePlume, MantlePlume> entry in ridgePairs)
        {
            Ridges.Add(GenerateRidge(entry.Key, entry.Value, maxLateralRidgeSway));
        }

        FindCellsInRidges(plumeCenterSize, Ridges);

        
        int ridgeCells = 0;
        foreach(var ridge in Ridges)
        {
            Console.WriteLine($"Ridge: {ridge.RidgeId} Lateral: {ridge.LateralAngle} Normal: {ridge.Normal} \n" +
                $"Start Plume: ID: {ridge.PlumeAId} Position: {PlumeMap[ridge.PlumeAId].Center} End Plume:  ID: {ridge.PlumeBId} Position: {PlumeMap[ridge.PlumeBId].Center}");
            Console.WriteLine("--Segments--");
            foreach (var seg in ridge.Segments)
            {
                Console.WriteLine($"START: {seg.StartPoint} END: {seg.EndPoint} NORMAL: {seg.Normal} DISTANCE: {Vector3.Distance(seg.StartPoint, seg.EndPoint)}");
            }
            ridgeCells += ridge.ContainedCellIds.Count();
        }
        
        Console.WriteLine($"Total Cells: {Cells.Count()} Ridge Cells: {ridgeCells}");


        //UPDATE!
        currentProgress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Tectonic,
            StageProgress = 0,
            CurrentStep = WorldGenStepTypes.GeneratingInitialRidges,
            Payload = TectonicsPayloadCreator.CreateTectonicPayload(Ridges, TectonicCellMap)
        });
        
        Console.WriteLine("WORLDGEN: Finished finding cells in the ridges!");
    }
    private MantlePlume GeneratePlume(int size, Vector3 center)
    {
        MantlePlume newPlume = new MantlePlume(nextPlumeId, center, size);
        nextPlumeId++;
        
        float plumeRadius = size / (2f * currentParameters.TectonicParameters.PlanetRadius);

        foreach(var cell in Cells)
        {
            float angularDistance = MathF.Acos(Math.Clamp(Vector3.Dot(cell.Geometry.Position, center), -1f, 1f));
            if(angularDistance <= plumeRadius)
            {
                newPlume.CellIds.Add(cell.Geometry.CellId);
                if (angularDistance <= plumeRadius * currentParameters.TectonicParameters.PlumeCenterSize)
                {
                    newPlume.CenterCellIds.Add(cell.Geometry.CellId);
                }
            }
        }


        PlumeMap.Add(newPlume.PlumeID, newPlume);
        return newPlume;
    }
    private MantleRidge GenerateRidge(MantlePlume plumeA,  MantlePlume plumeB, float maxLateralRidgeSway)
    {
        //create a random number of points along the line between the 2 seed plumes
        //disperse them evenly along the line.
        int segmentCount = rng.Next(8, 15);
        float spaceBetweenPoints = 1f / (segmentCount + 1);
        List<Vector3> points = new();
        points.Add(plumeA.Center);

        for (int i = 1; i < segmentCount; i++)
        {
            Vector3 point = Vector3.Normalize(Vector3.Lerp(plumeA.Center, plumeB.Center, spaceBetweenPoints * (i + 1)));
            points.Add(point);
        }

        points.Add(plumeB.Center);


        //build segments
        //each segment is built of two endpoints that are a random distance between each of the previous made points. 
        List<RidgeSegment> segments = new();
        Dictionary<(Vector3, Vector3), Vector3> cachedMidpoints = new();

        for (int i = 0; i < points.Count(); i++)
        {
            Vector3 startPoint;
            Vector3 endPoint;
            if (i < points.Count() - 1)
            {
                if (i == 0)
                {
                    startPoint = points[i];
                }
                else
                {
                    startPoint = cachedMidpoints[(points[i - 1], points[i])];
                }


                float rngDistance = rng.NextSingle() * 0.6f + 0.2f;
                Vector3 point = Vector3.Normalize(Vector3.Lerp(points[i], points[i + 1], rngDistance));
                Vector3 segmentDirection = Vector3.Normalize(Vector3.Cross(Vector3.Cross(points[i], points[i + 1]), points[i]));

                float adjustment = Vector3.Dot(point - points[i], segmentDirection);

                endPoint = Vector3.Normalize(points[i] + segmentDirection * adjustment);

                cachedMidpoints.Add((points[i], points[i + 1]), endPoint);
            }
            else
            {
                startPoint = cachedMidpoints[(points[i - 1], points[i])];
                endPoint = points[i];
            }
            Vector3 segmentNormalVector = Vector3.Normalize(Vector3.Cross(startPoint, endPoint));
            segments.Add(new RidgeSegment(startPoint, endPoint, segmentNormalVector));
        }


        //Choose a random segment to be our furthest segment on the bend
        //Move it laterally a random amount within "reason"

        int index = rng.Next(3, segmentCount - 3);
        Vector3 normalVector = Vector3.Normalize(Vector3.Cross(plumeA.Center, plumeB.Center));
        float widthBased = (currentParameters.TectonicParameters.RidgeWidth * maxLateralRidgeSway) / currentParameters.TectonicParameters.PlanetRadius;
        float ridgeArc = MathF.Acos(Vector3.Dot(plumeA.Center, plumeB.Center));
        float maxLateral = MathF.Min(widthBased, ridgeArc * 0.15f);
        float lateralAngle = (rng.NextSingle() * 2f - 1f) * maxLateral;
        segments[index].StartPoint = Vector3.Normalize(segments[index].StartPoint * MathF.Cos(lateralAngle) + normalVector * MathF.Sin(lateralAngle));
        segments[index].EndPoint = Vector3.Normalize(segments[index].EndPoint * MathF.Cos(lateralAngle) + normalVector * MathF.Sin(lateralAngle));
        segments[index].Normal = Vector3.Normalize(Vector3.Cross(segments[index].StartPoint, segments[index].EndPoint));


        //move all the other segments into a staircase up to the furthest point and back down
        float inwardLateralStep = lateralAngle / index;
        float currentLateralDisplacement = inwardLateralStep;
        float outwardLateralStep = lateralAngle / (points.Count() - (index +1));

        for (int i = 1; i < segments.Count() - 1; i++)
        {
            if (i < index)
            {
                segments[i].StartPoint = Vector3.Normalize(segments[i].StartPoint * MathF.Cos(currentLateralDisplacement) + normalVector * MathF.Sin(currentLateralDisplacement));
                segments[i].EndPoint = Vector3.Normalize(segments[i].EndPoint * MathF.Cos(currentLateralDisplacement) + normalVector * MathF.Sin(currentLateralDisplacement));
                segments[i].Normal = Vector3.Normalize(Vector3.Cross(segments[i].StartPoint, segments[i].EndPoint));
                currentLateralDisplacement += inwardLateralStep;
            }
            else if (i == index) continue;
            else
            {
                currentLateralDisplacement -= outwardLateralStep;
                segments[i].StartPoint = Vector3.Normalize(segments[i].StartPoint * MathF.Cos(currentLateralDisplacement) + normalVector * MathF.Sin(currentLateralDisplacement));
                segments[i].EndPoint = Vector3.Normalize(segments[i].EndPoint * MathF.Cos(currentLateralDisplacement) + normalVector * MathF.Sin(currentLateralDisplacement));
                segments[i].Normal = Vector3.Normalize(Vector3.Cross(segments[i].StartPoint, segments[i].EndPoint));
            }

        }


        //rough up the eveness of the segment steps to produce a more natural result - possibly...


        MantleRidge newRidge = new MantleRidge(nextRidgeId, plumeA.PlumeID, plumeB.PlumeID, lateralAngle, normalVector, segments);
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
                float thresholdSin = MathF.Sin((normalizedRidgeWidth) + MathF.Abs(ridge.LateralAngle));
                float distanceToArc = MathF.Abs(Vector3.Dot(ridge.Normal, cell.Geometry.Position));
                bool withinWidth = distanceToArc <= thresholdSin;

                Vector3 ridgeDirection = Vector3.Normalize(Vector3.Cross(ridge.Normal, PlumeMap[ridge.PlumeAId].Center));
                float cellT = Vector3.Dot(cell.Geometry.Position, ridgeDirection);
                float endBT = Vector3.Dot(PlumeMap[ridge.PlumeBId].Center, ridgeDirection);
                float startBT = Vector3.Dot(PlumeMap[ridge.PlumeAId].Center, ridgeDirection);
                bool betweenEndpoints = cellT >= startBT && cellT <= endBT;

                bool correctSide = Vector3.Dot(cell.Geometry.Position, PlumeMap[ridge.PlumeAId].Center) > 0 ||
                   Vector3.Dot(cell.Geometry.Position, PlumeMap[ridge.PlumeBId].Center) > 0;


                if (withinWidth && correctSide)
                {
                    //Console.WriteLine($"Ridge Checks -- ThresholdSin:{thresholdSin} DistancetoArc: {distanceToArc} withinWidth: {withinWidth} " +
                    // $"Direction: {ridgeDirection} cellT: {cellT} EndBT: {endBT} BetweenEnds: {betweenEndpoints}");
                    
                    RidgeSegment? bestSegment = null;
                    float bestDistance = float.MaxValue;

                    foreach (RidgeSegment segment in ridge.Segments)
                    {
                        float segmentthresholdSin = MathF.Sin(normalizedRidgeWidth);
                        float distanceToSegmentArc = MathF.Abs(Vector3.Dot(segment.Normal, cell.Geometry.Position));
                        bool withinSegmentWidth = distanceToSegmentArc <= segmentthresholdSin;

                        Vector3 segmentDirection = Vector3.Normalize(Vector3.Cross(segment.Normal, segment.StartPoint));
                        float cellSegmentT = Vector3.Dot(cell.Geometry.Position, segmentDirection);
                        float segmentEndBT = Vector3.Dot(segment.EndPoint, segmentDirection);
                        float segmentStartBT = Vector3.Dot(segment.StartPoint, segmentDirection);
                        bool betweenSegmentEndpoints = cellSegmentT >= MathF.Min(segmentStartBT, segmentEndBT) &&
                               cellSegmentT <= MathF.Max(segmentStartBT, segmentEndBT);


                        if (withinSegmentWidth && betweenSegmentEndpoints && distanceToSegmentArc < bestDistance)
                        {
                            bestDistance = distanceToSegmentArc;
                            bestSegment = segment;
                        }
                        
                    }//segment Loop

                    if(bestSegment != null)
                    {
                        float centerthresholdSin = MathF.Sin((normalizedRidgeWidth) * centerSize);
                        if (bestDistance <= centerthresholdSin)
                        {
                            ridge.CenterCellIds.Add(cell.Geometry.CellId);
                        }

                        ridge.ContainedCellIds.Add(cell.Geometry.CellId);
                        break;//can only be in one ridge
                    }
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
