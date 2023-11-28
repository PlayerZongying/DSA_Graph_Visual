Shader "Custom/Sh_Outline"
{
    Properties
    {
        _Color("Main Color", Color) = (.5, .5, .5, 1)
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _Outline("Outline width", Range (0.002, 0.03)) = 0.005
        _MainTex("Base (RGB)", 2D) = "white" { }
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            // #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            ENDCG

            SetTexture[_MainTex]
            {
                combine primary
            }
        }
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
        }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        fixed4 _Color;
        fixed4 _OutlineColor;
        float _Outline;
        sampler2D _MainTex;
        

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert(inout appdata_full v)
        {
            v.normal.y = 0;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            // Add the outline
            float4 outline = tex2D(_MainTex, IN.uv_MainTex);
            o.Alpha *= smoothstep(0.5 - _Outline, 0.5, outline.a);
            o.Emission = _OutlineColor.rgb;
        }
        ENDCG
    }
}
