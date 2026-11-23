Shader "Shader Graphs/Limb" {
	Properties {
		[NoScaleOffset] _MainTex ("_MainTex", 2D) = "white" {}
		[NoScaleOffset] _NormalMap ("_NormalMap", 2D) = "white" {}
		_SkinDamage ("SkinDamage", Float) = 0
		_RandomSeed ("RandomSeed", Float) = 0
		_BloodDark ("BloodDark", Vector) = (0.6509434,0.3803399,0.08904416,0)
		_BloodLight ("BloodLight", Vector) = (1,0.801364,0.0990566,0)
		_MuscleDamage ("MuscleDamage", Float) = 0
		_InfectionPercent ("InfectionPercent", Float) = 0
		_BloodOverlay ("BloodOverlay", Float) = 0
		_SnowAmount ("SnowAmount", Float) = 0
		_Dirtyness ("Dirtyness", Float) = 0
		_Wetness ("Wetness", Float) = 1
		_Pain ("Pain", Float) = 0
		[NoScaleOffset] _OverlayTex ("OverlayTex", 2D) = "white" {}
		_OverlayVis ("OverlayVis", Float) = 0
		[NoScaleOffset] _SampleTexture2D_e790f639f30e4348add2107bda1c0181_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_722a3646334c45d58299ef3dd9fd21be_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_2b1c14afc7804182bca65b5e1d40c654_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_b18c07c6f1ad40fbb83aa524debfc19c_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_7c98fa358cda470586a331202c5ca577_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}