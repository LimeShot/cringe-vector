using OpenTK.Mathematics;

namespace CringeCraft.Client.Model.Camera;
public class Camera {
   public Vector2 Position { get; set; } = Vector2.Zero; // Позиция камеры (центр)
   public float Zoom { get; private set; } = 1.0f;       // Масштаб (1.0f = исходный размер)
   public float MinZoom { get; set; } = 0.1f;            // Минимальный масштаб
   public float MaxZoom { get; set; } = 10.0f;           // Максимальный масштаб

   private readonly Vector2 _viewportSize;               // Размер области просмотра (в пикселях)

   public Camera(Vector2 viewportSize) {
      _viewportSize = viewportSize;
   }

   // Установка масштаба с ограничением
   public void SetZoom(float zoom) {
      Zoom = Math.Clamp(zoom, MinZoom, MaxZoom);
   }

   // Изменение масштаба (например, через колесо мыши)
   public void AdjustZoom(float delta, Vector2 zoomCenter) {
      float oldZoom = Zoom;
      float newZoom = Zoom * (1.0f + delta * 0.1f); // delta — направление и скорость изменения
      SetZoom(newZoom);

      // Корректировка позиции камеры для масштабирования относительно точки (zoomCenter)
      Position += (zoomCenter - Position) * (1.0f - Zoom / oldZoom);
   }

   // Преобразование экранных координат (мыши) в мировые координаты холста
   public Vector2 ScreenToWorld(Vector2 screenPoint) {
      return (screenPoint - _viewportSize * 0.5f) / Zoom + Position;
   }

   // Преобразование мировых координат в экранные для рендеринга
   public Vector2 WorldToScreen(Vector2 worldPoint) {
      return (worldPoint - Position) * Zoom + _viewportSize * 0.5f;
   }

   // Получение матрицы преобразования для OpenGL
   public Matrix4 GetViewMatrix() {
      return Matrix4.CreateScale(Zoom) *
             Matrix4.CreateTranslation(-Position.X * Zoom, -Position.Y * Zoom, 0) *
             Matrix4.CreateTranslation(_viewportSize.X * 0.5f, _viewportSize.Y * 0.5f, 0);
   }
}
