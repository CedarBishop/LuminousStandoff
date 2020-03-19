Shader "Custom/PackedPractice"
{
    Properties
    {
        _myColor ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        CGPROGRAM
		#pragma surface surf Lambert

		struct Input
       	{
       	    float2 uvMainTex;
       	};

       	fixed4 _myColor;

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo.rg = _myColor.xy;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
