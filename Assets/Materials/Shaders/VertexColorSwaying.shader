
Shader "Custom/Vertex Color Swaying" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_Speed ("MoveSpeed", Range(20,50)) = 25 // speed of the swaying
			_Rigidness("Rigidness", Range(1,50)) = 25 // lower makes it look more "liquid" higher makes it look rigid
		_SwayMax("Sway Max", Range(0, 0.1)) = .005 // how far the swaying goes
		_YOffset("Y offset", float) = 0.5// y offset, below this is no animation
			
	}

	SubShader {
		Tags { "RenderType"="Opaque" "DisableBatching" = "True" }// disable batching lets us keep object space
		LOD 200

		
CGPROGRAM
#pragma surface surf Lambert vertex:vert addshadow // addshadow applies shadow after vertex animation

float4 _Color;

float _Speed;
float _SwayMax;
float _YOffset;
float _Rigidness;


struct Input {
	float2 uv_MainTex : TEXCOORD0;
	float3 vertColor;
};

void vert(inout appdata_full v, out Input o)// 
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	o.vertColor = v.color;


	float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;// world position
	float x = sin(wpos.x / _Rigidness + (_Time.x * _Speed)) *(v.vertex.y - _YOffset) * 5;// x axis movements
	float z = sin(wpos.z / _Rigidness + (_Time.x * _Speed)) *(v.vertex.y - _YOffset) * 5;// z axis movements
	v.vertex.x += step(0,v.vertex.y - _YOffset) * x * _SwayMax;// apply the movement if the vertex's y above the YOffset
	v.vertex.z += step(0,v.vertex.y - _YOffset) * z * _SwayMax;
}

void surf (Input IN, inout SurfaceOutput o) {
	half4 c = _Color;
	o.Albedo = c.rgb * IN.vertColor;
	o.Alpha = c.a;
}
ENDCG

	} 

	Fallback "Diffuse"
}
