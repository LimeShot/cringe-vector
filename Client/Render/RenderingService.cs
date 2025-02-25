namespace CringeCraft.Client.Render;

using ElementalAdventure.Client.Graphics.OpenGL;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class RenderingService {
    private Shader? _shaderLine;
    private VAO? _vaoLine;
    private Matrix4 _projection = Matrix4.Identity, _view = Matrix4.Identity;

    public void Initialize() {
        _shaderLine = new Shader(@"
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec3 aColor;

            out vec3 VertColor;

            uniform mat4 uProjection, uView;

            void main() {
                gl_Position = uProjection * uView * vec4(aPos, 1.0);
                VertColor = aColor;
            }
            ", @"
            #version 330 core

            in vec3 VertColor;

            out vec4 FragColor;

            void main() {
                FragColor = vec4(VertColor, 1.0);
            }");
        _vaoLine = new VAO([3, 3]);

        _vaoLine.Append([
            -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.8f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.8f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.6f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.9f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.7f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,

            -0.58f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.58f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.58f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.4f, 0.75f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.4f, 0.75f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.58f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.58f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.4f, 0.25f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.4f, 0.25f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.58f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

            -0.35f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.3f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.3f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.25f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.25f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.3f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.3f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.35f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,

            -0.18f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.18f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.18f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.0f, 0.75f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.0f, 0.75f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.18f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.18f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.0f, 0.25f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.0f, 0.25f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.18f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.2f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.2f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.4f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.1f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.3f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
        ]);
    }

    public void Render(TimeSpan timeSpan) {
        if (_shaderLine == null || _vaoLine == null)
            return;
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_shaderLine.Id);
        GL.BindVertexArray(_vaoLine.Id);
        _shaderLine.SetUniform("uProjection", _projection);
        _shaderLine.SetUniform("uView", _view);
        GL.DrawArrays(PrimitiveType.Lines, 0, _vaoLine.Length / _vaoLine.Stride);
    }

    public void OnShapesUpdated() {
        //
    }

    public void OnResize(int width, int height) {
        float aspect = (float)width / height;
        _projection = Matrix4.CreateOrthographicOffCenter(-aspect, aspect, -1, 1, -1, 1);
        _view = Matrix4.Identity;
    }
}