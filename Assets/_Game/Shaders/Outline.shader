Shader "Spine/Special/Outline" {
   Properties {
      _Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
      [NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
      [Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
      [HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
      [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default
      
      [PerRendererData]_Threshold("threshold", Float) = 0.5
      [PerRendererData] _Outline("Outline", Float) = 0
      [PerRendererData] _OutlineColor("Outline Color", Color) = (1,0,0,1)
   
   }
   CGINCLUDE
   #include "UnityCG.cginc"
   ENDCG

   SubShader {
      Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

      Fog { Mode Off }
      Cull Off
      ZWrite Off
      Blend One OneMinusSrcAlpha
      Lighting Off

      Stencil {
         Ref[_StencilRef]
         Comp[_StencilComp]
         Pass Keep
      }
         
      Pass{
         CGPROGRAM
#pragma vertex vert
#pragma fragment frag

         sampler2D _MainTex;

         fixed4 _MainTex_TexelSize;
         fixed _Threshold;
         fixed _Outline;
         fixed4 _OutlineColor;
         
         struct VertexInput {
            fixed4 vertex : POSITION;
            fixed2 uv : TEXCOORD0;
         };

         struct VertexOutput {
            fixed4 pos : SV_POSITION;
            fixed2 uv : TEXCOORD0;
         };
         
         VertexOutput vert(VertexInput v) {
            VertexOutput o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
         }
         
         fixed4 frag(VertexOutput i) : SV_Target{
            //fixed4 texColor = tex2D(_MainTex, i.uv);
            fixed4 texColor = fixed4(0,0,0,0);
            if (_Outline > 0 && texColor.a <= _Threshold) {
               // Get the neighbouring four pixels.
               fixed pixelUp = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y*_Outline)).a;
               fixed pixelDown = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y*_Outline)).a;
               fixed pixelRight = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x*_Outline, 0)).a;
               fixed pixelLeft = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x*_Outline, 0)).a;

               if (pixelUp + pixelDown + pixelRight + pixelLeft > _Threshold)
               {
                  texColor.rgba = _OutlineColor;
               }
            }

            return texColor;
         }
         ENDCG
      }
      
      Pass {
         CGPROGRAM
#pragma vertex vert
#pragma fragment frag
         #pragma shader_feature _ _STRAIGHT_ALPHA_INPUT

         sampler2D _MainTex;

         
         struct VertexInput {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float4 vertexColor : COLOR;
         };

         struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 vertexColor : COLOR;
         };

         VertexOutput vert (VertexInput v) {
            VertexOutput o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            o.vertexColor = v.vertexColor;
            return o;
         }

         float4 frag (VertexOutput i) : SV_Target {
            float4 texColor = tex2D(_MainTex, i.uv);
            #if defined(_STRAIGHT_ALPHA_INPUT)
            texColor.rgb *= texColor.a;
            #endif
            return (texColor * i.vertexColor);
         }
         ENDCG
      }

      Pass {
         Name "Caster"
         Tags { "LightMode"="ShadowCaster" }
         Offset 1, 1
         ZWrite On
         ZTest LEqual

         Fog { Mode Off }
         Cull Off
         Lighting Off

         CGPROGRAM
#pragma vertex vert
#pragma fragment frag
         #pragma multi_compile_shadowcaster
         #pragma fragmentoption ARB_precision_hint_fastest
         sampler2D _MainTex;
         fixed _Cutoff;

         struct VertexOutput { 
            V2F_SHADOW_CASTER;
            float4 uvAndAlpha : TEXCOORD1;
         };

         VertexOutput vert (appdata_base v, float4 vertexColor : COLOR) {
            VertexOutput o;
            o.uvAndAlpha = v.texcoord;
            o.uvAndAlpha.a = vertexColor.a;
            TRANSFER_SHADOW_CASTER(o)
            return o;
         }

         float4 frag (VertexOutput i) : SV_Target {
            fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
            clip(texcol.a * i.uvAndAlpha.a - _Cutoff);
            SHADOW_CASTER_FRAGMENT(i)
         }
         ENDCG
      }
   }
}