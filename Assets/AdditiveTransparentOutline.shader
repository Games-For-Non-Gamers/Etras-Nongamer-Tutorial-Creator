Shader "Custom/AdditiveTransparentOutline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }

        SubShader
        {
            Tags {"Queue" = "Transparent" }
            LOD 100

            Pass
            {
                Blend One One
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                float4 _Color;
                sampler2D _MainTex;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = _Color;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    half4 texColor = tex2D(_MainTex, i.uv);

                    // Check if the output color is black
                    if (texColor.r == 0 && texColor.g == 0 && texColor.b == 0)
                    {
                        return texColor; // Return black without any modification
                    }
                    else
                    {
                        half4 col = texColor * i.color;
                        return col;
                    }
                }
                ENDCG
            }
        }
}
