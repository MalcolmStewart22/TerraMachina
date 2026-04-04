using System;
using System.Collections.Generic;
using System.Text;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;

namespace TerraMachina.WorldGen.Tectonics;

static class TectonicsPayloadCreator
{
    static public List<TectonicCellMinimum> ConvertToTectonicCellMinimums(List<int> cellIds, Dictionary<int, TectonicCell> tectonicCellMap)
    {
        List<TectonicCellMinimum> results = new();
        foreach (var id in cellIds)
        {
            TectonicCell cell = tectonicCellMap[id];
            results.Add(new TectonicCellMinimum(cell.Geometry.CellId, cell.Elevation, cell.PlateId));
        }

        return results;
    }
    static public List<TectonicCellMinimum> ConvertToTectonicCellMinimums(List<TectonicCell> cells)
    {
        List<TectonicCellMinimum> results = new();
        foreach (var cell in cells)
        {
            results.Add(new TectonicCellMinimum(cell.Geometry.CellId, cell.Elevation, cell.PlateId));
        }

        return results;
    }

    //Full Payload
    static public TectonicsProgressPayload CreateTectonicPayload(List<MantlePlume> plumes, List<MantleRidge> ridges, List<TectonicPlate> plates, List<TectonicCell> cells, Dictionary<int, TectonicCell> tectonicCellMap)
    {
        TectonicsProgressPayload result = new();

        result.Plumes = new();
        foreach (MantlePlume plume in plumes)
        {
            result.Plumes.Add(new PlumePayload(plume.PlumeID, ConvertToTectonicCellMinimums(plume.CellIds, tectonicCellMap)));
        }

        result.Ridges = new();
        foreach (MantleRidge ridge in ridges)
        {
            result.Ridges.Add(new RidgePayload(ridge.RidgeId, ConvertToTectonicCellMinimums(ridge.ContainedCellIds, tectonicCellMap))); ;
        }

        result.Plates = new();
        foreach (TectonicPlate plate in plates)
        {
            result.Plates.Add(plate.PlateID, plate.Name);
        }

        result.TectonicCells = new(ConvertToTectonicCellMinimums(cells));

        return result;
    }
    
    //Plumes Only
    static public TectonicsProgressPayload CreateTectonicPayload(List<MantlePlume> plumes, Dictionary<int, TectonicCell> tectonicCellMap)
    {
        TectonicsProgressPayload result = new();

        result.Plumes = new();
        foreach (MantlePlume plume in plumes)
        {
            result.Plumes.Add(new PlumePayload(plume.PlumeID, ConvertToTectonicCellMinimums(plume.CellIds, tectonicCellMap)));
        }

        return result;
    }

    
    //Ridges Only
    static public TectonicsProgressPayload CreateTectonicPayload(List<MantleRidge> ridges, Dictionary<int, TectonicCell> tectonicCellMap)
    {
        TectonicsProgressPayload result = new();

        result.Ridges = new();
        foreach (MantleRidge ridge in ridges)
        {
            result.Ridges.Add(new RidgePayload(ridge.RidgeId, ConvertToTectonicCellMinimums(ridge.ContainedCellIds, tectonicCellMap))); ;
        }

        return result;
    }

    //Plates Only
    static public TectonicsProgressPayload CreateTectonicPayload( List<TectonicPlate> plates, List<TectonicCell> cells)
    {
        TectonicsProgressPayload result = new();

        result.Plates = new();
        foreach (TectonicPlate plate in plates)
        {
            result.Plates.Add(plate.PlateID, plate.Name);
        }

        result.TectonicCells = new(ConvertToTectonicCellMinimums(cells));

        return result;
    }

    //Cells Only
    static public TectonicsProgressPayload CreateTectonicPayload(List<TectonicCell> cells)
    {
        TectonicsProgressPayload result = new();

        result.TectonicCells = new(ConvertToTectonicCellMinimums(cells));

        return result;
    }
}
