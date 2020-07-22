Shader "Skybox/Farland Skies/Cloudy Crown" {
	
	Properties{
		[Header(Sky)]

			_TopColor("Color Top", Color) = (.247, .318, .561, 1.0)
			_BottomColor("Color Bottom", Color) = (.773, .455, .682, 1.0)

		[Header(Stars)]

			_StarsTint("Stars Tint", Color) = (.5, .5, .5, 1.0)
			_StarsExtinction("Stars Extinction", Range(0, 10)) = 2.0
			_StarsTwinklingSpeed("Stars Twinkling Speed", Range(0, 25)) = 4.0
			[NoScaleOffset]
			_StarsTex("Stars Cubemap", Cube) = "grey" {}
			[NoScaleOffset]
			_StarsTwinklingTex("Stars Twinkling Cubemap", Cube) = "grey" {}

		[Header(Sun)]

			_SunSize("Sun Size", Range(0.1, 3)) = 1.0
			_SunTint("Sun Tint", Color) = (.5, .5, .5, 1.0)

			[NoScaleOffset]
			_SunTex("Sun Texture", 2D) = "grey" {}

		[Header(Moon)]

			_MoonSize("Moon Size", Range(0.1, 3)) = 1.0
			_MoonTint("Moon Tint", Color) = (.5, .5, .5, 1.0)

			[NoScaleOffset]
			_MoonTex("Moon Texture", 2D) = "grey" {}

        [Header(Clouds)]

			_CloudsHeight("Clouds Height", Range(-0.75, 0.75)) = 0
			_CloudsOffset("Clouds Offset", Range(0, 1)) = 0.2
			_CloudsRotationSpeed("Clouds Rotation Speed", Range(-50, 50)) = 1
			[NoScaleOffset]
			_CloudsTex("Clouds Cubemap", Cube) = "grey" {}

		[Header(General)]

			[Gamma] _Exposure("Exposure", Range(0, 10)) = 1.0
	}

	SubShader{
		Tags{ "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Cull Off ZWrite Off

		Pass{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Exposed
			half3 _TopColor;
			half3 _BottomColor;

			fixed _CloudsHeight;
			fixed _CloudsOffset;
			fixed _CloudsRotationSpeed;
			samplerCUBE _CloudsTex;

			half4 _StarsTint;
			fixed _StarsTwinklingSpeed;
			fixed _StarsExtinction;
			samplerCUBE _StarsTex;
			samplerCUBE _StarsTwinklingTex;

			fixed _SunSize;
			half4 _SunTint;
			sampler2D _SunTex;

			fixed _MoonSize;
			half4 _MoonTint;
			sampler2D _MoonTex;

			half _Exposure;

			// Scripted
			float4x4 sunMatrix;
			float4x4 moonMatrix;

			// Const
			static const half4 kHaloBase = half4(1.0, 1.0, 1.0, 0);

			// -----------------------------------------
			// Structs
			// -----------------------------------------

			struct v2f {
				float4 position : SV_POSITION;
				float3 vertex : TEXCOORD0;
				float3 cloudsPosition1 : TEXCOORD1;
				float3 cloudsPosition2 : TEXCOORD2;
				float3 cloudsPosition3 : TEXCOORD3;
				float3 cloudsPosition4 : TEXCOORD4;
				float3 cloudsPosition5 : TEXCOORD5;
				float3 twinklingPosition : TEXCOORD6;
				float3 sunPosition : TEXCOORD7;
				float3 moonPosition : TEXCOORD8;
			};

			// -----------------------------------------
			// Functions
			// -----------------------------------------

			float4 RotateAroundYInDegrees(float4 vertex, float degrees)
			{
				float alpha = degrees * UNITY_PI / 180.0;
				float sina, cosa;
				sincos(alpha, sina, cosa);
				float2x2 m = float2x2(cosa, -sina, sina, cosa);
				return float4(mul(m, vertex.xz), vertex.yw).xzyw;
			}

			float4 RotateAroundYXInDegrees(float4 vertex, float degrees)
			{
				float alpha = degrees * UNITY_PI / 180.0;
				float sina, cosa;
				sincos(alpha, sina, cosa);
				
				float2x2 m = float2x2(cosa, -sina, sina, cosa);
				float4 rot = float4(mul(m, vertex.xz), vertex.yw).xzyw;

				m = float2x2(cosa, sina, -sina, cosa);
				rot = float4(mul(m, rot.yz), rot.xw).yzxw;

				return rot;
			}

			half4 CelestialColor(float3 position, sampler2D tex, fixed size, half4 tint) {
				fixed depthCheck = step(position.z, 0); // equivalent of (position.z < 0)			
				half4 sTex = tex2D(tex, position.xy / (0.5 * size) + float2(0.5, 0.5));

				half4 halo = 1.0 - smoothstep(0, 0.35 * size, length(position.xy));
				sTex = sTex.r + (kHaloBase + 1.75 * halo * halo) * sTex.b;

				tint.rgb = unity_ColorSpaceDouble.rgb * tint.rgb;

				return depthCheck * sTex * tint;
			}

			v2f vert(appdata_base v)
			{
				v2f OUT;

				// General
				OUT.position = mul(UNITY_MATRIX_MVP, v.vertex);
				OUT.vertex = v.vertex;
				
				// Stars
				OUT.twinklingPosition = RotateAroundYXInDegrees(v.vertex, _Time.y * _StarsTwinklingSpeed);
				// Sun
				OUT.sunPosition = mul(sunMatrix, v.vertex);
				// Moon
				OUT.moonPosition = mul(moonMatrix, v.vertex);

				// Clouds
				OUT.cloudsPosition1 = RotateAroundYInDegrees(v.vertex, 2.00 * _CloudsRotationSpeed * _Time.y);
				OUT.cloudsPosition1.y -= _CloudsHeight - 1.275 * _CloudsOffset;

				OUT.cloudsPosition2 = RotateAroundYInDegrees(v.vertex, 1.25 * _CloudsRotationSpeed * _Time.y + 72);
				OUT.cloudsPosition2.y -= _CloudsHeight - 0.600 * _CloudsOffset;

				OUT.cloudsPosition3 = RotateAroundYInDegrees(v.vertex, 0.75 * _CloudsRotationSpeed * _Time.y + 144);
				OUT.cloudsPosition3.y -= _CloudsHeight;

				OUT.cloudsPosition4 = RotateAroundYInDegrees(v.vertex, 0.40 * _CloudsRotationSpeed * _Time.y + 216);
				OUT.cloudsPosition4.y -= _CloudsHeight + 0.500 * _CloudsOffset;

				OUT.cloudsPosition5 = RotateAroundYInDegrees(v.vertex, 0.25 * _CloudsRotationSpeed * _Time.y + 288);
				OUT.cloudsPosition5.y -= _CloudsHeight + 0.950 *_CloudsOffset;

				return OUT;
			}

			half4 frag(v2f IN) : SV_Target
			{				
				// Stars
				half3 starsTex = texCUBE(_StarsTex, IN.vertex);
				half3 twinklingTex = texCUBE(_StarsTwinklingTex, IN.twinklingPosition);
				half extinction = saturate((IN.vertex.y - _CloudsHeight - _CloudsOffset) * _StarsExtinction);
				half starsCoef = starsTex.r * _StarsTint.a * extinction * twinklingTex;
				half3 color = _TopColor * (1 - starsCoef) + (_StarsTint.rgb * unity_ColorSpaceDouble.rgb) * starsCoef;

				// Sun
				half4 sunColor = CelestialColor(IN.sunPosition, _SunTex, _SunSize, _SunTint);
				color = lerp(color, sunColor.rgb, sunColor.a);

				// Moon
				half4 moonColor = CelestialColor(IN.moonPosition, _MoonTex, _MoonSize, _MoonTint);
				color = lerp(color, moonColor.rgb, moonColor.a);

				// Clouds
				half3 cloudsTex = texCUBE(_CloudsTex, IN.cloudsPosition5);
				color = cloudsTex.r * lerp(_BottomColor, _TopColor, 0.8) + cloudsTex.b * color;

				cloudsTex = texCUBE(_CloudsTex, IN.cloudsPosition4);
				color = cloudsTex.r * lerp(_BottomColor, _TopColor, 0.6) + cloudsTex.b * color;

				cloudsTex = texCUBE(_CloudsTex, IN.cloudsPosition3);
				color = cloudsTex.r * lerp(_BottomColor, _TopColor, 0.4) + cloudsTex.b * color;

				cloudsTex = texCUBE(_CloudsTex, IN.cloudsPosition2);
				color = cloudsTex.r * lerp(_BottomColor, _TopColor, 0.2) + cloudsTex.b * color;

				cloudsTex = texCUBE(_CloudsTex, IN.cloudsPosition1);
				color = cloudsTex.r * _BottomColor + cloudsTex.b * color;
				
				// General
				color *= _Exposure;

				return half4(color, 1);
			}
			ENDCG
		}
	}
}