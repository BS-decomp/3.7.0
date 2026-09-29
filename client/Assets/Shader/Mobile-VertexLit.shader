// Restored from the Block Strike 608 APK ground truth + era-authentic source.
// See tools/shader-extract/ and docs/shader-lightmap-recovery.md.
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Era-authentic Unity 4.x "Mobile/VertexLit" restored for the Block Strike 608 project.
// Pass-for-pass mirrors the pass set embedded in the 608 APK:
// Vertex (fixed function), VertexLM, VertexLMRGBM (both fixed function, unity_Lightmap),
// and the classic shadow caster pass (Cull Off, Fog Mode Off, Offset 1,1).

Shader "Mobile/VertexLit" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 80

	// Non-lightmapped
	Pass {
		Tags { "LightMode" = "Vertex" }

		Material {
			Diffuse (1,1,1,1)
			Ambient (1,1,1,1)
		}
		Lighting On
		SetTexture [_MainTex] {
			combine texture * primary double, texture alpha * primary alpha
		}
	}

	// Lightmapped. The Unity 4.7 shader had a dLDR pass ("* double") and an RGBM pass ("* quad"),
	// and Unity 5.6 picks the RGBM one on Android although our lightmaps are dLDR (about 4x too
	// bright). Both passes now run one program that decodes dLDR explicitly, exactly like the 4.7
	// GLES code in the APK: rgb = main.rgb * (2.0 * lightmap.rgb). No lighting is applied here,
	// same as the original fixed-function passes. See docs/android-lightmap-overexposure.md.
	Pass {
		Tags { "LightMode" = "VertexLM" }
		CGPROGRAM
		#pragma vertex vert_lm
		#pragma fragment frag_lm
		#pragma multi_compile_fog
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		float4 _MainTex_ST;
		struct appdata_lm {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
		};
		struct v2f_lm {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float2 lmuv : TEXCOORD1;
			UNITY_FOG_COORDS(2)
		};
		v2f_lm vert_lm (appdata_lm v)
		{
			v2f_lm o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.lmuv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}
		fixed4 frag_lm (v2f_lm i) : SV_Target
		{
			fixed4 mainTex = tex2D(_MainTex, i.uv);
			half3 lm = 2.0h * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv).rgb;
			fixed4 c;
			c.rgb = mainTex.rgb * lm;
			c.a = mainTex.a;
			UNITY_APPLY_FOG(i.fogCoord, c);
			return c;
		}
		ENDCG
	}
	Pass {
		Tags { "LightMode" = "VertexLMRGBM" }
		CGPROGRAM
		#pragma vertex vert_lm
		#pragma fragment frag_lm
		#pragma multi_compile_fog
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		float4 _MainTex_ST;
		struct appdata_lm {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
		};
		struct v2f_lm {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float2 lmuv : TEXCOORD1;
			UNITY_FOG_COORDS(2)
		};
		v2f_lm vert_lm (appdata_lm v)
		{
			v2f_lm o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.lmuv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}
		fixed4 frag_lm (v2f_lm i) : SV_Target
		{
			fixed4 mainTex = tex2D(_MainTex, i.uv);
			half3 lm = 2.0h * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv).rgb;
			fixed4 c;
			c.rgb = mainTex.rgb * lm;
			c.a = mainTex.a;
			UNITY_APPLY_FOG(i.fogCoord, c);
			return c;
		}
		ENDCG
	}

	// Pass to render object as a shadow caster
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }

		ZWrite On ZTest LEqual Cull Off
		Fog { Mode Off }
		Offset 1, 1

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_shadowcaster
		#include "UnityCG.cginc"

		struct v2f {
			V2F_SHADOW_CASTER;
		};

		v2f vert( appdata_base v )
		{
			v2f o;
			TRANSFER_SHADOW_CASTER(o)
			return o;
		}

		float4 frag( v2f i ) : COLOR
		{
			SHADOW_CASTER_FRAGMENT(i)
		}
		ENDCG
	}
}
}
