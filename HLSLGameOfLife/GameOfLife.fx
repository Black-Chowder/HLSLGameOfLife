#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float2 dim;

texture Texture;
sampler TextureSampler = sampler_state
{
	Texture = <Texture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float vertPixSize = 1.0f / dim.y;
	float horPixSize = 1.0f / dim.x;

	float4 white = float4(1, 1, 1, 1);
	float4 black = float4(0, 0, 0, 1);

	float4 texColor = tex2D(TextureSampler, input.TextureCoordinate);

	int count = 0;
	for (int r = -1; r <= 1; r++) {
		for (int c = -1; c <= 1; c++) {

			if (r == 0 && c == 0) continue;

			float4 neighbor = tex2D(
				TextureSampler,
				input.TextureCoordinate + float2(horPixSize * r, vertPixSize * c)
			);

			if (neighbor.r == 0)
				count++;
		}
	}

	if (count < 2 || count > 3)
		return white;
	if (count == 3)
		return black;

	return texColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};