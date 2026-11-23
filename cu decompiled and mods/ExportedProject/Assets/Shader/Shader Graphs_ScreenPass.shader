Shader "Shader Graphs/ScreenPass" {
	Properties {
		_Pain ("Pain", Float) = 0
		_BloodAmount ("BloodAmount", Float) = 1
		_Consciousness ("Consciousness", Float) = 0
		_Stamina ("Stamina", Float) = 0
		[NoScaleOffset] _ScreenIce ("ScreenIce", 2D) = "white" {}
		_FrostAmount ("FrostAmount", Float) = 0
		_OverheatAmount ("OverheatAmount", Float) = 0
		_BadFocusAmount ("BadFocusAmount", Float) = 0
		_InsanityFX ("InsanityFX", Float) = 0.65
		_BrightMult ("BrightMult", Float) = 0
		_PlayerPos ("PlayerPos", Vector) = (0,0,0,0)
		_LowOxyAmt ("LowOxyAmt", Float) = 0
		_Dirtyness ("Dirtyness", Float) = 1
		_SaturationMult ("SaturationMult", Float) = 1
		_Brightness ("Brightness", Float) = 0
		_PosterizeSteps ("PosterizeSteps", Float) = 124
		_NightVisAmount ("NightVisAmount", Float) = 0
		[NoScaleOffset] _SampleTexture2D_fc488d78c9744ac29c325d344d9e3721_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_5036a882258e4124b12b278d514899ab_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_f02d86d5227d471aa3b32755fd86122e_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_7c233926384f459ea956215906aed791_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_7208f093b39a4b4bbcd5a9d0c30e47c2_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _SampleTexture2D_6f4676ee6a194673957ea638ac6e3bc9_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.Rendering.Fullscreen.ShaderGraph.FullscreenShaderGUI"
}