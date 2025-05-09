//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "TMPro/Bitmap" {
Properties {
_MainTex ("Font Atlas", 2D) = "white" { }
_FaceTex ("Font Texture", 2D) = "white" { }
_FaceColor ("Text Color", Color) = (1,1,1,1)
_VertexOffsetX ("Vertex OffsetX", Float) = 0
_VertexOffsetY ("Vertex OffsetY", Float) = 0
_ClipRect ("Mask Coords", Vector) = (0,0,100000,100000)
_MaskSoftnessX ("Mask SoftnessX", Float) = 0
_MaskSoftnessY ("Mask SoftnessY", Float) = 0
}
SubShader {
 Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 Pass {
  Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
  ZClip Off
  ZTest Off
  ZWrite Off
  Cull Off
  Fog {
   Mode Off
  }
  GpuProgramID 3454
Program "vp" {
SubProgram "d3d9 " {
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d9 " {
"// shader disassembly not supported on DXBC"
}
}
}
}
}