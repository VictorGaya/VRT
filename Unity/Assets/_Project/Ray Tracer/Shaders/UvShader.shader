// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UvShader"
{
    Properties
    {
        [Slider(3.0)]
        _WireframeWidth ("Wireframe width", Range(0., 0.2)) = 0.05
        _WireframeColor ("Color", color) = (1., 0., 0., 1.)
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent-1" "IgnoreProjector" = "True" }
        Pass {
            Tags{ "Queue" = "Opaque" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 vert(appdata_base v) : SV_POSITION {
                return UnityObjectToClipPos (v.texcoord);
            }

            fixed4 frag() : SV_Target {
                return fixed4(0.3,0.3,0.3,1.0);
            }
            ENDCG
        }

        Pass
        {
            Name "Wireframe"
            Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            // Stencil {
            //     Ref 1
            //     Comp NotEqual
            // }
            Blend SrcAlpha OneMinusSrcAlpha
            // ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #pragma multi_compile_fragment VERTEX EDGE FACE

            #include "UnityCG.cginc"

            // struct v2g {
            //     float4 worldPos : SV_POSITION;
            // };

            struct g2f {
                float4 pos : SV_POSITION;
                float3 bary : TEXCOORD1;
            };

            // v2g vert(appdata_base v) {
            //     v2g o;
            //     o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            //     return o;
            // }

            float4 vert(appdata_base v) : SV_POSITION {
                return UnityObjectToClipPos(v.vertex);
            }

            [maxvertexcount(3)]
            void geom(triangle float4 input[3] : SV_POSITION, inout TriangleStream<g2f> triStream) {
                g2f o;
                // for (int i = 0; i < 3; i++) {
                //     o.pos = input[i];
                //     o.bary = float3(0., 0., 0.);
                //     o.bary[i] = 1.0;
                //     triStream.Append(o);
                // }
                o.pos = mul(UNITY_MATRIX_VP, input[0]);
                o.bary = float3(1., 0., 0.);
                triStream.Append(o);
                o.pos = mul(UNITY_MATRIX_VP, input[1]);
                o.bary = float3(0., 0., 1.);
                triStream.Append(o);
                o.pos = mul(UNITY_MATRIX_VP, input[2]);
                o.bary = float3(0., 1., 0.);
                triStream.Append(o);
            }

            uniform float _WireframeWidth;
            uniform fixed4 _WireframeColor;

            fixed4 frag(g2f i) : SV_Target {
#if defined (VERTEX)
                    if(!any(bool3(i.bary.x > 1-_WireframeWidth, i.bary.y > 1-_WireframeWidth, i.bary.z > 1-_WireframeWidth)))
                        discard;
#elif defined (EDGE)
                    if(!any(bool3(i.bary.x < _WireframeWidth, i.bary.y < _WireframeWidth, i.bary.z < _WireframeWidth)))
                        discard;
// #elif defined (FACE) 
#else
                    float3 dist = abs(i.bary - float3(1./3., 1./3., 1./3.));
                    float maxDist = max(max(dist.x, dist.y), dist.z);
                    if(maxDist > _WireframeWidth)
                        discard;
// #else
//                     discard;
#endif
                return _WireframeColor;
            }

            ENDCG
        }
    }
}