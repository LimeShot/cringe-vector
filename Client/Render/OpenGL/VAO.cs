namespace ElementalAdventure.Client.Graphics.OpenGL;

using OpenTK.Graphics.OpenGL4;

public class VAO : IDisposable {
    private readonly int _vao, _vbo;
    private readonly int _stride;
    private readonly List<float> _content;
    private bool _disposed = false;

    public int Id => _vao;
    public int Stride => _stride;
    public float[] Content => _content.ToArray();
    public int Length => _content.Count();

    public VAO(int[] attribs) {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _stride = 0;
        _content = [];

        for (int i = 0; i < attribs.Length; i++) {
            _stride += attribs[i];
        }

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, 0, BufferUsageHint.DynamicDraw);
        int offset = 0;
        for (int i = 0; i < attribs.Length; i++) {
            GL.VertexAttribPointer(i, attribs[i], VertexAttribPointerType.Float, false, _stride * sizeof(float), offset * sizeof(float));
            GL.EnableVertexAttribArray(i);
            offset += attribs[i];
        }
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    private void Commit() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _content.Count * sizeof(float), _content.ToArray(), BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    private void CommitRange(int index, int count) {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, index * sizeof(float), count * sizeof(float), _content.ToArray()[index..(index + count)]);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public (int, int) InsertRange(int index, float[] data) {
        _content.InsertRange(index, data);
        Commit();
        return (index, data.Length);
    }

    public void DeleteRange(int index, int count) {
        if (index + count > _content.Count())
            throw new ArgumentException("Index out of bounds of allocated buffer!");
        _content.RemoveRange(index, count);
        Commit();
    }

    public (int, int) ReplaceRange(int index, int count, float[] data) {
        if (index + count > _content.Count())
            throw new ArgumentException("Index out of bounds of allocated buffer!");
        _content.RemoveRange(index, count);
        _content.InsertRange(index, data);
        Commit();
        return (index, data.Length);
    }

    public (int, int) Append(float[] data) {
        return InsertRange(_content.Count(), data);
    }

    public void Dispose() {
        if (!_disposed) {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            _disposed = true;
        }
    }
}