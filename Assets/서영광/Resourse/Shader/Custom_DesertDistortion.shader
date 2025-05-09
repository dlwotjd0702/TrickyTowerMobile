//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Custom/DesertDistortion" {
Properties {
_MainTex ("Texture", 2D) = "white" { }
_DistortionMap ("Texture", 2D) = "white" { }
}
SubShader {
 Pass {
  ZClip Off
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 30841
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