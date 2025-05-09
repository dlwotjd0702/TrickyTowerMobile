//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "World/Background/Puzzle/Island" {
Properties {
_Color ("Main Color", Color) = (1,1,1,1)
_SubColor ("Sub Color", Color) = (1,1,1,1)
_OutlineColorTop ("Outline Color Top", Color) = (1,1,1,1)
_OutlineColorBottom ("Outline Color Bottom", Color) = (1,1,1,1)
_MainTex ("Sprite Texture", 2D) = "white" { }
}
SubShader {
 Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 Pass {
  Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
  Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
  ZClip Off
  ZWrite Off
  Cull Off
  GpuProgramID 57383
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