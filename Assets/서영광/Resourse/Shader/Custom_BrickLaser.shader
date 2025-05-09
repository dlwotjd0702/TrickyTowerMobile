//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Custom/BrickLaser" {
Properties {
_Color ("Main Color", Color) = (1,1,1,1)
_EdgeColor ("Edge Color", Color) = (1,1,1,1)
_Cutoff ("Cutoff", Float) = 1
_NoiseTex ("Noise Texture", 2D) = "white" { }
_MainTex ("Sprite Texture", 2D) = "white" { }
[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
}
SubShader {
 Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 Pass {
  Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
  Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
  ZClip Off
  ZWrite Off
  Cull Off
  GpuProgramID 9120
Program "vp" {
SubProgram "d3d9 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "PIXELSNAP_ON" }
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d9 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "PIXELSNAP_ON" }
"// shader disassembly not supported on DXBC"
}
}
}
}
Fallback "Diffuse"
}