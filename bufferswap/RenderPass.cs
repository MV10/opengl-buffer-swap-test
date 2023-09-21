
namespace bufferswap;

public class RenderPass
{
    public Shader Shader;
    public BufferInfo DrawBuffer = new();
    public BufferInfo OldBuffer = new();
}
