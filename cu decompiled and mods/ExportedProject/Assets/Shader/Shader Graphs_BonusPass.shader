Shader "Shader Graphs/BonusPass" {
	Properties {
		[NoScaleOffset] _BlitTexture ("_BlitTexture", 2D) = "white" {}
		_BlurIntensity ("BlurIntensity", Float) = 1
		_HungerIntensity ("HungerIntensity", Float) = 0
		_WaterAmount ("WaterAmount", Float) = 0
		_DropletAmount ("DropletAmount", Float) = 1
		_DropletNoise ("DropletNoise", Float) = 0
		_DropletColor ("DropletColor", Vector) = (1,1,1,0)
		_SicknessAmount ("SicknessAmount", Float) = 1
		_Insanity ("Insanity", Float) = 0
		[NoScaleOffset] _SampleTexture2D_9ff60a9ed66a4839992ec5a55556a386_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
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