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
    private VAO? _vaoBackground;
    private Matrix4 _projection = Matrix4.Identity, _view = Matrix4.Identity;

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
        _vaoLine = new([3, 2, 1, 3]);

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

            layout (location = 0) in vec3 aPos;      // Center of ellipse (x, y, z)
            layout (location = 1) in vec2 aSize;     // (width, height) of ellipse
            layout (location = 2) in float aRotate;  // Rotation *in degrees*
            layout (location = 3) in vec3 aColor;    // RGB color

            out VS_OUT {
                vec3  center;
                vec2  size;
                float rotateDeg; 
                vec3  color;
            } vs_out;

            void main() {
                vs_out.center    = aPos;
                vs_out.size      = aSize;
                vs_out.rotateDeg = aRotate;
                vs_out.color     = aColor;
            }
        ", @"
            #version 330 core

            layout (points) in;
            layout (triangle_strip, max_vertices = 4) out;

            uniform mat4 uProjection;
            uniform mat4 uView;

            in VS_OUT {
                vec3  center;      // (x, y, z)
                vec2  size;        // (width, height)
                float rotateDeg;   // rotation in degrees
                vec3  color;       // RGB
            } gs_in[];

            out GS_OUT {
                vec2 localPos;  // local ellipse coords in [-a..+a] x [-b..+b]
                vec2 halfSize;  // (a, b) for ellipse test
                vec3 color;
            } gs_out;

            void main() {
                vec3  center   = gs_in[0].center;      
                vec2  sizeXY   = gs_in[0].size;
                float rotateD  = gs_in[0].rotateDeg;
                vec3  color    = gs_in[0].color;

                float radians = -rotateD * 3.1415926535 / 180.0;
                float c = cos(radians);
                float s = sin(radians);

                float a = sizeXY.x * 0.5;  // semi-width
                float b = sizeXY.y * 0.5;  // semi-height

                vec2 corners[4] = vec2[](
                    vec2(-a, -b),  // bottom-left
                    vec2( a, -b),  // bottom-right
                    vec2(-a,  b),  // top-left
                    vec2( a,  b)   // top-right
                );

                for (int i = 0; i < 4; i++) {
                    gs_out.localPos = corners[i];
                    gs_out.halfSize = vec2(a, b);
                    gs_out.color    = color;
                    float rx = corners[i].x * c - corners[i].y * s; // x*cos - y*sin
                    float ry = corners[i].x * s + corners[i].y * c; // x*sin + y*cos
                    vec3 worldPos = center + vec3(rx, ry, 0.0);
                    gl_Position = uProjection * uView * vec4(worldPos, 1.0);
                    EmitVertex();
                }

                EndPrimitive();
            }
        ", @"
            #version 330 core

            in GS_OUT {
                vec2 localPos;   // local coords in [-a..+a], [-b..+b]
                vec2 halfSize;   // (a, b)
                vec3 color;
            } fs_in;

            out vec4 FragColor;

            void main() {
                float x = fs_in.localPos.x / fs_in.halfSize.x; // normalized to [-1..+1]
                float y = fs_in.localPos.y / fs_in.halfSize.y; // normalized to [-1..+1]
                float ellipseEq = x*x + y*y;

                if (ellipseEq > 1.0)
                    discard;

                FragColor = vec4(fs_in.color, 1.0);
            }");
        _vaoCircle = new([3, 2, 1, 3]);

        _shaderCircumference = new(@"
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
            layout(line_strip, max_vertices = 120) out;

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
        _vaoCircumference = new([3, 2, 3]);

        _vaoBackground = new([3, 2, 1, 3]);

        GL.Enable(EnableCap.DepthTest);
        rebuildVBOs();
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
        if (_shaderLine == null || _vaoLine == null || _shaderTriangle == null || _vaoTriangle == null || _shaderCircle == null || _vaoCircle == null || _shaderCircumference == null || _vaoCircumference == null || _vaoBackground == null)
            return;
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderLine.Id);
        _shaderLine.SetUniform("uProjection", _projection);
        _shaderLine.SetUniform("uView", _view);
        GL.BindVertexArray(_vaoLine.Id);
        GL.DrawArrays(PrimitiveType.Lines, 0, _vaoLine.Length / _vaoLine.Stride);

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
        // GL.BindVertexArray(_vaoCircumference.Id);
        // GL.DrawArrays(PrimitiveType.Points, 0, _vaoCircumference.Length / _vaoCircumference.Stride);

        // GL.UseProgram(_shaderCircumference.Id);
        // _shaderCircumference.SetUniform("uProjection", _projection);
        // _shaderCircumference.SetUniform("uView", _view);
        // GL.BindVertexArray(_vaoCircumference.Id);
        // GL.DrawArrays(PrimitiveType.Points, 0, _vaoCircumference.Length / _vaoCircumference.Stride);
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