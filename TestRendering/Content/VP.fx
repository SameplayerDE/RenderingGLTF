#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

matrix WorldViewProjection;
matrix World;
matrix View;
matrix Projection;

bool UseTexture = false;
bool Unlit = true;
bool Toon = false;

float4 BaseColorFactor = float4(1, 1, 1, 1);

float DeltaTime = 0;

float3 LightPosition = float3(10, 25, 0);
float LightAttenuation = 2.5;
float LightFalloff = 1;

float3 CameraPosition = float3(10, 10, 0);

Texture2D Texture : register(t0);
sampler TextureSampler : register(s0)
{
	Texture = (Texture);
	MinFilter = Point; // Minification Filter
    MagFilter = Point;// Magnification Filter
    MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureUVs : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureUVs : TEXCOORD0;
	float4 Color : COLOR0;
	float3 Normal : NORMAL0;
	
	float3 LightDirection : TEXCOORD1;
	float LightDistance : TEXCOORD2;
    
    float3 ViewDirection : TEXCOORD3;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 position = input.Position;
	//position.y += sin(position.x / 100 + DeltaTime) * 50;
	output.Position = mul(position, WorldViewProjection);
	output.Color = BaseColorFactor;
	output.Normal = mul(input.Normal, World);
	//output.LightDirection =  LightPosition - mul(input.Position, World);
	output.LightDirection =  mul(input.Position, World) - LightPosition;
	output.LightDistance =  distance(mul(input.Position, World), LightPosition);
	output.ViewDirection = CameraPosition - mul(input.Position, World);
	output.TextureUVs = input.TextureUVs;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 Color = input.Color;
    float3 N = normalize(input.Normal);
    float3 L = normalize(input.LightDirection);
    float3 C = normalize(input.ViewDirection);
    float3 R = reflect(L, N);
    
    if (UseTexture) {
        Color *= tex2D(TextureSampler, input.TextureUVs);
        clip(Color.a < 0.75f ? -1 : 1);
    }
    
    if (!Unlit) {
        float A = saturate(dot(N, -L));
        A = clamp(A, 0.1f, 1);
        Color += pow(saturate(dot(R, C)), 10) * float4(1, 1, 1, 1);   
        //float Attenuation = 1.05 - pow(clamp(input.LightDistance / LightAttenuation, 0, 1), LightFalloff);
        Color *= A;// * Attenuation;
    }
    if (Toon) {
        if (dot(C, N) < 0.4) {
            Color.rgb *= 0;
        }
        
        if (dot(C, N) > 0.6) {
            Color.rgb *= 1.1;
        }
        
        if (dot(C, N) > 0.98) {
            Color.rgb *= 2;
        }
    }
    
    /*float r = Color.r;
    float g = Color.g;
    float b = Color.b;
    
    if (r > g && r > b) {
         Color.gb = 0;
    }
    else if (g > r && g > b) {
         Color.rb = 0;
    }
    else if (b > g && b > r) {
         Color.gr = 0;
    }*/
	return Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
        ZEnable = true;
        ZWriteEnable = true;
        AlphaBlendEnable = false;
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};