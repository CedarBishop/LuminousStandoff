Shader "Custom/MyFirstShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Color) = (1,1,1,1)
        _Normal ("Normal", Color) = (1,1,1,1)
    }

    SubShader
    {
    	CGPROGRAM
        #pragma surface surf Lambert

		struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        fixed4 _Emission;
        fixed4 _Normal;

        void surf (Input IN, inout SurfaceOutput o)
        {
        	o.Albedo = _Color.rgb;
        	o.Emission = _Emission.rgb;
        	o.Normal = _Normal.rgb;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
