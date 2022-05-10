Shader "NewItem"
 {
     Properties
     {
         _Center ("Cener Pos", Vector) = (0,0,0,0)
         _Dist ("Dist", float) = 0
         _MainTex("Base (RGB)",2D) = "white" {} 
     }
         SubShader
         {
             Pass
             {
             Name "ColorizeSubshader"

             // ---
             // For Alpha transparency:   https://docs.unity3d.com/462/Documentation/Manual/SL-SubshaderTags.html
             Tags
             {
                 "Queue" = "Transparent"
                 "RenderType" = "Transparent"
             }
             Blend SrcAlpha OneMinusSrcAlpha
             // ---

             CGPROGRAM
             #pragma vertex   MyVertexShaderFunction
             #pragma fragment  MyFragmentShaderFunction
             #pragma fragmentoption ARB_precision_hint_fastest
             #include "UnityCG.cginc"

             sampler2D _MainTex;

             fixed4 _Center;
             float _Dist;
             //fixed4 _Color0;

             // http://wiki.unity3d.com/index.php/Shader_Code :
             // There are some pre-defined structs e.g.: v2f_img, appdata_base, appdata_tan, appdata_full, v2f_vertex_lit
             //
             // but if you want to create a custom struct, then the see Acceptable Field types and names at http://wiki.unity3d.com/index.php/Shader_Code
             // my custom struct recieving data from unity
             struct my_needed_data_from_unity
             {
                 float4 vertex   : POSITION;  // The vertex position in model space.          //  Name&type must be the same!
                 float4 texcoord : TEXCOORD0; // The first UV coordinate.                     //  Name&type must be the same!
                 float4 color    : COLOR;     //    The color value of this vertex specifically. //  Name&type must be the same!
             };

             // my custom Vertex to Fragment struct
             struct my_v2f
             {
                 float4  pos : SV_POSITION;
                 float2  uv : TEXCOORD0;
                 float4  color : COLOR;
             };

             my_v2f  MyVertexShaderFunction(my_needed_data_from_unity  v)
             {
                 my_v2f  result;
                 result.pos = UnityObjectToClipPos(v.vertex);  // Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
                 result.uv = v.texcoord.xy;
                 result.color = v.color;
                 return result;
             }

             float4 MyFragmentShaderFunction(my_v2f  i) : COLOR
             {
                float4 texcolor;
                float angle = sin(atan2(i.pos.y - _Center.y, i.pos.x - _Center.x) * 10 + _Time * 50) / 2 + 0.5;
                if(angle > 0.5) {
                    texcolor = fixed4(1, 1, 1, 1);
                }
                else {
                    texcolor = fixed4(0.9, 0.9, 0.9, 1);
                }

                float dist = max(0, 1 - distance(_Center, i.pos) / _Dist) * 0.2;
                texcolor += fixed4(dist, dist, dist, 1);

                
                return texcolor;
             }

             ENDCG
         }
     }
     //Fallback "Diffuse"
 }