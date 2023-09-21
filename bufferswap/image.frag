#version 460

in vec2 fragCoord;
uniform vec2 iResolution;
uniform sampler2D iChannel0;
out vec4 fragColor;

#define fragCoord (fragCoord * iResolution)

void main()
{
	fragColor = texture(iChannel0, fragCoord / iResolution.xy);
}