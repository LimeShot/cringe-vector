namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

public interface IShape {
    public Vector2 Translate { get; }
    public float Z { set; get; }
    public float DeltaZ { set; get; } // Для отрисовки контура на слой выше, чем заливки
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
    public bool IsBelongsShape(Vector2 point, float radiusPoint);

    //Метод для проверки принадлежности точки BoundingBox, возвращает индекс вершины
    public int IsBBNode(Vector2 point);

    // Метод для перемещения фигуры на основе разницы между двумя точками
    public void Move(Vector2 delta);

    // Метод изменения фигуры при изменении координат только одного узла BoundingBox
    public void Resize(int index, Vector2 newNode);

    public string ShapeType => GetType().Name;
    public string IconPath => $"pack://siteoforigin:,,,/assets/tools/{ShapeType.ToLower()}.png";

    // Метод изменения угла поворота фигуры
    public void RotateShape(Vector2 p1, Vector2 p2);

    // Метод возвращает нормальную нумерацию точек в фигуре(проблема возникает после использования метода Resize)
    public void NormalizeIndexNodes();

    // Методы отражения фигуры, путем измненеия угла
    public void ReflectX();
    public void ReflectY();

    IShape Clone(); // Метод для глубокого копирования
}
