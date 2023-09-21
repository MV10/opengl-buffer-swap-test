# opengl-buffer-swap-test

Testing Shadertoy-style buffer swapping, where previous-frame buffer content is used to produce feedback effects.

For testing, this is an implementation of the [HSL Cellular Automata](https://www.shadertoy.com/view/wddGWM) shader. However, others like [isovalues 3](https://www.shadertoy.com/view/ldfczS) don't work for reasons I don't yet understand.

* `FrameData` provides a full-viewport quad (ie. draw all pixels) to the vertex shader
* `BufferInfo` stores a related set of Framebuffer and Texture handles and a TextureUnit
* `RenderPass` stores `BufferInfo` objects, the current-frame draw buffer and the last frame's old buffer

 ![Untitled](https://github.com/MV10/opengl-buffer-swap-test/assets/794270/22c6bdf4-0ad1-477d-ba9b-e8a9eed3e9eb)

