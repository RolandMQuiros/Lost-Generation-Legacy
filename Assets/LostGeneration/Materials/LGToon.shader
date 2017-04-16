// Shader created with Shader Forge v1.34 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.34;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:33452,y:33494,varname:node_9361,prsc:2|emission-1654-OUT,custl-3572-OUT,olwid-1313-OUT,olcol-3093-RGB;n:type:ShaderForge.SFN_LightVector,id:4003,x:30946,y:33775,varname:node_4003,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:5673,x:30946,y:33905,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:2333,x:31222,y:33714,varname:node_2333,prsc:2,dt:0|A-4003-OUT,B-5673-OUT;n:type:ShaderForge.SFN_ArcTan,id:8523,x:31939,y:33865,varname:node_8523,prsc:2|IN-6752-OUT;n:type:ShaderForge.SFN_Slider,id:5326,x:31452,y:33919,ptovrint:False,ptlb:Light hardness,ptin:_Lighthardness,varname:node_6165,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:21.63849,max:150;n:type:ShaderForge.SFN_Multiply,id:6752,x:31778,y:33865,varname:node_6752,prsc:2|A-2333-OUT,B-5326-OUT,C-6420-RGB,D-4764-OUT;n:type:ShaderForge.SFN_Pi,id:1359,x:32152,y:33991,varname:node_1359,prsc:2;n:type:ShaderForge.SFN_Vector1,id:8335,x:31939,y:33991,varname:node_8335,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:2384,x:32119,y:33865,varname:node_2384,prsc:2|A-8523-OUT,B-8335-OUT;n:type:ShaderForge.SFN_Divide,id:4796,x:32275,y:33865,varname:node_4796,prsc:2|A-2384-OUT,B-1359-OUT;n:type:ShaderForge.SFN_Add,id:8885,x:32446,y:33865,varname:node_8885,prsc:2|A-4796-OUT,B-6838-OUT;n:type:ShaderForge.SFN_Vector1,id:6838,x:32275,y:33991,varname:node_6838,prsc:2,v1:1;n:type:ShaderForge.SFN_Divide,id:9311,x:32617,y:33865,varname:node_9311,prsc:2|A-8885-OUT,B-7532-OUT;n:type:ShaderForge.SFN_Vector1,id:7532,x:32446,y:33991,varname:node_7532,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp,id:9743,x:32927,y:33723,varname:node_9743,prsc:2|IN-9311-OUT,MIN-6268-OUT,MAX-108-OUT;n:type:ShaderForge.SFN_Slider,id:6268,x:32586,y:34041,ptovrint:False,ptlb:Light Minimum,ptin:_LightMinimum,varname:node_1507,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:108,x:32586,y:34121,ptovrint:False,ptlb:Light Maximum,ptin:_LightMaximum,varname:_Min_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Color,id:6420,x:31609,y:34011,ptovrint:False,ptlb:Flare,ptin:_Flare,varname:node_1509,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9264706,c2:0.408737,c3:0.408737,c4:1;n:type:ShaderForge.SFN_Color,id:3093,x:33138,y:34136,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_8968,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1737132,c2:0.2236309,c3:0.4632353,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:490,x:32929,y:34064,ptovrint:False,ptlb:Outline Coefficient,ptin:_OutlineCoefficient,varname:node_2016,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_VertexColor,id:138,x:32929,y:33920,cmnt:Red vertex color channel affects line thickness,varname:node_138,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1313,x:33138,y:33947,varname:node_1313,prsc:2|A-138-R,B-490-OUT;n:type:ShaderForge.SFN_VertexColor,id:1321,x:31426,y:34262,varname:node_1321,prsc:2;n:type:ShaderForge.SFN_Subtract,id:4764,x:31625,y:34202,cmnt:Blue vertex color channel can mask vertices from lighting,varname:node_4764,prsc:2|A-2431-OUT,B-1321-B;n:type:ShaderForge.SFN_Vector1,id:2431,x:31426,y:34202,varname:node_2431,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:3572,x:33168,y:33660,varname:node_3572,prsc:2|A-7926-RGB,B-9100-OUT,C-6122-RGB,D-9743-OUT;n:type:ShaderForge.SFN_LightColor,id:6122,x:32535,y:33639,varname:node_6122,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:9100,x:32485,y:33512,varname:node_9100,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1654,x:32781,y:33187,varname:node_1654,prsc:2|A-3610-RGB,B-7926-RGB;n:type:ShaderForge.SFN_AmbientLight,id:3610,x:32470,y:33125,varname:node_3610,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:7926,x:32262,y:33319,ptovrint:False,ptlb:Diffuse Texture,ptin:_DiffuseTexture,varname:node_7926,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-127-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:127,x:32028,y:33319,varname:node_127,prsc:2,uv:0,uaff:False;proporder:490-3093-5326-6268-108-6420-7926;pass:END;sub:END;*/

Shader "Lost Generation/LGToon" {
    Properties {
        _OutlineCoefficient ("Outline Coefficient", Float ) = 0.1
        _OutlineColor ("Outline Color", Color) = (0.1737132,0.2236309,0.4632353,1)
        _Lighthardness ("Light hardness", Range(0, 150)) = 21.63849
        _LightMinimum ("Light Minimum", Range(0, 1)) = 0
        _LightMaximum ("Light Maximum", Range(0, 1)) = 1
        _Flare ("Flare", Color) = (0.9264706,0.408737,0.408737,1)
        _DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _OutlineColor;
            uniform float _OutlineCoefficient;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(0)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.normal*(o.vertexColor.r*_OutlineCoefficient),1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
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
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _Lighthardness;
            uniform float _LightMinimum;
            uniform float _LightMaximum;
            uniform float4 _Flare;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
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
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                float3 emissive = (UNITY_LIGHTMODEL_AMBIENT.rgb*_DiffuseTexture_var.rgb);
                float3 finalColor = emissive + (_DiffuseTexture_var.rgb*attenuation*_LightColor0.rgb*clamp(((((atan((dot(lightDirection,normalDirection)*_Lighthardness*_Flare.rgb*(1.0-i.vertexColor.b)))*2.0)/3.141592654)+1.0)/2.0),_LightMinimum,_LightMaximum));
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
            uniform float _Lighthardness;
            uniform float _LightMinimum;
            uniform float _LightMaximum;
            uniform float4 _Flare;
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
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
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                float3 finalColor = (_DiffuseTexture_var.rgb*attenuation*_LightColor0.rgb*clamp(((((atan((dot(lightDirection,normalDirection)*_Lighthardness*_Flare.rgb*(1.0-i.vertexColor.b)))*2.0)/3.141592654)+1.0)/2.0),_LightMinimum,_LightMaximum));
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
