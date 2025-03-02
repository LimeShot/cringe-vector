using System.Reflection;
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
}