Shader "Custom/VertexColorShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
#pragma fragment frag
# include "UnityCG.cginc"

            struct appdata_t
{
    float4 vertex : POSITION;
                float4 color : COLOR;  // Vertex Color
            };

struct v2f
{
    float4 pos : SV_POSITION;
                float4 color : COLOR;  // Vertex Color
            };

float4 _Color;

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = v.color;  // Vertex Color 전달
    return o;
}

fixed4 frag(v2f i) : SV_Target
            {
                return i.color;  // Vertex Color 출력
            }
            ENDCG
        }
    }
}