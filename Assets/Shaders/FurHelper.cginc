#ifndef FUR_SHADER_HELPER
#define FUR_SHADER_HELPER

#pragma vertex vert
#pragma fragment frag

struct VertexInput 
{
	float4 position : POSITION;
	float4 normal   : NORMAL;
	float2 uv  		: TEXCOORD0;//Corresponds to the noise texture
	float2 uv2 		: TEXCOORD1;//Corresponds to the main texture
};

struct Vert2Frag
{
	float4 position : POSITION;
	float4 normal	: NORMAL;
	float2 uv       : TEXCOORD0;
	float2 uv2      : TEXCOORD1;
};

//Properties from material block that
//need to be redeclared in order to be 
//used in the shaders
uniform sampler2D _MainTex;
uniform sampler2D _NoiseTex;
uniform float3 _Direction;
uniform float _Thinness;
uniform float3 _WindSpeed;
uniform float _Length;

float3 calculateWindForceDirection(float3 vertexInputPosition)
{
	//Wind force
	float3 windForceDirection = float3(0.0, 0.0, 0.0);
	//_Time.y corresponds to the time since
	//the app loaded
	//Use the sine and cosine waves to create a smooth wind
	//force that oscillates
	//Then multiply it by the windspeed vector
	//to amplify the wind force
	windForceDirection.x = sin((_Time.y + vertexInputPosition.y) * _WindSpeed.x);
	windForceDirection.y = cos((_Time.y + vertexInputPosition.y) * _WindSpeed.y);
	windForceDirection.z = sin((_Time.y + vertexInputPosition.y) * _WindSpeed.z);
	return windForceDirection;
}

Vert2Frag vert(VertexInput vertexInput) 
{	
	Vert2Frag vertexOutput;
		
	//Calculate the wind force direction that will be applied to the fur
	float3 windForceDirection = calculateWindForceDirection(vertexInput.position);
	
	//Add the direction of the fur to the wind force direction
	//to get the displacment
	float3 displacement = windForceDirection + _Direction.xyz;
	
	//Add that displacement to the normal to get the direction
	//of the fur affected by the wind force
	float displacementFactor = FUR_OFFSET;
	float4 aNormal = vertexInput.normal;
	aNormal.xyz +=  displacement * displacementFactor;
	
	//Multiply the product by the fur offset and length
	//to get the fur rendered from the surface of the object
	//and determine the fur length
	//Note increasing the fur length increases how thin the fur looks
	float4 n = normalize(aNormal) * FUR_OFFSET * _Length;
	//Finally we add it to original position and multipy it by
	//the MVP to get correctly rendered into world space
	float4 worldPosition = float4(vertexInput.position.xyz + n.xyz, 1.0);
	vertexOutput.position = mul(UNITY_MATRIX_MVP, worldPosition);
	
	vertexOutput.normal = vertexInput.normal;
	
	vertexOutput.uv = vertexInput.uv;
	vertexOutput.uv2 = vertexInput.uv2;

	return vertexOutput;
}

float4 frag(Vert2Frag vertexOutput) : COLOR 
{
	//Don't render a pixel if the noiseTexture alpha
	//is 0 or below or the b is less than the fur offset
	float4 noiseTexColor = tex2D(_NoiseTex, vertexOutput.uv);
	if (noiseTexColor.a <= 0.0 || noiseTexColor.b < FUR_OFFSET) 
	{
		discard;
	}

	//Set the main texture color to be the same as the noise
	//texture alpha
	float4 mainTexColor = tex2D(_MainTex, vertexOutput.uv2);
	mainTexColor.a = noiseTexColor.a;
	return mainTexColor;
}

#endif