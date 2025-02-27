namespace ElementalAdventure.Client.Graphics.OpenGL;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Shader : IDisposable {
    private readonly int _id;
    private bool _disposed = false;

    public int Id => _id;

    public Shader(string vert, string frag) {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vert);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexStatus);
        if (vertexStatus != (int)All.True) {
            string log = GL.GetShaderInfoLog(vertexShader);
            GL.DeleteShader(vertexShader);
            throw new Exception($"Error compiling vertex shader: {log}");
        }

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, frag);
        GL.CompileShader(fragmentShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int fragmentStatus);
        if (fragmentStatus != (int)All.True) {
            string log = GL.GetShaderInfoLog(vertexShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            throw new Exception($"Error compiling fragment shader: {log}");
        }

        _id = GL.CreateProgram();
        GL.AttachShader(_id, vertexShader);
        GL.AttachShader(_id, fragmentShader);
        GL.LinkProgram(_id);
        GL.GetProgram(_id, GetProgramParameterName.LinkStatus, out int programStatus);
        if (programStatus != (int)All.True) {
            string log = GL.GetProgramInfoLog(_id);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteProgram(_id);
            throw new Exception($"Error linking shader program: {log}");
        }

        GL.DetachShader(_id, vertexShader);
        GL.DetachShader(_id, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public Shader(string vert, string geom, string frag) {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vert);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexStatus);
        if (vertexStatus != (int)All.True) {
            string log = GL.GetShaderInfoLog(vertexShader);
            GL.DeleteShader(vertexShader);
            throw new Exception($"Error compiling vertex shader: {log}");
        }

        int geometryShader = GL.CreateShader(ShaderType.GeometryShader);
        GL.ShaderSource(geometryShader, geom);
        GL.CompileShader(geometryShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int geometryStatus);
        if (geometryStatus != (int)All.True) {
            string log = GL.GetShaderInfoLog(vertexShader);
            GL.DeleteShader(vertexShader);
            throw new Exception($"Error compiling geometry shader: {log}");
        }

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, frag);
        GL.CompileShader(fragmentShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int fragmentStatus);
        if (fragmentStatus != (int)All.True) {
            string log = GL.GetShaderInfoLog(vertexShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            throw new Exception($"Error compiling fragment shader: {log}");
        }

        _id = GL.CreateProgram();
        GL.AttachShader(_id, vertexShader);
        GL.AttachShader(_id, geometryShader);
        GL.AttachShader(_id, fragmentShader);
        GL.LinkProgram(_id);
        GL.GetProgram(_id, GetProgramParameterName.LinkStatus, out int programStatus);
        if (programStatus != (int)All.True) {
            string log = GL.GetProgramInfoLog(_id);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteProgram(_id);
            throw new Exception($"Error linking shader program: {log}");
        }

        GL.DetachShader(_id, vertexShader);
        GL.DetachShader(_id, geometryShader);
        GL.DetachShader(_id, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(geometryShader);
        GL.DeleteShader(fragmentShader);
    }

    public void SetUniform(string name, Matrix4 value) {
        int location = GL.GetUniformLocation(_id, name);
        if (location == -1)
            throw new ArgumentException($"Uniform {name} not found in shader.");
        GL.UniformMatrix4(location, false, ref value);
    }

    public void Dispose() {
        if (!_disposed) {
            GL.DeleteProgram(_id);
            _disposed = true;
        }
    }
}