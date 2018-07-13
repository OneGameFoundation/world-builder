Shader "Custom/Vertex Colored Unlit" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
 
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 150
 
CGPROGRAM
#pragma surface surf Unlit vertex:vert
 
sampler2D _MainTex;
fixed4 _Color;
 
struct Input {
    float2 uv_MainTex;
    float3 vertColor;
};

half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
    half4 c;
    c.rgb = s.Albedo;
    c.a = s.Alpha;

    return c;
}
 
void vert (inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input, o);
    o.vertColor = v.color;
}
 
void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb * IN.vertColor;
    o.Alpha = c.a;
}
ENDCG
}
 
Fallback "Diffuse"
}
