using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using TerraMachina.Abstractions.Surface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TerraMachina.WorldGen.Tectonics;

public static class TectonicPlateNameGenerator
{
    private static List<string> prefixes = ["Ard", "Vel", "Cor", "Mal", "Sten", "Byr", "Kel", "Dav", "Mor", "Fen", "Tor", "Ash"];
    private static List<string> middles = ["an", "eth", "or", "al", "en", "ith", "ur", "om", "el", "ar", "in", "oth"];
    private static List<string> suffixes = ["wick", "moor", "fell", "sten", "dale", "thorpe", "shaw", "mere", "ford", "holm", "crest", "vale"];
    public static string GenerateTectonicPlateName(Random rng)
    {
        string prefix = prefixes[rng.Next(0, prefixes.Count)];
        string suffix = suffixes[rng.Next(0, suffixes.Count)];

        if(rng.Next(0,2) > 0)
        {
            string middle = middles[rng.Next(0, middles.Count)];

            return prefix + middle + suffix;
        }
        else
        {
            return prefix + suffix;
        }

    }
}
