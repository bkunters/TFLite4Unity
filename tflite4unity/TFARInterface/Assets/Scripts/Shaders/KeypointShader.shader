Shader "Custom/KeypointShader"
{
    Properties{
        _MainTex("Texture", 2D) = "white"{}
    }

    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 scrPos : TEXCOORD0;
                float2 uv     : TEXCOORD1; 
            };

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex);
                o.uv     = v.texcoord; 
                return o;
            }
             
            uniform sampler2D _MainTex; 
            // 2D keypoint position buffer.
            uniform float m_keypointBuffer[42];

            fixed4 frag (v2f i) : SV_Target
            {
                // Compute the rendered pixel.
                fixed4 uv_rendered = tex2D(_MainTex, i.uv);

                // Returns the normalized screen coords.((0,0) bottom-left/(1,1) top-right).
                float2 scrPos = i.scrPos.xy / i.scrPos.w;
                for(int j = 0; j < 21; j += 2){
                    // Compute the Unity-based screen coordinates.
                    float2 keypointPos = float2(m_keypointBuffer[j], scrPos.y * _ScreenParams.y - m_keypointBuffer[j+1]);
                    // Set the circle on the keypoint.
                    if(distance(scrPos * _ScreenParams.xy, keypointPos) <= 15){
                        return fixed4(1.0, 1.0, 1.0, 1.0);    
                    }
                }

                return uv_rendered;
            }
            ENDCG
        }
    }
}
