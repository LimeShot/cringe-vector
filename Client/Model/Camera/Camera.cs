namespace CringeCraft.Client.Model;

using CommunityToolkit.Mvvm.ComponentModel;
using OpenTK.Mathematics;
using CringeCraft.Client.Render;

public partial class Camera : ObservableObject {

   [ObservableProperty]
   private string _percentZoom = $"Zoom: 100%";
   private Vector2 _viewportSize;

   public Vector2 Position { get; set; } = Vector2.Zero;
   public float Zoom { get; private set; } = 1.0f;
   public float MinZoom { get; set; } = 0.1f;
   public float MaxZoom { get; set; } = 5.0f;
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
      Position += point / Zoom;
      UpdateViewMatrix();
   }

   public void UndoPosition() {
      Position = (0.0f, 0.0f);
      Zoom = 1.0f;
      UpdateViewMatrix();
   }

   public void AdjustZoom(float delta, Vector2 zoomCenter) {
      Console.WriteLine(delta);
      float oldZoom = Zoom;
      float newZoom = Zoom + delta * 0.001f;
      Zoom = Math.Clamp(newZoom, MinZoom, MaxZoom);
      PercentZoom = $"Zoom: {Math.Round(Zoom * 100)}%";
      UpdateViewMatrix();
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
      var projection = Matrix4.CreateScale(Zoom, Zoom, 1) * Matrix4.CreateOrthographicOffCenter(
         -_viewportSize.X / 2.0f, _viewportSize.X / 2.0f,
         _viewportSize.Y / 2.0f, -_viewportSize.Y / 2.0f,
         -1, 1);

      var view = Matrix4.CreateTranslation(-Position.X, -Position.Y, 0);

      MatrixProjecitonPortChanged?.Invoke(projection, view);
   }
}
