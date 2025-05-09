//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "World/Bricks/Default" {
Properties {
_MainTex ("MainTex", 2D) = "white" { }
_PatternTex ("Pattern", 2D) = "white" { }
_HighlightColor ("Highlight Color", Color) = (1,1,1,1)
_ShadowColor ("Shadow Color", Color) = (1,1,1,1)
_BorderColor ("Border Color", Color) = (1,1,1,1)
_Flash ("Flash", Range(0, 1)) = 0
}
SubShader {
 Tags { "CanUseSpriteAtlas" = "true" "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 Pass {
  Tags { "CanUseSpriteAtlas" = "true" "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
  ZClip Off
  ZWrite Off
  Cull Off
  GpuProgramID 36977
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
CustomEditor "BrickMaterialInspector"
}