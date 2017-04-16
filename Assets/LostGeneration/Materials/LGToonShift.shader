// Shader created with Shader Forge v1.34 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.34;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4866,x:32719,y:32712,varname:node_4866,prsc:2|custl-5108-OUT,clip-8379-A,olwid-2268-OUT,olcol-6302-RGB;n:type:ShaderForge.SFN_Tex2d,id:8379,x:30141,y:32587,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_8379,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:31ec5b52797e690439ca0daa45842b59,ntxv:0,isnm:False|UVIN-9752-OUT;n:type:ShaderForge.SFN_Vector4Property,id:8436,x:29287,y:32495,ptovrint:False,ptlb:UVOffset,ptin:_UVOffset,varname:node_8436,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_TexCoord,id:9989,x:29557,y:32546,varname:node_9989,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Append,id:9854,x:29557,y:32393,varname:node_9854,prsc:2|A-8436-X,B-8436-Y;n:type:ShaderForge.SFN_Append,id:1091,x:29557,y:32698,varname:node_1091,prsc:2|A-8436-Z,B-8436-W;n:type:ShaderForge.SFN_Multiply,id:1183,x:29781,y:32676,varname:node_1183,prsc:2|A-9989-UVOUT,B-1091-OUT;n:type:ShaderForge.SFN_Add,id:9752,x:29953,y:32587,varname:node_9752,prsc:2|A-9854-OUT,B-1183-OUT;n:type:ShaderForge.SFN_Color,id:7196,x:30210,y:32391,ptovrint:False,ptlb:Dark Color,ptin:_DarkColor,varname:node_7196,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1570069,c2:0.1788271,c3:0.3235294,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:71,x:32211,y:33119,ptovrint:False,ptlb:Outline Coefficient,ptin:_OutlineCoefficient,varname:node_71,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;n:type:ShaderForge.SFN_VertexColor,id:8146,x:32211,y:32976,varname:node_8146,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2268,x:32412,y:33031,varname:node_2268,prsc:2|A-8146-R,B-71-OUT;n:type:ShaderForge.SFN_Color,id:6302,x:32412,y:33196,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_6302,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:3837,x:30983,y:32159,varname:node_3837,prsc:2|A-3039-OUT,B-8379-R;n:type:ShaderForge.SFN_Vector1,id:3039,x:30795,y:32139,varname:node_3039,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp01,id:7948,x:31181,y:32096,varname:node_7948,prsc:2|IN-3837-OUT;n:type:ShaderForge.SFN_Lerp,id:630,x:31404,y:31945,varname:node_630,prsc:2|A-99-OUT,B-5169-RGB,T-7948-OUT;n:type:ShaderForge.SFN_Color,id:5169,x:30983,y:32015,ptovrint:False,ptlb:Color R1,ptin:_ColorR1,varname:node_5169,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6617647,c2:0.175173,c3:0.4872629,c4:1;n:type:ShaderForge.SFN_Vector1,id:425,x:30983,y:32298,varname:node_425,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:1890,x:31181,y:32221,varname:node_1890,prsc:2|A-3837-OUT,B-425-OUT;n:type:ShaderForge.SFN_Color,id:5559,x:31404,y:32096,ptovrint:False,ptlb:Color R2,ptin:_ColorR2,varname:node_5559,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.007352948,c3:0.3975661,c4:1;n:type:ShaderForge.SFN_Lerp,id:7177,x:31715,y:32185,varname:node_7177,prsc:2|A-630-OUT,B-5559-RGB,T-1381-OUT;n:type:ShaderForge.SFN_Multiply,id:62,x:30977,y:32627,varname:node_62,prsc:2|A-9472-OUT,B-8379-G;n:type:ShaderForge.SFN_Vector1,id:9472,x:30801,y:32592,varname:node_9472,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp01,id:6266,x:31175,y:32564,varname:node_6266,prsc:2|IN-62-OUT;n:type:ShaderForge.SFN_Lerp,id:4302,x:31398,y:32389,varname:node_4302,prsc:2|A-3667-OUT,B-2620-RGB,T-6266-OUT;n:type:ShaderForge.SFN_Color,id:2620,x:30965,y:32485,ptovrint:False,ptlb:Color G1,ptin:_ColorG1,varname:_ColorR2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:4098,x:30977,y:32766,varname:node_4098,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:5982,x:31175,y:32689,varname:node_5982,prsc:2|A-62-OUT,B-4098-OUT;n:type:ShaderForge.SFN_Color,id:9457,x:31398,y:32564,ptovrint:False,ptlb:Color G2,ptin:_ColorG2,varname:_ColorR3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:9240,x:31709,y:32653,varname:node_9240,prsc:2|A-4302-OUT,B-9457-RGB,T-7880-OUT;n:type:ShaderForge.SFN_Multiply,id:9366,x:30974,y:33072,varname:node_9366,prsc:2|A-1801-OUT,B-8379-B;n:type:ShaderForge.SFN_Vector1,id:1801,x:30807,y:33015,varname:node_1801,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp01,id:3848,x:31172,y:33009,varname:node_3848,prsc:2|IN-9366-OUT;n:type:ShaderForge.SFN_Lerp,id:7310,x:31395,y:32858,varname:node_7310,prsc:2|A-2308-OUT,B-324-RGB,T-3848-OUT;n:type:ShaderForge.SFN_Color,id:324,x:30974,y:32923,ptovrint:False,ptlb:Color B1,ptin:_ColorB1,varname:_ColorG2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9053,x:30974,y:33211,varname:node_9053,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:6767,x:31172,y:33134,varname:node_6767,prsc:2|A-9366-OUT,B-9053-OUT;n:type:ShaderForge.SFN_Color,id:7856,x:31395,y:33009,ptovrint:False,ptlb:Color B2,ptin:_ColorB2,varname:_ColorG3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:9515,x:31706,y:33098,varname:node_9515,prsc:2|A-7310-OUT,B-7856-RGB,T-9059-OUT;n:type:ShaderForge.SFN_Add,id:5108,x:32056,y:32667,varname:node_5108,prsc:2|A-7177-OUT,B-9240-OUT,C-9515-OUT;n:type:ShaderForge.SFN_Clamp01,id:1381,x:31398,y:32236,varname:node_1381,prsc:2|IN-1890-OUT;n:type:ShaderForge.SFN_Clamp01,id:7880,x:31395,y:32706,varname:node_7880,prsc:2|IN-5982-OUT;n:type:ShaderForge.SFN_Clamp01,id:9059,x:31395,y:33155,varname:node_9059,prsc:2|IN-6767-OUT;n:type:ShaderForge.SFN_Relay,id:7332,x:30543,y:31894,varname:node_7332,prsc:2|IN-7196-RGB;n:type:ShaderForge.SFN_Relay,id:4421,x:30657,y:32805,varname:node_4421,prsc:2|IN-7196-RGB;n:type:ShaderForge.SFN_Multiply,id:2308,x:30785,y:32842,varname:node_2308,prsc:2|A-4421-OUT,B-8379-B;n:type:ShaderForge.SFN_Multiply,id:3667,x:30747,y:32397,varname:node_3667,prsc:2|A-7196-RGB,B-8379-G;n:type:ShaderForge.SFN_Multiply,id:99,x:30687,y:31894,varname:node_99,prsc:2|A-7332-OUT,B-8379-R;proporder:8379-8436-7196-71-6302-5169-5559-2620-9457-324-7856;pass:END;sub:END;*/

Shader "Lost Generation/LGToonShift" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _UVOffset ("UVOffset", Vector) = (0,0,0,0)
        _DarkColor ("Dark Color", Color) = (0.1570069,0.1788271,0.3235294,1)
        _OutlineCoefficient ("Outline Coefficient", Float ) = 0.01
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _ColorR1 ("Color R1", Color) = (0.6617647,0.175173,0.4872629,1)
        _ColorR2 ("Color R2", Color) = (1,0.007352948,0.3975661,1)
        _ColorG1 ("Color G1", Color) = (0.5,0.5,0.5,1)
        _ColorG2 ("Color G2", Color) = (0.5,0.5,0.5,1)
        _ColorB1 ("Color B1", Color) = (0.5,0.5,0.5,1)
        _ColorB2 ("Color B2", Color) = (0.5,0.5,0.5,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _UVOffset;
            uniform float _OutlineCoefficient;
            uniform float4 _OutlineColor;
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
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*(o.vertexColor.r*_OutlineCoefficient),1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_9752 = (float2(_UVOffset.r,_UVOffset.g)+(i.uv0*float2(_UVOffset.b,_UVOffset.a)));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_9752, _Texture));
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _UVOffset;
            uniform float4 _DarkColor;
            uniform float4 _ColorR1;
            uniform float4 _ColorR2;
            uniform float4 _ColorG1;
            uniform float4 _ColorG2;
            uniform float4 _ColorB1;
            uniform float4 _ColorB2;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_9752 = (float2(_UVOffset.r,_UVOffset.g)+(i.uv0*float2(_UVOffset.b,_UVOffset.a)));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_9752, _Texture));
                clip(_Texture_var.a - 0.5);
////// Lighting:
                float node_3837 = (2.0*_Texture_var.r);
                float node_62 = (2.0*_Texture_var.g);
                float node_9366 = (2.0*_Texture_var.b);
                float3 finalColor = (lerp(lerp((_DarkColor.rgb*_Texture_var.r),_ColorR1.rgb,saturate(node_3837)),_ColorR2.rgb,saturate((node_3837-1.0)))+lerp(lerp((_DarkColor.rgb*_Texture_var.g),_ColorG1.rgb,saturate(node_62)),_ColorG2.rgb,saturate((node_62-1.0)))+lerp(lerp((_DarkColor.rgb*_Texture_var.b),_ColorB1.rgb,saturate(node_9366)),_ColorB2.rgb,saturate((node_9366-1.0))));
                fixed4 finalRGBA = fixed4(finalColor,1);
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_9752 = (float2(_UVOffset.r,_UVOffset.g)+(i.uv0*float2(_UVOffset.b,_UVOffset.a)));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_9752, _Texture));
                clip(_Texture_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
