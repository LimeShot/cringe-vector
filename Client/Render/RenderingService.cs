namespace CringeCraft.Client.Render;

using CringeCraft.Client.Graphics.OpenGL;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class RenderingService {
    private readonly MyCanvas _canvas;
    private Shader? _shaderLine, _shaderTriangle, _shaderEllipse;
    private VAO? _vaoLine, _vaoTriangle, _vaoEllipse;
    private Matrix4 _projection = Matrix4.Identity, _view = Matrix4.Identity;

    public RenderingService(MyCanvas Cringe) {
        _canvas = Cringe;
    }

    public void Initialize() {
        _shaderLine = new(@"
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
        _vaoLine = new([3, 3]);

        _shaderTriangle = new(@"
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
        _vaoTriangle = new([3, 3]);

        _shaderEllipse = new(@"
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aSize;
            layout (location = 2) in vec3 aColor;

            out vec3 VertPos;
            out vec2 VertSize;
            out vec3 VertColor;

            void main() {
                VertPos = aPos;
                VertSize = aSize;
                VertColor = aColor;
                gl_Position = vec4(VertPos, 1.0);
            }
            ", @"
            #version 330 core

            layout(points) in;
            layout(triangle_strip, max_vertices = 120) out;

            in vec3 VertPos[];
            in vec2 VertSize[];
            in vec3 VertColor[];

            out vec3 GeomColor;

            uniform mat4 uProjection, uView;

            void main() {
                vec3 center = VertPos[0].xyz;
                float width = VertSize[0].x;
                float height = VertSize[0].y;
                float halfW = width * 0.5;
                float halfH = height * 0.5;

                GeomColor = VertColor[0];

                for (int i = 1; i <= 40; i++) {
                    gl_Position = uProjection * uView * vec4(center, 1.0);
                    EmitVertex();

                    float angle = (3.1415926 * 2.0) * ((i - 1) / float(40));
                    float x = cos(angle);
                    float y = sin(angle);
                    gl_Position = uProjection * uView * vec4(center.x + x * halfW, center.y + y * halfH, center.z, 1.0);
                    EmitVertex();

                    angle = (3.1415926 * 2.0) * (i / float(40));
                    x = cos(angle);
                    y = sin(angle);
                    gl_Position = uProjection * uView * vec4(center.x + x * halfW, center.y + y * halfH, center.z, 1.0);
                    EmitVertex();
                }

                EndPrimitive();
            }
            ", @"
            #version 330 core

            in vec3 GeomColor;

            out vec4 FragColor;

            void main() {
                FragColor = vec4(GeomColor, 1.0);
            }");
        _vaoEllipse = new([3, 2, 3]);

        _vaoLine.Append([
            -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.8f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.8f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.6f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.9f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.7f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,

            -0.58f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.58f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.58f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.4f, 0.75f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.4f, 0.75f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.58f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.58f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.4f, 0.25f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.4f, 0.25f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.58f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

            -0.35f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.3f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.3f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.25f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.25f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.3f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.3f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.35f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f,

            -0.18f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.18f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.18f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.0f, 0.75f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.0f, 0.75f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.18f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.18f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.0f, 0.25f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.0f, 0.25f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.18f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,

            0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            0.2f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.2f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.4f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.1f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.3f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
        ]);

        _vaoTriangle.Append([
            -1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f
        ]);

        _vaoEllipse.Append([
            0.0f, 0.0f, 0.0f, 2.0f, 2.0f, 0.5f, 0.5f, 0.5f
        ]);

        GL.Enable(EnableCap.DepthTest);
    }

    public void Render(TimeSpan timeSpan) {
        if (_shaderLine == null || _vaoLine == null || _shaderTriangle == null || _vaoTriangle == null || _shaderEllipse == null || _vaoEllipse == null)
            return;
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderLine.Id);
        GL.BindVertexArray(_vaoLine.Id);
        _shaderLine.SetUniform("uProjection", _projection);
        _shaderLine.SetUniform("uView", _view);
        GL.DrawArrays(PrimitiveType.Lines, 0, _vaoLine.Length / _vaoLine.Stride);

        GL.UseProgram(_shaderTriangle.Id);
        GL.BindVertexArray(_vaoTriangle.Id);
        _shaderTriangle.SetUniform("uProjection", _projection);
        _shaderTriangle.SetUniform("uView", _view);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vaoTriangle.Length / _vaoTriangle.Stride);

        GL.UseProgram(_shaderEllipse.Id);
        GL.BindVertexArray(_vaoEllipse.Id);
        _shaderEllipse.SetUniform("uProjection", _projection);
        _shaderEllipse.SetUniform("uView", _view);
        GL.DrawArrays(PrimitiveType.Points, 0, _vaoEllipse.Length / _vaoEllipse.Stride);

        GL.UseProgram(_shaderTriangle.Id);
    }

    public void OnShapeAdded(IShape shape) {
        //
    }

    public void OnShapeUpdated(IShape shape) {
        //
    }

    public void OnShapeRemoved(IShape shape) {
        //
    }

    public void OnResize(int width, int height) {
        float aspect = (float)width / height;
        _projection = Matrix4.CreateOrthographicOffCenter(-aspect, aspect, -1, 1, -1, 1);
        _view = Matrix4.Identity;
    }

    public void Cleanup() {
        if (_vaoLine != null) {
            _vaoLine.Dispose();
            _vaoLine = null;
        }
        if (_shaderLine != null) {
            _shaderLine.Dispose();
            _shaderLine = null;
        }
    }
}