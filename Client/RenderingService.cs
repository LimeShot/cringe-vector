using OpenTK.Graphics.OpenGL4;

namespace CringeCraft.Client.Render;

public class RenderingService {
    private int shaderProgram;
    private int vbo;
    private int vao;

    // 🟢 Команды
    public void InitializeOpenGL(string StatusMessage) {
        StatusMessage = "Компиляция шейдеров...";

        // Компиляция шейдеров
        string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                void main() {
                    gl_Position = vec4(aPos, 1.0);
                }";
        string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                void main() {
                    FragColor = vec4(1.0, 0.5, 0.2, 1.0);
                }";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderErrors(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderErrors(fragmentShader);

        shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);
        GL.DetachShader(shaderProgram, vertexShader);
        GL.DetachShader(shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // 🟢 Обновляем статус
        StatusMessage = "Шейдеры загружены!";

        SetupVBO();
    }

    public void Render(TimeSpan timeSpan) {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }
    
    private void SetupVBO() {
        vao = GL.GenVertexArray();
        vbo = GL.GenBuffer();
        GL.BindVertexArray(vao);

        float[] vertices = {
                -0.5f, -0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                 0.0f,  0.5f, 0.0f
            };

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
    }

    private void CheckShaderErrors(int shader) {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0) {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Shader compilation error: {infoLog}");
        }
    }
}