
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace bufferswap;
public class Win : GameWindow, IDisposable
{
    // named the way Shadertoy refers to them
    private RenderPass BufferA = new();
    private RenderPass Image = new();

    private FrameData VertexData = new();
    private Color4 BgColor = new(0, 0, 0, 1);
    private Stopwatch Clock = new();

    Vector2 iResolution;
    float iTime;

    public Win(GameWindowSettings gameWindow, NativeWindowSettings nativeWindow)
        : base(gameWindow, nativeWindow)
    {
        BufferA.Shader = new(".\\passthrough.vert", ".\\bufferA.frag");
        Image.Shader = new(".\\passthrough.vert", ".\\image.frag");
        Clock.Start();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(BgColor);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        // IMPORTANT: As of OpenTK 4.8, WindowState is NOT reliable in
        // this event when the state change was initiated through code.
        // https://github.com/opentk/opentk/issues/1640

        if (WindowState == WindowState.Minimized || e.Width == 0 || e.Height == 0) return;

        AllocateBuffers();

        // any shader instance is adequate because they all have the same vert shader/locations
        VertexData.ViewportChanged(Image.Shader);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        iResolution = new Vector2(ClientSize.X, ClientSize.Y);
        iTime = (float)Clock.Elapsed.TotalSeconds;

        Render(BufferA, BufferA.OldBuffer);
        Render(Image, BufferA.OldBuffer);

        GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, Image.DrawBuffer.FramebufferHandle);
        GL.BlitFramebuffer(
            0, 0, ClientSize.X, ClientSize.Y,
            0, 0, ClientSize.X, ClientSize.Y,
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        SwapBuffers();
        SwapBuffers(BufferA);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyReleased(Keys.Escape))
        {
            Close();
            return;
        }
    }

    private void Render(RenderPass pass, BufferInfo iChannel0)
    {
        pass.Shader.SetUniform("iResolution", iResolution);
        pass.Shader.SetUniform("iTime", iTime);
        pass.Shader.SetTexture("iChannel0", iChannel0.TextureHandle, iChannel0.TextureUnit);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, pass.DrawBuffer.FramebufferHandle);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        VertexData.Draw();

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    private void SwapBuffers(RenderPass pass)
    {
        (pass.DrawBuffer, pass.OldBuffer) = (pass.OldBuffer, pass.DrawBuffer);
    }

    private void AllocateBuffers()
    {
        Console.WriteLine($"Allocating buffers ({ClientSize.X},{ClientSize.Y})");

        DeleteBuffers();
        AllocateBuffers(BufferA.DrawBuffer);
        AllocateBuffers(BufferA.OldBuffer);
        AllocateBuffers(Image.DrawBuffer);
        AllocateBuffers(Image.OldBuffer);
    }

    private void AllocateBuffers(BufferInfo buffer)
    {
        buffer.FramebufferHandle = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer.FramebufferHandle);

        buffer.TextureHandle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, buffer.TextureHandle);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ClientSize.X, ClientSize.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, buffer.TextureHandle, 0);

        var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

        if (!status.Equals(FramebufferErrorCode.FramebufferComplete)
            && !status.Equals(FramebufferErrorCode.FramebufferCompleteExt))
            Program.Fail($"Error creating framebuffer: {status}");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    private void DeleteBuffers()
    {
        DeleteBuffers(BufferA.DrawBuffer);
        DeleteBuffers(BufferA.OldBuffer);
        DeleteBuffers(Image.DrawBuffer);
        DeleteBuffers(Image.OldBuffer);
    }

    private void DeleteBuffers(BufferInfo buffer)
    {
        if (buffer.FramebufferHandle > -1) GL.DeleteFramebuffer(buffer.FramebufferHandle);
        if (buffer.TextureHandle > -1) GL.DeleteTexture(buffer.TextureHandle);
        buffer.FramebufferHandle = -1;
        buffer.TextureHandle = -1;
    }

    public new void Dispose()
    {
        base.Dispose();
        DeleteBuffers();
        VertexData.Dispose();
        BufferA.Shader.Dispose();
        Image.Shader.Dispose();
    }
}
