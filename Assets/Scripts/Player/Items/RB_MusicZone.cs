using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_MusicZone : MonoBehaviour
{
    //Prefabs
    [Header("Prefabs")]
    [SerializeField] private GameObject _MusicNotePrefab;

    //Properties
    [Header("Properties")]
    [SerializeField] private float _noteAmount;
    WaitForSeconds waitForPointOneSec = new WaitForSeconds(.1f);
    private Transform _transform;

    //Components
    [Header("Components")]
    [SerializeField] private List<Sprite> _sprites = new();
    [SerializeField] private SpriteRenderer _spriteRenderer = new();

    private void Awake()
    {
        _spriteRenderer = _MusicNotePrefab.GetComponentInChildren<SpriteRenderer>();
        _transform = transform;
    }

    private void Start()
    {
        for(int i = 0; i < _noteAmount; i++)
        {
            StartCoroutine(DelaySpawnMusicNote());
        }
    }

    private IEnumerator DelaySpawnMusicNote()
    {
        yield return waitForPointOneSec;
        var musicNote = Instantiate(_MusicNotePrefab, _transform);
        _spriteRenderer.sprite = _sprites[Random.Range(0, _sprites.Count)];
        _spriteRenderer.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
    }
}
