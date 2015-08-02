Shader "Gameplay/SandingShader" 
{
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SandingRenderTex ("Sanding Render Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,0.5)) = 0.5
		_SandingStrength ("Sanding Strength", Range(1,5)) = 2
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf StandardSpecular fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SandingRenderTex;
		half _Glossiness;
		half _SandingStrength;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_SandingRenderTex;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			fixed4 sanding = tex2D (_SandingRenderTex, IN.uv_SandingRenderTex);
			half sandingValue = (sanding.r + sanding.g + sanding.b)/3.0f;
			half invertSandingValue = 1 - sandingValue;

			fixed4 main = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = main.rgb + (main.rgb * (invertSandingValue / _SandingStrength));
			o.Alpha = main.a;

			o.Smoothness = _Glossiness * sandingValue;
		}


		ENDCG
	} 
	FallBack "Diffuse"
}
