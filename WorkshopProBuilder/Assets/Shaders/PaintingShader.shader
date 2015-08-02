Shader "Gameplay/PaintingShader" 
{
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_PaintRenderTex ("Paint Render Texture", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf StandardSpecular fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _PaintRenderTex;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_PaintRenderTex;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			fixed4 main = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 paint = tex2D (_PaintRenderTex, IN.uv_PaintRenderTex);
			o.Albedo = main.rgb * paint.rgb;
			o.Alpha = main.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

//struct SurfaceOutputStandardSpecular
//{
//    fixed3 Albedo;      // diffuse color
//    fixed3 Specular;    // specular color
//    fixed3 Normal;      // tangent space normal, if written
//    half3 Emission;
//    half Smoothness;    // 0=rough, 1=smooth
//    half Occlusion;     // occlusion (default 1)
//    fixed Alpha;        // alpha for transparencies
//};