//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "FMVTexture" {
Properties {
_MainTex ("Luminance Texture", 2D) = "white" { }
_CromaTex ("croma texture", 2D) = "white" { }
}
SubShader {
 Pass {
  ZClip Off
  GpuProgramID 42330
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