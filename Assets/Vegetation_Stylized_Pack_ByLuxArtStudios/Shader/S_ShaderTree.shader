Shader "Custom/S_ShaderTree"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _FresnelPower ("Fresnel Power", Float) = 1.0 // Potencia del efecto Fresnel
        _FresnelColor ("Fresnel Color", Color) = (0,1,0,1) // Color para el efecto Fresnel
        _Contrast ("Contrast", Float) = 4.0 // Control del contraste del fresnel
        
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" }
        Cull Off

        CGPROGRAM
        #pragma surface surf Lambert addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;

        float _FresnelPower; // Potencia del fresnel
        fixed4 _FresnelColor; // Color del fresnel
        float _Contrast; // Contraste del fresnel
      

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal; // Normal del mundo para calcular Fresnel
            float3 viewDir; // Dirección de la vista
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Color base del tronco
            fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Calcular el efecto Fresnel: 1 - dot(normal, viewDir)
            float fresnelFactor = 1.0 - abs(dot(normalize(IN.worldNormal), normalize(IN.viewDir)));

            // Ajustar el Fresnel usando la potencia
            float fresnelPowerAdjusted = pow(fresnelFactor, _FresnelPower);

            // Aplicar el contraste
            float contrastAdjusted = pow(fresnelPowerAdjusted, _Contrast);

            // Interpolar entre el color base y el color del fresnel
            fixed4 finalColor = lerp(baseColor, _FresnelColor, contrastAdjusted);

           

            // Aplicar el color final al objeto
            o.Albedo = finalColor.rgb;
            o.Alpha = baseColor.a;
            clip(o.Alpha - 0.5); // Clip para transparencia
        }
        ENDCG
    }
    FallBack "Diffuse"
}
