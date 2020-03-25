Shader "Unlit/Character_Shader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Intensity("_Intensity", Float) = 10
		_EdgeColor("_EdgeColor", Color) = (1,0,0.7064714,0)
		_FresnelPower("_FresnelPower", Float) = 5
		_FresnelColor("_FresnelColor", Color) = (1,1,1,0)

		/* Dissolve effect Properties */
		_DissolveTex ("Dissolve Texture", 2D) = "white" {}
		_DissolveBorderColour1 ("Edge colour 1", Color) = (1,1,1,1)
		_DissolveBorderColour2 ("Edge colour 2", Color) = (1,1,1,1)
		_DissolveAmount ("Dissolution level", Range (0, 1)) = 0.1
		_DissolveTexWidth ("Edge width", Range (0.0, 1.0)) = 0.025
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off // Make double sided
        	Lighting Off
        	ZWrite Off
        	Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Intensity;
			fixed4 _EdgeColor;

			/* Dissolve properties initialization */
			sampler2D _DissolveTex;
			float4 _DissolveBorderColour1;
			float4 _DissolveBorderColour2;
			float _DissolveAmount;
			float _DissolveTexWidth;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture(s)
				fixed4 col = tex2D(_MainTex, i.uv);
				float cutout = tex2D(_DissolveTex, i.uv).r;

				col *= _EdgeColor;
				col *= _Intensity;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				if (cutout < _DissolveAmount)
					discard;

				if (cutout < col.a && cutout < _DissolveAmount + _DissolveTexWidth)
					col = lerp(_DissolveBorderColour1, _DissolveBorderColour2, (cutout - _DissolveAmount) / _DissolveTexWidth);

				return col;
			}
			ENDCG
		}
	}
}
