Shader "Custom/ThruImage" {
    Properties {
        _MainTex ("Base", 2D) = "white" {}
    }
    
    CGINCLUDE   
    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : POSITION;
        float2 uv : TEXCOORD0;
    };

    sampler2D _MainTex;

    v2f vert (appdata_img v) {
        v2f o;
        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        o.uv = v.texcoord.xy;
        return o;
    }

    half4 frag (v2f i) : COLOR
    {
        return tex2D (_MainTex, i.uv);
    }
    ENDCG
    
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest 
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
