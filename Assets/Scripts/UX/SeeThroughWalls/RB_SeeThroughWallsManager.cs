using System.Collections.Generic;
using UnityEngine;

public class RB_SeeThroughWallsManager : MonoBehaviour
{
    public static RB_SeeThroughWallsManager Instance;
    public Material WallMaterial;
    public List<Transform> Entities = new(); //player and ennemies

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
            Vector3 pos = Entities[i].position;
            float a = Entities[i].GetComponent<RB_SeeThroughWalls>().LastTimeWallTouched;
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
    }
}