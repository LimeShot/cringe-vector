using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenTK.Mathematics;
namespace CringeCraft.IO;

// Кастомный конвертер для автоматического определения типаusing OpenTK.Mathematics;

// Конвертер для Vector2
public class Vector2Converter : JsonConverter<Vector2> {
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer) {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteValue(value.X);
        writer.WritePropertyName("Y");
        writer.WriteValue(value.Y);
        writer.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        JObject obj = JObject.Load(reader);
        return new Vector2(
            (float)obj["X"],
            (float)obj["Y"]
        );
    }
}

// Конвертер для Vector3
public class Vector3Converter : JsonConverter<Vector3> {
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer) {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteValue(value.X);
        writer.WritePropertyName("Y");
        writer.WriteValue(value.Y);
        writer.WritePropertyName("Z");
        writer.WriteValue(value.Z);
        writer.WriteEndObject();
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue,
                                    bool hasExistingValue, JsonSerializer serializer) {
        JObject obj = JObject.Load(reader);
        return new Vector3(
            (float)obj["X"],
            (float)obj["Y"],
            (float)obj["Z"]
        );
    }
}
public class PrivateSetterContractResolver : DefaultContractResolver {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);

        // Разрешаем запись в приватные сеттеры
        if (prop.Writable == false && member is PropertyInfo property) {
            prop.Writable = property.GetSetMethod(nonPublic: true) != null;
        }

        // Исключаем свойство BoundingBox
        if (prop.PropertyName == "LocalBoundingBox" || prop.PropertyName == "BoundingBox" || prop.PropertyName == "SelectedShapes" || prop.PropertyName == "ScreenPerWorld") {
            prop.ShouldSerialize = _ => false;
        }

        return prop;
    }
}