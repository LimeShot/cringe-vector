namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

public interface IShape {
    public Vector2 Translate { get; }
    public float Z { set; get; }
    public float Rotate { get; } // Угол в градусах
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { get; }
    public Vector2[] Nodes { set; get; }

    //Метод возвращает линии фигуры(без заполнения(Fill=false)) необходимые для отрисовки
    public float[] GetLineVertices();

    //Метод возвращает треугольники(с заполнением(Fill=true)) необходимые для отрисовки фигуры
    public float[] GetTriangleVertices();

    //Метод возвращает окружность(с заполнением(Fill=false)) необходимые для отрисовки фигуры
    public float[] GetCircumferenceVertices();

    //Метод возвращает Эллипс(с заполнением(Fill=true)) необходимые для отрисовки фигуры
    public float[] GetCircleVertices();

    //Метод для проверки принадлежности точки фигуре 
    public bool IsBelongsShape(Vector2 point);

    //Метод для проверки принадлежности точки BoundingBox, возвращает индекс вершины
    public int IsBBNode(Vector2 point);

    // Метод для перемещения фигуры на основе разницы между двумя точками
    public void Move(Vector2 oldPoint, Vector2 newPoint);

    // Метод чисто для Безье(но это не точно)
    public void MoveNode(int index, Vector2 newNode);

    // Метод изменения фигуры при изменении координат только одного узла BoundingBox
    public void Resize(int index, Vector2 newNode);

    public event Action? OnChange;
}
