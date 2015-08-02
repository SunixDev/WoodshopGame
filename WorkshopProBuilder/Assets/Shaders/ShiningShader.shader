Shader "Gameplay/ShiningShader" {
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_LacquerRenderTex ("Lacquer Render Texture", 2D) = "white" {}
		_Glossiness ("Shine", Range(0,1)) = 1
		_LacquerStrength ("Lacquer Strength", Range(2,5)) = 2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf StandardSpecular fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _LacquerRenderTex;
		half _Glossiness;
		half _LacquerStrength;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_LacquerRenderTex;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			fixed4 main = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 lacquer = tex2D (_LacquerRenderTex, IN.uv_LacquerRenderTex);

			half lacquerAmount = (lacquer.r + lacquer.g + lacquer.b) / 3.0f;
			half invertLacquerAmount = 1 - lacquerAmount;

			o.Albedo = main.rgb - ((main.rgb * invertLacquerAmount) / _LacquerStrength);
			o.Alpha = main.a;
			o.Smoothness = _Glossiness * invertLacquerAmount;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}



//struct SurfaceOutput
//{
//    fixed3 Albedo;  // diffuse color
//    fixed3 Normal;  // tangent space normal, if written
//    fixed3 Emission;
//    half Specular;  // specular power in 0..1 range
//    fixed Gloss;    // specular intensity
//    fixed Alpha;    // alpha for transparencies
//};
