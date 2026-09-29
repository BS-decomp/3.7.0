// Block Strike 608 Unity 5.6 port of the stock Unity 4.7 "Mobile/Unlit (Supports Lightmap)".
//
// WHY THIS DIFFERS FROM THE APK COPY
// The Unity 4.7 shader used fixed-function VertexLM (dLDR, "* double") and VertexLMRGBM
// (RGBM, "* texture alpha double ... * quad") passes and let the engine pick one. Unity 5.5/5.6
// pick the RGBM pass on Android/iOS targets for legacy shaders, but the recovered
// LightmapFar-0.png files are dLDR data with no alpha channel, so the scene rendered about
// 4x too bright ("everything blown out" on Android, fine in the editor / Windows target).
//
// FIX
// Both lightmap passes now run the same program, which decodes the lightmap exactly the way the
// 4.7 GLES program in the APK did (tools/shader-extract, VertexLit shaders):
//     rgb = main.rgb * (2.0 * lightmap.rgb)
// so the result no longer depends on which encoding the engine thinks the platform uses.
// The non-lightmapped "Vertex" pass is unchanged. The lightmap textures must therefore be
// imported as a plain (Default) texture - NOT the "Lightmap" import type, which makes Unity
// re-encode the pixels per platform. See docs/android-lightmap-overexposure.md.

Shader "Mobile/Unlit (Supports Lightmap)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
 LOD 100
 Tags { "RenderType"="Opaque" }

 CGINCLUDE
 #include "UnityCG.cginc"

 sampler2D _MainTex;
 float4 _MainTex_ST;

 struct appdata_bs {
  float4 vertex : POSITION;
  float2 texcoord : TEXCOORD0;   // main texture (fixed-function TexCoord1)
  float2 texcoord1 : TEXCOORD1;  // baked atlas UV (fixed-function TexCoord0)
 };

 struct v2f_bs {
  float4 pos : SV_POSITION;
  float2 uv : TEXCOORD0;
  float2 lmuv : TEXCOORD1;
  UNITY_FOG_COORDS(2)
 };

 v2f_bs vert_bs (appdata_bs v)
 {
  v2f_bs o;
  o.pos = UnityObjectToClipPos(v.vertex);
  o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
  o.lmuv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  UNITY_TRANSFER_FOG(o, o.pos);
  return o;
 }

 fixed4 frag_bs (v2f_bs i) : SV_Target
 {
  fixed4 mainTex = tex2D(_MainTex, i.uv);
  // dLDR: stored value is intensity / 2.
  half3 lm = 2.0h * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv).rgb;
  fixed4 c;
  c.rgb = mainTex.rgb * lm;
  c.a = mainTex.a;
  UNITY_APPLY_FOG(i.fogCoord, c);
  return c;
 }
 ENDCG

 Pass {
  Tags { "LIGHTMODE"="Vertex" "RenderType"="Opaque" }
  SetTexture [_MainTex] { combine texture }
 }
 Pass {
  Tags { "LIGHTMODE"="VertexLM" "RenderType"="Opaque" }
  CGPROGRAM
  #pragma vertex vert_bs
  #pragma fragment frag_bs
  #pragma multi_compile_fog
  ENDCG
 }
 // Same program on purpose: whichever lightmap pass the engine selects, the decode is identical.
 Pass {
  Tags { "LIGHTMODE"="VertexLMRGBM" "RenderType"="Opaque" }
  CGPROGRAM
  #pragma vertex vert_bs
  #pragma fragment frag_bs
  #pragma multi_compile_fog
  ENDCG
 }
}
}
