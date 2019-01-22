Shader "Solochara/SpriteEffectsShader" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
        [PerRendererData] _Flash ("Flash", Color) = (1,1,1,0)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        [PerRendererData] _Alpha("Alpha", Float) = 1.0
        [PerRendererData] _Desaturation("Desaturation", Range(0, 1)) = 0.0
    }
    
	SubShader {
		Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnitySprites.cginc"
            
            float _Alpha;
            float _Desaturation;
            fixed4 _Flash;

			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

            fixed4 frag(v2f IN) : SV_Target {
                float2 xy = IN.texcoord;
                fixed4 c = SampleSpriteTexture(xy) * IN.color;
                float avg = (c[0] + c[1] + c[2]) / 3.0;
                float4 desat = float4(avg / 2.0, avg / 2.0, avg / 2.0, c.a);
                c.rgb = c.rgb * (1.0 - _Desaturation) + desat.rgb * (_Desaturation);
                c.rgb = c.rgb * (1.0 - _Flash.a) + _Flash.rgb * _Flash.a;
                c.rgb *= c.a;
                return c;
            }
		ENDCG
		}
	}
}
