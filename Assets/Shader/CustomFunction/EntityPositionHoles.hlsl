// EntityPositionHoles.hlsl

#define MAX_TEX_DIM 64

void CalculateHoles_float(float texWidth, float texHeight, float3 worldPosition, float3 cameraPosition, Texture2D entityPositionsTex, SamplerState samplerState, float posDivider, float holeRadius, out float3 hole, out float holeAlpha)
{
    float3 entityPosition;
    float2 uv;
    float dist;
    //hole = 0.0;
    hole = float3(9999, 9999, 9999);
    holeAlpha = 1;
    
    int maxTexWidth = (int) min(texWidth, MAX_TEX_DIM);
    int maxTexHeight = (int) min(texHeight, MAX_TEX_DIM);
    
    for (int x = 0; x < MAX_TEX_DIM; x++)
    {
        if (x >= maxTexWidth) break;
        for (int y = 0; y < MAX_TEX_DIM; y++)
        {
            if (y >= maxTexHeight) break;
            uv = float2(x / texWidth, y / texHeight);
            entityPosition = entityPositionsTex.Sample(samplerState, uv).xyz * posDivider;
            float entityAlpha = entityPositionsTex.Sample(samplerState, uv).a;
            dist = distance(worldPosition, entityPosition);

            if (dist < holeRadius * 4)
            {
                float3 flatEntityPosition = float3(entityPosition.x, 0, entityPosition.z);
                float3 flatWorldPosition = float3(worldPosition.x, 0, worldPosition.z);
                if (distance(flatWorldPosition, cameraPosition) < distance(cameraPosition, flatEntityPosition))
                {
                    //hole = 1.0;
                    hole = entityPosition;
                    holeAlpha = entityAlpha;
                }
                else
                {
                    //hole = 0;
                }
                break;
            }
        }
    }
}
