namespace CringeCraft.Client.Render;

using ElementalAdventure.Client.Graphics.OpenGL;

using OpenTK.Graphics.OpenGL4;

public class RenderingService {
    private Shader? _shaderLine;
    private VAO? _vaoLine;

    public void Initialize() {
        _shaderLine = new Shader(@"
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec3 aColor;

            out vec3 VertColor;

            void main() {
                gl_Position = vec4(aPos, 1.0);
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
            0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        ]);
    }

    public void Render(TimeSpan timeSpan) {
        if (_shaderLine == null || _vaoLine == null)
            return;
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_shaderLine.Id);
        GL.BindVertexArray(_vaoLine.Id);
        GL.DrawArrays(PrimitiveType.Lines, 0, _vaoLine.Length / _vaoLine.Stride);
    }

    public void OnShapesUpdated() {
        //
    }
}