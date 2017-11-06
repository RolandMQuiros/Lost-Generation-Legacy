// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32716,y:32712,varname:node_3138,prsc:2|custl-9188-RGB,clip-9188-A,olwid-1597-OUT,olcol-7125-RGB;n:type:ShaderForge.SFN_Color,id:7125,x:32400,y:33189,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_8968,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1917,x:32191,y:33117,ptovrint:False,ptlb:Outline Coefficient,ptin:_OutlineCoefficient,varname:node_2016,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.0008;n:type:ShaderForge.SFN_VertexColor,id:7324,x:32191,y:32973,cmnt:Red vertex color channel affects line thickness,varname:node_7324,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1597,x:32400,y:33000,varname:node_1597,prsc:2|A-7324-R,B-1917-OUT;n:type:ShaderForge.SFN_TexCoord,id:6727,x:31155,y:32642,varname:node_6727,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector4Property,id:7635,x:31018,y:32866,ptovrint:False,ptlb:UVOffset,ptin:_UVOffset,varname:node_7635,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:1,v4:1;n:type:ShaderForge.SFN_Append,id:1994,x:31224,y:32988,varname:node_1994,prsc:2|A-7635-Z,B-7635-W;n:type:ShaderForge.SFN_Add,id:2775,x:31584,y:32642,varname:node_2775,prsc:2|A-9938-OUT,B-3602-OUT;n:type:ShaderForge.SFN_Append,id:3602,x:31224,y:32854,varname:node_3602,prsc:2|A-7635-X,B-7635-Y;n:type:ShaderForge.SFN_Multiply,id:9938,x:31387,y:32642,varname:node_9938,prsc:2|A-6727-UVOUT,B-1994-OUT;n:type:ShaderForge.SFN_Tex2d,id:9188,x:31886,y:32651,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_3437,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2775-OUT;proporder:1917-7125-7635-9188;pass:END;sub:END;*/

Shader "Lost Generation/LGCharacter" {
    Properties {
        _OutlineCoefficient ("Outline Coefficient", Float ) = 0.0008
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _UVOffset ("UVOffset", Vector) = (0,0,1,1)
        _Texture ("Texture", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _OutlineColor;
            uniform float _OutlineCoefficient;
            uniform float4 _UVOffset;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( float4(v.vertex.xyz + v.normal*(o.vertexColor.r*_OutlineCoefficient),1) );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_2775 = ((i.uv0*float2(_UVOffset.b,_UVOffset.a))+float2(_UVOffset.r,_UVOffset.g));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_2775, _Texture));
                clip(_Texture_var.a - 0.5);
                return fixed4(_OutlineColor.rgb,0);
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _UVOffset;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_2775 = ((i.uv0*float2(_UVOffset.b,_UVOffset.a))+float2(_UVOffset.r,_UVOffset.g));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_2775, _Texture));
                clip(_Texture_var.a - 0.5);
////// Lighting:
                float3 finalColor = _Texture_var.rgb;
                return fixed4(finalColor,1);
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _UVOffset;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
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
                float2 node_2775 = ((i.uv0*float2(_UVOffset.b,_UVOffset.a))+float2(_UVOffset.r,_UVOffset.g));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_2775, _Texture));
                clip(_Texture_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
