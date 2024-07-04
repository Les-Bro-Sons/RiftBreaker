using System.Collections.Generic;
using UnityEngine;

public class RB_SeeThroughWallsManager : MonoBehaviour
{
    public static RB_SeeThroughWallsManager Instance;
    public Material WallMaterial;
    public List<RB_SeeThroughWalls> Entities = new(); //player and ennemies

    private Texture2D _entityPositionsTexture;
    [SerializeField] private float _divider = 10f;
    public float LerpTime = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    

    void Start()
    {
        CreateTexture();
    }

    void Update()
    {
        //CreateTexture();
        for (int i = 0; i < Entities.Count; i++)
        {
            int x = i % _entityPositionsTexture.width;
            int y = i / _entityPositionsTexture.width;
            Vector3 pos = Entities[i].ShaderPosition;
            float a = Entities[i].LastTimeWallTouched;
            _entityPositionsTexture.SetPixel(x, y, new Color(pos.x / _divider, pos.y / _divider, pos.z / _divider, a / _divider));
        }
        _entityPositionsTexture.Apply();
        WallMaterial.SetTexture("_EntityPositionsTex", _entityPositionsTexture);
        WallMaterial.SetFloat("_LerpTime", LerpTime);
    }

    private void CreateTexture()
    {
        int textureSize = Mathf.CeilToInt(Mathf.Sqrt(Entities.Count));
        _entityPositionsTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBAFloat, false);
        _entityPositionsTexture.filterMode = FilterMode.Point;
        WallMaterial.SetTexture("_EntityPositionsTex", _entityPositionsTexture);
        WallMaterial.SetFloat("_PosDivider", _divider);
        WallMaterial.SetFloat("_EntityTexHeight", textureSize);
        WallMaterial.SetFloat("_EntityTexWidth", textureSize);
    }

    public void AddEntity(RB_SeeThroughWalls entity)
    {
        if (!Entities.Contains(entity)) Entities.Add(entity);
        CreateTexture();
    }
}