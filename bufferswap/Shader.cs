
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace bufferswap;

// mostly borrowed from https://github.com/MV10/eyecandy/blob/master/eyecandy/Visual/Shader.cs

public class Shader : IDisposable
{
    public static int TexUnitToOrdinal(TextureUnit unit)
        => (int)unit - (int)TextureUnit.Texture0;

    public static TextureUnit OrdinalToTexUnit(int ordinal)
        => (TextureUnit)(ordinal + (int)TextureUnit.Texture0);

    public int Handle;

    public Dictionary<string, int> UniformLocations = new();

    public Shader(string vertexPathname, string fragmentPathname)
    {
        int VertexShader = 0;
        int FragmentShader = 0;

        try
        {
            string VertexShaderSource = File.ReadAllText(vertexPathname);
            string FragmentShaderSource = File.ReadAllText(fragmentPathname);
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
        }
        catch (Exception ex)
        {
            Program.Fail($"Loading shader source {ex.GetType()}: {ex.Message}");
        }

        try
        {
            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int vertOk);
            if (vertOk == 0) Program.Fail($"Compiling vert: {GL.GetShaderInfoLog(VertexShader)}");

            GL.CompileShader(FragmentShader);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int fragOk);
            if (fragOk == 0) Program.Fail($"Compiling frag: {GL.GetShaderInfoLog(FragmentShader)}");
        }
        catch (Exception ex)
        {
            Program.Fail($"Compiling shaders {ex.GetType()}: {ex.Message}");
        }

        try
        {
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);
            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkOk);
            if (linkOk == 0) Program.Fail($"Linking shaders: {GL.GetProgramInfoLog(Handle)}");
        }
        catch (Exception ex)
        {
            Program.Fail($"Linking shaders {ex.GetType()}: {ex.Message}");
        }

        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);

        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var uniformCount);
        for (var i = 0; i < uniformCount; i++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);
            var location = GL.GetUniformLocation(Handle, key);
            UniformLocations.Add(key, location);
        }
    }

    public void Use()
    {
        if (IsDisposed) Program.Fail("Shader.Use called after disposal");
        GL.UseProgram(Handle);
    }

    public int GetAttribLocation(string attribName)
    {
        Use();
        return GL.GetAttribLocation(Handle, attribName);
    }

    public void SetTexture(string name, int handle, TextureUnit unit)
        => SetTexture(name, handle, TexUnitToOrdinal(unit));

    public void SetTexture(string name, int handle, int unit)
    {
        if (!UniformLocations.ContainsKey(name)) return;

        Use();

        GL.ActiveTexture(OrdinalToTexUnit(unit));
        GL.BindTexture(TextureTarget.Texture2D, handle);
        GL.Uniform1(UniformLocations[name], unit);
    }

    public void SetUniform(string name, int data)
    {
        if (!UniformLocations.ContainsKey(name)) return;

        Use();
        GL.Uniform1(UniformLocations[name], data);
    }

    public void SetUniform(string name, float data)
    {
        if (!UniformLocations.ContainsKey(name)) return;

        Use();
        GL.Uniform1(UniformLocations[name], data);
    }

    public void SetUniform(string name, Matrix4 data)
    {
        if (!UniformLocations.ContainsKey(name)) return;

        Use();
        GL.UniformMatrix4(UniformLocations[name], transpose: true, ref data);
    }

    public void SetUniform(string name, Vector2 data)
    {
        if (!UniformLocations.ContainsKey(name)) return;

        Use();
        GL.Uniform2(UniformLocations[name], data);
    }

    public void SetUniform(string name, Vector3 data)
    {
        if (!UniformLocations.ContainsKey(name)) return;

        Use();
        GL.Uniform3(UniformLocations[name], data);
    }

    public void Dispose()
    {
        if (IsDisposed) return;

        GL.DeleteProgram(Handle);

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
    private bool IsDisposed = false;
}
