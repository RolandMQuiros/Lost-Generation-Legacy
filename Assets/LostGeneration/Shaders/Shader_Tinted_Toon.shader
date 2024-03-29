// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "Lost Generation/Tinted Toon" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _UVOffset ("UVOffset", Vector) = (0,0,0,0)
        _Smoothing ("Smoothing", Range(0, 1)) = 0.0
        _OutlineCoefficient ("Outline Coefficient", Float ) = 0.01
        _OutlineDarkening ("Outline Darkening", Range(0, 1)) = 0.6
        _TintRed ("Red Tint", Color) = (1, 0, 0, 1)
        _TintGreen ("Color G1", Color) = (0, 1, 0, 1)
        _TintBlue ("Color B1", Color) = (0, 0, 1, 1)
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        LOD 100
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _UVOffset;
            uniform float _Smoothing;
            uniform float4 _DarkColor;
            uniform float _OutlineCoefficient;
            uniform float _OutlineDarkening;
            uniform float4 _TintRed;
            uniform float4 _ColorR2;
            uniform float4 _TintGreen;
            uniform float4 _ColorG2;
            uniform float4 _TintBlue;
            uniform float4 _ColorB2;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( float4(v.vertex.xyz + v.normal*(o.vertexColor.r*_OutlineCoefficient),1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float2 uvTransform = (float2(_UVOffset.r,_UVOffset.g)+(i.uv0*float2(_UVOffset.b,_UVOffset.a)));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(uvTransform, _Texture));
                clip(_Texture_var.a - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                
                float3 redTint = lerp(0, _TintRed.rgb, _Texture_var.r);
                float3 greenTint = lerp(0, _TintGreen.rgb, _Texture_var.g);
                float3 blueTint = lerp(0, _TintBlue.rgb, _Texture_var.b);
                float3 tinting = redTint + blueTint + greenTint;

                float up = saturate(i.normalDir.y);
                float down = saturate(-i.normalDir.y);
                float middle = 1 - abs(i.normalDir.y);
                float3 ambient = unity_AmbientSky.rgb * up + unity_AmbientGround * down + unity_AmbientEquator * middle;

                float nDotL = -smoothstep(0, _Smoothing, dot(lightDirection, i.normalDir));
                float3 emissive = ambient * tinting;
                float3 tintAndLight = (tinting * nDotL * (_LightColor0.rgb));
                
                float3 finalRGB = emissive * (-nDotL + 1.0) + tintAndLight;
                return fixed4(_OutlineDarkening * finalRGB, 0);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _UVOffset;
            uniform float _Smoothing;
            uniform float4 _DarkColor;
            uniform float4 _TintRed;
            uniform float4 _ColorR2;
            uniform float4 _TintGreen;
            uniform float4 _ColorG2;
            uniform float4 _TintBlue;
            uniform float4 _ColorB2;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float2 uvTransform = (float2(_UVOffset.r,_UVOffset.g)+(i.uv0*float2(_UVOffset.b,_UVOffset.a)));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(uvTransform, _Texture));
                clip(_Texture_var.a - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:

                float3 redTint = lerp(0, _TintRed.rgb, _Texture_var.r);
                float3 greenTint = lerp(0, _TintGreen.rgb, _Texture_var.g);
                float3 blueTint = lerp(0, _TintBlue.rgb, _Texture_var.b);
                float3 tinting = redTint + blueTint + greenTint;

                float up = saturate(i.normalDir.y);
                float down = saturate(-i.normalDir.y);
                float middle = 1 - abs(i.normalDir.y);
                float3 ambient = unity_AmbientSky.rgb * up + unity_AmbientGround * down + unity_AmbientEquator * middle;

                float nDotL = smoothstep(0, _Smoothing, dot(lightDirection,i.normalDir));
                float3 emissive = ambient * tinting;
                float3 tintAndLight = (tinting * nDotL * (_LightColor0.rgb));

                float3 finalColor = (emissive * (-nDotL + 1.0)) + tintAndLight;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _UVOffset;
            uniform float _Smoothing;
            uniform float4 _DarkColor;
            uniform float4 _TintRed;
            uniform float4 _ColorR2;
            uniform float4 _TintGreen;
            uniform float4 _ColorG2;
            uniform float4 _TintBlue;
            uniform float4 _ColorB2;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float2 uvTransform = (float2(_UVOffset.r,_UVOffset.g)+(i.uv0*float2(_UVOffset.b,_UVOffset.a)));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(uvTransform, _Texture));
                clip(_Texture_var.a - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);

                float3 redTint = lerp(0, _TintRed.rgb, _Texture_var.r);
                float3 greenTint = lerp(0, _TintGreen.rgb, _Texture_var.g);
                float3 blueTint = lerp(0, _TintBlue.rgb, _Texture_var.b);
                float3 tinting = redTint + greenTint + blueTint;
                
                float nDotL = dot(lightDirection,i.normalDir);
                float3 tintAndLight = (tinting*smoothstep(0, _Smoothing, nDotL)*(_LightColor0.rgb*attenuation));
                float3 finalColor = tintAndLight;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _UVOffset;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 uvTransform = (float2(_UVOffset.r,_UVOffset.g)+(i.uv0*float2(_UVOffset.b,_UVOffset.a)));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(uvTransform, _Texture));
                clip(_Texture_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
