namespace CringeCraft.Client.Model;

using CringeCraft.Client.Render;

using OpenTK.Mathematics;


public class Camera {
   public Vector2 Position { get; set; } = Vector2.Zero;
   public float Zoom { get; private set; } = 1.0f;
   public float MinZoom { get; set; } = 0.1f;
   public float MaxZoom { get; set; } = 10.0f;

   private Vector2 _viewportSize;

   public event Action<Matrix4, Matrix4> MatrixProjecitonPortChanged;

   public Camera(RenderingService rendering) {
      _viewportSize = Vector2.Zero;
      MatrixProjecitonPortChanged += rendering.OnProjectionChanged;
   }

   public void UpdateViewport(double ActualWidth, double ActualHeight) {
      _viewportSize = ((float)ActualWidth, -(float)ActualHeight);
      UpdateViewMatrix();
   }

   public void SetPosition(Vector2 point) {
      Position += point;
      UpdateViewMatrix();
   }

   public void UndoPosition() {
      Position = (0.0f, 0.0f);
      UpdateViewMatrix();
   }

   public void SetZoom(float zoom) {
      Zoom = Math.Clamp(zoom, MinZoom, MaxZoom);
   }

   public void AdjustZoom(float delta, Vector2 zoomCenter) {
      float oldZoom = Zoom;
      float newZoom = Zoom * (1.0f + delta * 0.1f);
      SetZoom(newZoom);

      Position += (zoomCenter - Position) * (1.0f - Zoom / oldZoom);
   }

   public Vector2 ScreenToWorld(Vector2 screenPoint) {
      screenPoint.Y = -screenPoint.Y;
      return (screenPoint - _viewportSize * 0.5f) / Zoom + Position;
   }

   public Vector2 WorldToScreen(Vector2 worldPoint) {
      var screenPoint = (worldPoint - Position) * Zoom + _viewportSize * 0.5f;
      screenPoint = -screenPoint;
      return screenPoint;
   }


   private void UpdateViewMatrix() {
      var projection = Matrix4.CreateOrthographicOffCenter(
         -_viewportSize.X / 2.0f, _viewportSize.X / 2.0f,
         _viewportSize.Y / 2.0f, -_viewportSize.Y / 2.0f,
         -1, 1);

      var view = Matrix4.CreateTranslation(-Position.X, -Position.Y, 0); // Инвертированное смещение

      MatrixProjecitonPortChanged?.Invoke(projection, view);
   }

}
