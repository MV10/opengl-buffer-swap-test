﻿#version 460

in vec2 fragCoord;
uniform float iTime;
uniform vec2 iResolution;
uniform sampler2D iChannel0;
out vec4 fragColor;

#define fragCoord (fragCoord * iResolution)

float noise3( vec3 x ) {
    vec3 p = floor(x);
    vec3 f = fract(x);

    f = f * f * (3. - 2. * f);  // or smoothstep     // to make derivative continuous at borders

    #define hash3(p) fract(sin(1e3 * dot(p, vec3(1, 57, -13.7))) * 4375.5453)   // rand
    
    return mix( mix(mix( hash3(p+vec3(0,0,0)), hash3(p+vec3(1,0,0)),f.x),       // triilinear interp
                    mix( hash3(p+vec3(0,1,0)), hash3(p+vec3(1,1,0)),f.x),f.y),
                mix(mix( hash3(p+vec3(0,0,1)), hash3(p+vec3(1,0,1)),f.x),       
                    mix( hash3(p+vec3(0,1,1)), hash3(p+vec3(1,1,1)),f.x),f.y), f.z);
}

#define noise(x) (noise3(x)+noise3(x+11.5)) / 2. // pseudoperlin improvement from foxes idea 

void main()
{ 
    vec2 R = iResolution.xy;
    float n = noise(vec3(fragCoord * 8. / R.y, .1 * iTime));
    float v = sin(6.28 * 10. * n);
    float t = iTime;
    
    v = smoothstep(1.,0., .5*abs(v)/fwidth(v));
    
	fragColor = mix( exp(-33. / R.y ) * texture( iChannel0, (fragCoord + vec2(1, sin(t))) / R), // .97
               .5 + .5 * sin(12. * n + vec4(0, 2.1, -2.1, 0)),
               v);
}