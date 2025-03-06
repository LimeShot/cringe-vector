using System.Reflection;
<<<<<<< HEAD
using System.Text.Json;
using System.Text.Json.Serialization;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.IO;

// Кастомный конвертер для автоматического определения типа
public class ShapeConverter : JsonConverter<IShape> {
    public override IShape Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        // Копируем JSON в документ, чтобы прочитать поле "Type"
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader)) {
            var root = doc.RootElement;
            string typeName = root.GetProperty("Type").GetString();

            // Находим тип по переменной typeName
            Type shapeType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName && typeof(IShape).IsAssignableFrom(t));

            if (shapeType == null) {
                throw new NotSupportedException($"Unknown shape type: {typeName}");
            }

            // Десериализуем объект в найденный тип
            return (IShape)JsonSerializer.Deserialize(root.GetRawText(), shapeType, options);
        }
    }

    public override void Write(Utf8JsonWriter writer, IShape value, JsonSerializerOptions options) {
        // Добавляем поле "Type" в JSON
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().Name); // Записываем имя класса
        foreach (var property in value.GetType().GetProperties()) {
            if (property.CanRead) {
                writer.WritePropertyName(property.Name);
                writer.WriteRawValue(JsonSerializer.Serialize(property.GetValue(value), options));
            }
        }
        writer.WriteEndObject();
    }
=======
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenTK.Mathematics;
namespace CringeCraft.IO;

// Кастомный конвертер для автоматического определения типаusing OpenTK.Mathematics;

// Конвертер для Vector2
public class Vector2Converter : JsonConverter<Vector2>{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
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
public class Vector3Converter : JsonConverter<Vector3>{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer){
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
                                    bool hasExistingValue, JsonSerializer serializer){
        JObject obj = JObject.Load(reader);
        return new Vector3(
            (float)obj["X"],
            (float)obj["Y"],
            (float)obj["Z"]
        );
    }
}
public class PrivateSetterContractResolver : DefaultContractResolver{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)    {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);

        // Разрешаем запись в приватные сеттеры
        if (prop.Writable == false && member is PropertyInfo property){
            prop.Writable = property.GetSetMethod(nonPublic: true) != null;
        }

        // Исключаем свойство BoundingBox
        if (prop.PropertyName == "BoundingBox" || prop.PropertyName == "SelectedShapes"){
            prop.ShouldSerialize = _ => false;
        }

        return prop;
    }
>>>>>>> dev
}