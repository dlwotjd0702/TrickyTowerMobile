//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "UI/ModeMenuMask" {
Properties {
_MainTex ("Sprite Texture", 2D) = "white" { }
_RampSize ("Ramp Size", Float) = 0.1
_EdgePosition ("Edge Position", Float) = 0.3
_YOffset ("Y Offset", Float) = 0.3
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
  Fog {
   Mode Off
  }
  GpuProgramID 33651
Program "vp" {
SubProgram "d3d9 " {
Keywords { "DUMMY" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "PIXELSNAP_ON" }
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d9 " {
Keywords { "DUMMY" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d9 " {
Keywords { "PIXELSNAP_ON" }
"// shader disassembly not supported on DXBC"
}
}
}
}
}