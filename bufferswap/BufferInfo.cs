
using OpenTK.Graphics.OpenGL;

namespace bufferswap;

public class BufferInfo
{
    public static int AllocatedTextureUnitOrdinals = -1;

    public int FramebufferHandle = -1;
    public int TextureHandle = -1;
    public readonly int TextureUnitOrdinal;
    public TextureUnit TextureUnit => Shader.OrdinalToTexUnit(TextureUnitOrdinal);

    public BufferInfo()
    {
        TextureUnitOrdinal = ++AllocatedTextureUnitOrdinals;
    }

}
