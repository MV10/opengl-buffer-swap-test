# opengl-buffer-swap-test

Testing Shadertoy-style buffer swapping, where previous-frame buffer content is used to produce feedback effects.

For testing, this is an implementation of the [isovalues 3](https://www.shadertoy.com/view/ldfczS) shader.

* `FrameData` provides a full-viewport quad (ie. draw all pixels) to the vertex shader
* `BufferInfo` stores a related set of Framebuffer and Texture handles and a TextureUnit
* `RenderPass` stores `BufferInfo` objects, the current-frame draw buffer and the last frame's old buffer

 
