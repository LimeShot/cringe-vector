namespace CringeCraft.Client.Render;

using CringeCraft.Client.Graphics.OpenGL;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class RenderingService {
    private readonly MyCanvas _canvas;
    private Shader? _shaderLine, _shaderTriangle, _shaderCircle, _shaderCircumference;
    private VAO? _vaoLine, _vaoTriangle, _vaoCircle, _vaoCircumference;
    private VAO? _vaoBackground, _vaoBoundingBox;
    private Matrix4 _projection = Matrix4.Identity, _view = Matrix4.Identity;
    private Vector2[]? _bbox;

    public RenderingService(MyCanvas Cringe) {
        _canvas = Cringe;
    }

    public void Initialize() {
        _shaderLine = new(@"
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aTranslate;
            layout (location = 2) in float aRotate;
            layout (location = 3) in vec3 aColor;

            out vec3 VertColor;

            uniform mat4 uProjection, uView;
            uniform float uTime;

            const float PI = 3.1415926535897932384626433832795;

            float deg2rad(float deg) {
                return deg * (PI / 180.0);
            }

            mat4 rotate(float angle) {
                float cosTheta = cos(angle);
                float sinTheta = sin(angle);
                return mat4(
                    cosTheta, -sinTheta, 0.0, 0.0,
                    sinTheta,  cosTheta, 0.0, 0.0,
                    0.0,       0.0,      1.0, 0.0,
                    0.0,       0.0,      0.0, 1.0
                );
            }

            void main() {
                gl_Position = uProjection * uView * (rotate(deg2rad(aRotate)) * vec4(aPos, 1.0) + vec4(aTranslate, 0.0, 0.0));
                VertColor = aColor.x >= 0 ? aColor : vec3(abs(sin(uTime)) * 0.8);
            }
            ", @"
            #version 330 core

            in vec3 VertColor;

            out vec4 FragColor;

            void main() {
                FragColor = vec4(VertColor, 1.0);
            }");
        _vaoLine = new([3, 2, 1, 3]);
        _vaoBoundingBox = new([3, 2, 1, 3]);

        _shaderTriangle = new(@"
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aTranslate;
            layout (location = 2) in float aRotate;
            layout (location = 3) in vec3 aColor;

            out vec3 VertColor;

            uniform mat4 uProjection, uView;

            const float PI = 3.1415926535897932384626433832795;

            float deg2rad(float deg) {
                return deg * (PI / 180.0);
            }

            mat4 rotate(float angle) {
                float cosTheta = cos(angle);
                float sinTheta = sin(angle);
                return mat4(
                    cosTheta, -sinTheta, 0.0, 0.0,
                    sinTheta,  cosTheta, 0.0, 0.0,
                    0.0,       0.0,      1.0, 0.0,
                    0.0,       0.0,      0.0, 1.0
                );
            }

            void main() {
                gl_Position = uProjection * uView * (rotate(deg2rad(aRotate)) * vec4(aPos, 1.0) + vec4(aTranslate, 0.0, 0.0));
                VertColor = aColor;
            }
            ", @"
            #version 330 core

            in vec3 VertColor;

            out vec4 FragColor;

            void main() {
                FragColor = vec4(VertColor, 1.0);
            }");
        _vaoTriangle = new([3, 2, 1, 3]);

        _shaderCircle = new(@"
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aSize;
            layout (location = 2) in float aRotate;
            layout (location = 3) in vec3 aColor;

            out vec3 VertCenter;
            out vec2 VertSize;
            out float VertRotate;
            out vec3 VertColor;

            void main() {
                VertCenter = aPos;
                VertSize = aSize;
                VertRotate = aRotate;
                VertColor = aColor;
            }
            ", @"
            #version 330 core

            layout (points) in;
            layout (triangle_strip, max_vertices = 4) out;

            in vec3 VertCenter[];
            in vec2 VertSize[];
            in float VertRotate[];
            in vec3 VertColor[];

            out vec2 GeomLocalPos;
            out vec2 GeomHalfSize;
            out vec3 GeomColor;

            uniform mat4 uProjection;
            uniform mat4 uView;

            const float PI = 3.1415926535897932384626433832795;

            float deg2rad(float deg) {
                return deg * (PI / 180.0);
            }

            void main() {
                float a = VertSize[0].x * 0.5;
                float b = VertSize[0].y * 0.5;
                vec2 corners[4] = vec2[](vec2(-a, -b), vec2(a, -b), vec2(-a,  b), vec2(a,  b));

                float radians = -deg2rad(VertRotate[0]);
                float c = cos(radians);
                float s = sin(radians);

                for (int i = 0; i < 4; i++) {
                    GeomLocalPos = corners[i];
                    GeomHalfSize = vec2(a, b);
                    GeomColor = VertColor[0];
                    vec3 pos = VertCenter[0] + vec3(corners[i].x * c - corners[i].y * s, corners[i].x * s + corners[i].y * c, 0.0);
                    gl_Position = uProjection * uView * vec4(pos, 1.0);
                    EmitVertex();
                }
                EndPrimitive();
            }
            ", @"
            #version 330 core

            in vec2 GeomLocalPos;
            in vec2 GeomHalfSize;
            in vec3 GeomColor;

            out vec4 FragColor;

            void main() {
                float x = GeomLocalPos.x / GeomHalfSize.x;
                float y = GeomLocalPos.y / GeomHalfSize.y;
                float ellipse = x*x + y*y;
                if (ellipse > 1.0)
                    discard;
                FragColor = vec4(GeomColor, 1.0);
            }");
        _vaoCircle = new([3, 2, 1, 3]);

        _shaderCircumference = new(@"
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aSize;
            layout (location = 2) in float aRotate;
            layout (location = 3) in vec3 aColor;

            out vec3 VertCenter;
            out vec2 VertSize;
            out float VertRotate;
            out vec3 VertColor;

            void main() {
                VertCenter = aPos;
                VertSize = aSize;
                VertRotate = aRotate;
                VertColor = aColor;
            }
            ", @"
            #version 330 core

            layout (points) in;
            layout (triangle_strip, max_vertices = 4) out;

            in vec3 VertCenter[];
            in vec2 VertSize[];
            in float VertRotate[];
            in vec3 VertColor[];

            out vec2 GeomLocalPos;
            out vec2 GeomHalfSize;
            out vec3 GeomColor;

            uniform mat4 uProjection;
            uniform mat4 uView;

            const float PI = 3.1415926535897932384626433832795;

            float deg2rad(float deg) {
                return deg * (PI / 180.0);
            }

            void main() {
                float a = VertSize[0].x * 0.5;
                float b = VertSize[0].y * 0.5;
                vec2 corners[4] = vec2[](vec2(-a, -b), vec2(a, -b), vec2(-a,  b), vec2(a,  b));

                float radians = -deg2rad(VertRotate[0]);
                float c = cos(radians);
                float s = sin(radians);

                for (int i = 0; i < 4; i++) {
                    GeomLocalPos = corners[i];
                    GeomHalfSize = vec2(a, b);
                    GeomColor = VertColor[0];
                    vec3 pos = VertCenter[0] + vec3(corners[i].x * c - corners[i].y * s, corners[i].x * s + corners[i].y * c, 0.0);
                    gl_Position = uProjection * uView * vec4(pos, 1.0);
                    EmitVertex();
                }
                EndPrimitive();
            }
            ", @"
            #version 330 core

            in vec2 GeomLocalPos;
            in vec2 GeomHalfSize;
            in vec3 GeomColor;

            out vec4 FragColor;

            void main() {
                float x = GeomLocalPos.x / GeomHalfSize.x; 
                float y = GeomLocalPos.y / GeomHalfSize.y; 
                float ellipse = x*x + y*y;
                float w = 2.0 * fwidth(ellipse);
                if (ellipse > 1.0 || ellipse < 1.0 - w)
                    discard;
                FragColor = vec4(GeomColor, 1.0);
            }");
        _vaoCircumference = new([3, 2, 1, 3]);

        _vaoBackground = new([3, 2, 1, 3]);

        GL.Enable(EnableCap.DepthTest);
        rebuildVBOs();
    }

    public void OnBoundingBoxChanged(Vector2[]? bbox) {
        if (bbox == null) {
            _vaoBoundingBox?.SetData([]);
        } else {
            float z = 1.0f - 1e-3f;
            float[] boundingBoxVertices = {
                bbox[3].X, bbox[3].Y, z, 0, 0, 0, -1, -1, -1, bbox[2].X, bbox[2].Y, z, 0, 0, 0, -1, -1, -1,
                bbox[2].X, bbox[2].Y, z, 0, 0, 0, -1, -1, -1, bbox[1].X, bbox[1].Y, z, 0, 0, 0, -1, -1, -1,
                bbox[1].X, bbox[1].Y, z, 0, 0, 0, -1, -1, -1, bbox[0].X, bbox[0].Y, z, 0, 0, 0, -1, -1, -1,
                bbox[0].X, bbox[0].Y, z, 0, 0, 0, -1, -1, -1, bbox[3].X, bbox[3].Y, z, 0, 0, 0, -1, -1, -1,
                bbox[0].X, bbox[0].Y, z, 5, 5, 0, 0, 0, 0,
                bbox[1].X, bbox[1].Y, z, 5, 5, 0, 0, 0, 0,
                bbox[2].X, bbox[2].Y, z, 5, 5, 0, 0, 0, 0,
                bbox[3].X, bbox[3].Y, z, 5, 5, 0, 0, 0, 0
            };
            _vaoBoundingBox?.SetData(boundingBoxVertices);
        }
    }

    private void rebuildVBOs() {
        if (_shaderLine == null || _vaoLine == null || _shaderTriangle == null || _vaoTriangle == null || _shaderCircle == null || _vaoCircle == null || _shaderCircumference == null || _vaoCircumference == null || _vaoBackground == null)
            return;
        _vaoLine.Clear();
        _vaoTriangle.Clear();
        _vaoCircle.Clear();
        _vaoCircumference.Clear();
        _vaoBackground.Clear();
        foreach (IShape shape in _canvas.Shapes) {
            _vaoLine.Append(shape.GetLineVertices());
            _vaoTriangle.Append(shape.GetTriangleVertices());
            _vaoCircle.Append(shape.GetCircleVertices());
            _vaoCircumference.Append(shape.GetCircumferenceVertices());
        }
        _vaoBackground.Append([
            -_canvas.Width / 2.0f, -_canvas.Height / 2.0f, -0.9999f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
            _canvas.Width / 2.0f, -_canvas.Height / 2.0f, -0.9999f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
            _canvas.Width / 2.0f, _canvas.Height / 2.0f, -0.9999f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,

            -_canvas.Width / 2.0f, -_canvas.Height / 2.0f, -0.9999f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
            _canvas.Width / 2.0f, _canvas.Height / 2.0f, -0.9999f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
            -_canvas.Width / 2.0f, _canvas.Height / 2.0f, -0.9999f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f
        ]);
    }

    public void Render(TimeSpan timeSpan) {
        if (_shaderLine == null || _vaoLine == null || _shaderTriangle == null || _vaoTriangle == null || _shaderCircle == null || _vaoCircle == null || _shaderCircumference == null || _vaoCircumference == null || _vaoBackground == null || _vaoBoundingBox == null)
            return;
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderLine.Id);
        _shaderLine.SetUniform("uProjection", _projection);
        _shaderLine.SetUniform("uView", _view);
        _shaderLine.SetUniform("uTime", (float)DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0f);
        GL.BindVertexArray(_vaoLine.Id);
        GL.DrawArrays(PrimitiveType.Lines, 0, _vaoLine.Length / _vaoLine.Stride);
        GL.BindVertexArray(_vaoBoundingBox.Id);
        GL.DrawArrays(PrimitiveType.Lines, 0, _vaoBoundingBox.Length == 0 ? 0 : 8);

        GL.UseProgram(_shaderTriangle.Id);
        _shaderTriangle.SetUniform("uProjection", _projection);
        _shaderTriangle.SetUniform("uView", _view);
        GL.BindVertexArray(_vaoTriangle.Id);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vaoTriangle.Length / _vaoTriangle.Stride);
        GL.BindVertexArray(_vaoBackground.Id);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vaoBackground.Length / _vaoBackground.Stride);

        GL.UseProgram(_shaderCircle.Id);
        _shaderCircle.SetUniform("uProjection", _projection);
        _shaderCircle.SetUniform("uView", _view);
        GL.BindVertexArray(_vaoCircle.Id);
        GL.DrawArrays(PrimitiveType.Points, 0, _vaoCircle.Length / _vaoCircle.Stride);
        GL.BindVertexArray(_vaoBoundingBox.Id);
        GL.DrawArrays(PrimitiveType.Points, _vaoBoundingBox.Length == 0 ? 0 : 8, _vaoBoundingBox.Length == 0 ? 0 : 4);

        GL.UseProgram(_shaderCircumference.Id);
        _shaderCircumference.SetUniform("uProjection", _projection);
        _shaderCircumference.SetUniform("uView", _view);
        GL.BindVertexArray(_vaoCircumference.Id);
        GL.DrawArrays(PrimitiveType.Points, 0, _vaoCircumference.Length / _vaoCircumference.Stride);
    }

    public void OnShapeAdded(params IShape[] shapes) {
        rebuildVBOs();
    }

    public void OnShapeUpdated(params IShape[] shapes) {
        rebuildVBOs();
    }

    public void OnShapeRemoved(params IShape[] shapes) {
        rebuildVBOs();
    }

    public void OnProjectionChanged(Matrix4 projection, Matrix4 view) {
        _projection = projection;
        _view = view;
    }

    public void OnCanvasResize(float width, float height) {
        if (_vaoBackground != null) _vaoBackground.ReplaceRange(0, _vaoBackground.Length, [
            -_canvas.Width / 2.0f, -_canvas.Height / 2.0f, -0.9999f, 1.0f, 1.0f, 1.0f,
            _canvas.Width / 2.0f, -_canvas.Height / 2.0f, -0.9999f, 1.0f, 1.0f, 1.0f,
            _canvas.Width / 2.0f, _canvas.Height / 2.0f, -0.9999f, 1.0f, 1.0f, 1.0f,

            -_canvas.Width / 2.0f, -_canvas.Height / 2.0f, -0.9999f, 1.0f, 1.0f, 1.0f,
            _canvas.Width / 2.0f, _canvas.Height / 2.0f, -0.9999f, 1.0f, 1.0f, 1.0f,
            -_canvas.Width / 2.0f, _canvas.Height / 2.0f, -0.9999f, 1.0f, 1.0f, 1.0f
        ]);
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