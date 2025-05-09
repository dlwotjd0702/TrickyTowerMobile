//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Custom/PostProcessTransition" {
Properties {
_MainTex ("Base (RGB)", 2D) = "white" { }
_WipePosition ("Wipe position", Float) = 0
}
SubShader {
 Pass {
  Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
  ZClip Off
  GpuProgramID 58016
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