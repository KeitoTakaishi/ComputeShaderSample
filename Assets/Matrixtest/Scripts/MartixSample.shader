Shader "Custom/MartixSample" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
        Pass{
		    CGPROGRAM
		    #pragma vertex vert
		    #fragment fragment frag
		    #include "UnityCG.cginc"
		    
		    struct v2f {
		        float4 pos : POSITION;
		        fixed4 color : COLOR;
		    };
		    
		    v2f vert (appdate_base v)
		    {
		        v2f o;
		        o.pos = UnityObjectToClipPos(v.vertex);
		        o.color = fixed4(1.0, 0.0, 0.0, 1.0);
		        return o;
		    } 
		    
		    fixed4 frag (v2f i) : SV_Target { return i.color; }
            ENDCG
		}
	}
	FallBack "Diffuse"
}
