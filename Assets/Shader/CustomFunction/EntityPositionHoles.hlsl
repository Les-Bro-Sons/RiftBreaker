// EntityPositionHoles.hlsl

void CalculateHoles_float(float3 worldPosition, float3 cameraPosition, Texture2D entityPositionsTex, SamplerState samplerState, float posDivider, float holeRadius, out float hole)
{
    float3 entityPosition;
    float2 uv;
    float dist;
    hole = 0.0;

    // Assuming the texture is 4x4 for this example
    float texWidth = 4.0;
    float texHeight = 4.0;
    
    for (int x = 0; x < 4; x++)
    {
        for (int y = 0; y < 4; y++)
        {
            uv = float2(x / texWidth, y / texHeight);
            entityPosition = entityPositionsTex.Sample(samplerState, uv).xyz * posDivider;
            dist = distance(worldPosition, entityPosition);

            if (dist < holeRadius)
            {
                float3 flatEntityPosition = float3(entityPosition.x, 0, entityPosition.z);
                float3 flatWorldPosition = float3(worldPosition.x, 0, worldPosition.z);
                if (distance(flatWorldPosition, cameraPosition) < distance(cameraPosition, flatEntityPosition))
                {
                    hole = 1.0;
                }
                else
                {
                    hole = 0;
                }
                break;
            }
        }
    }
}
