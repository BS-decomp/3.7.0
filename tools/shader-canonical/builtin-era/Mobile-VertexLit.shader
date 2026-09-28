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

	// Lightmapped, encoded as dLDR
	Pass {
		Tags { "LightMode" = "VertexLM" }

		BindChannels {
			Bind "vertex", Vertex
			Bind "normal", Normal
			Bind "texcoord1", TexCoord0
			Bind "texcoord", TexCoord1
		}

		SetTexture [unity_Lightmap] {
			Matrix [unity_LightmapMatrix]
			combine texture
		}
		SetTexture [_MainTex] {
			combine texture * previous double, texture alpha * primary alpha
		}
	}

	// Lightmapped, encoded as RGBM
	Pass {
		Tags { "LightMode" = "VertexLMRGBM" }

		BindChannels {
			Bind "vertex", Vertex
			Bind "normal", Normal
			Bind "texcoord1", TexCoord0
			Bind "texcoord", TexCoord1
		}

		SetTexture [unity_Lightmap] {
			Matrix [unity_LightmapMatrix]
			combine texture * texture alpha double
		}
		SetTexture [_MainTex] {
			combine texture * previous quad, texture alpha * primary alpha
		}
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
