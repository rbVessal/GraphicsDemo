Shader "Custom/Fur" 
{
	//Public properties that the user can manipulate
	Properties 
	{
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_NoiseTex  ("Noise Texture (RGB)", 2D) = "white" {} //Note: This must be a noise texture that has alpha spots.
															//otherwise the fur will not be rendered.
		[HideInInspector] _Direction ("Direction", Vector) = (0.0, -0.75, 0.0)
		_WindSpeed("WindSpeed", Vector) = (1.0, 1.0, 1.0) //Note: w component is not factored into the equation, so it is unnecessary.
		_Length("Length", Float) = 0.1 //Note: Increasing the fur length increases how thin the fur looks
	}
	
	Category 
	{
		ZWrite Off
		//Set the blend mode for transparency
		//since the noise texture we are 
		///using has transparency
		Tags {"Queue" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
			
		SubShader 
		{
			//Each pass creates a shell
			//This is important because
			//the subsequent passes build on
			//the previous ones which is how
			//the fur is generated from the base
			//to the tip.  If you want more detailed
			//fur, then add more passes and make sure
			//the fur offset slowly increments up to 1.0
			//starting from the 2nd pass.
			//Note: There should be a way
			//to dynamically create passes, but
			//it appears Unity doesn't allow it at the 
			//current moment.  Perhaps some injection
			//from a script could dynamically create the 
			//passes.
			Pass 
			{
				ZWrite On
				Blend Off
			
				CGPROGRAM
				
				#pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.04
				
				#include "FurHelper.cginc"
				
				ENDCG
			}
                
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.05
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
                
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.1
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
                
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.15
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
                
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.2
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
                
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.25
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
                
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.3
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.35
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.4
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.45
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.5
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.55
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.6
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.65
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.7
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.75
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.8
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
            
            Pass 
            {
                CGPROGRAM
                
                #pragma vertex vert
                #pragma fragment frag
                
                #define FUR_OFFSET 0.85
                
                #include "FurHelper.cginc"
                
                ENDCG
            }
		}
		
		Fallback "VertexLit", 1
	}
}