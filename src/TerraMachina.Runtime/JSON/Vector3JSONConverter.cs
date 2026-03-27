using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TerraMachina.Runtime.JSON;

public class Vector3JsonConverter : JsonConverter<Vector3>
{
    public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        float x = 0, y = 0, z = 0;
        reader.Read();
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            string prop = reader.GetString();
            reader.Read();
            if (prop == "x") x = reader.GetSingle();
            else if (prop == "y") y = reader.GetSingle();
            else if (prop == "z") z = reader.GetSingle();
            reader.Read();
        }
        return new Vector3(x, y, z);
    }

    public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("x", Math.Round(value.X, 4));
        writer.WriteNumber("y", Math.Round(value.Y, 4));
        writer.WriteNumber("z", Math.Round(value.Z, 4));
        writer.WriteEndObject();
    }
}
