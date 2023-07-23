Shader "UI/ItemLabel"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)
            fixed4 _TextureSampleAdd;

			v2f vert(appdata_t v)
            {				
				v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            }
        ENDCG
        }
    }
}
