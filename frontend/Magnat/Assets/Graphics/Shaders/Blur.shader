Shader "Custom/SimpleGrabPassBlur" {
    Properties {
    }
   
    Category {
   
        // We must be transparent, so other objects are drawn before this one.
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
   
   
        SubShader {
       
            // Horizontal blur
            GrabPass { }
            
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                    half4 color : COLOR;
                };
               
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    half4 color : COLOR;
                };
                
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.color = v.color;
                    return o;
                }
               
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
               
                half4 frag( v2f i ) : COLOR {
//                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
//                  return col;
                   
                    half4 sum = half4(0,0,0,0);
 
                    #define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*i.color.a, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
 
                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);
                    return sum;
                }
                ENDCG
            }
            
            // Vertical blur
            GrabPass {  }
            
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                    half4 color : COLOR;
                };
               
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    half4 color : COLOR;
                };
                
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.color=v.color;
                    return o;
                }
               
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
               
                half4 frag( v2f i ) : COLOR {
//                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
//                  return col;
                   
                    half4 sum = half4(0,0,0,0);
 
                    #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*i.color.a, i.uvgrab.z, i.uvgrab.w))) * weight
 
                    //G(X) = (1/(sqrt(2*PI*deviation*deviation))) * exp(-(x*x / (2*deviation*deviation)))
                   
                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);
                   	
                   	half dark = clamp(1.3-i.color.a,0,1);
                    return sum*dark;
                }
                ENDCG
            } 
        }
    }
}