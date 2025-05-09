//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Custom/EndlessRaceClouds" {
Properties {
_MainTex ("Main Texture", 2D) = "white" { }
_Mask1Tex ("Mask 1 Texture", 2D) = "white" { }
_Mask2Tex ("Mask 2 Texture", 2D) = "white" { }
}
SubShader {
 Tags { "CanUseSpriteAtlas" = "true" "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 Pass {
  Tags { "CanUseSpriteAtlas" = "true" "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
  ColorMask RGB 0
  ZClip Off
  ZWrite Off
  Cull Off
  GpuProgramID 60807
Program "vp" {
SubProgram "d3d9 " {
Keywords { "ICE_OFF" "WATER_OFF" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "ICE_OFF" "WATER_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "ICE_ON" "WATER_OFF" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "ICE_ON" "WATER_ON" }
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d9 " {
Keywords { "ICE_OFF" "WATER_OFF" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "ICE_OFF" "WATER_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "ICE_ON" "WATER_OFF" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "ICE_ON" "WATER_ON" }
"// shader disassembly not supported on DXBC"
}
}
}
}
}