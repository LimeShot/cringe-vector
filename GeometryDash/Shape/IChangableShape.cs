namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

public interface IChangableShape {

    // Метод чисто для Безье и многоугольника(но это не точно)
    public void MoveNode(int index, Vector2 newNode);
}
