using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_MusicZone : MonoBehaviour
{
    //Prefabs
    [Header("Prefabs")]
    [SerializeField] private GameObject _MusicNotePrefab;

    //Properties
    WaitForSeconds waitForPointOneSec = new WaitForSeconds(.1f);
    private Transform _transform;

    //Components
    [Header("Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer = new();
    private RB_MusicBox _currentMusicBox;
    private List<RB_MusicNoteZone> _musicNotes = new();

    private void Awake()
    {
        _spriteRenderer = _MusicNotePrefab.GetComponentInChildren<SpriteRenderer>();
        _transform = transform;
    }

    private void Start()
    {
        _currentMusicBox = RB_PlayerAction.Instance.Item as RB_MusicBox;
        int noteAmount = _currentMusicBox.ZoneProperties.NoteAmount;
        for (int i = 0; i < noteAmount; i++)
        {
            StartCoroutine(DelaySpawnMusicNote());
        }
    }



    public void StopTakeAway()
    {
        foreach(RB_MusicNoteZone note in _musicNotes)
        {
            note.StopTakeAway();
        }
    }

    private IEnumerator DelaySpawnMusicNote()
    {
        yield return waitForPointOneSec;
        var musicNote = Instantiate(_MusicNotePrefab, _transform);
        _musicNotes.Add(musicNote.GetComponent<RB_MusicNoteZone>());
        musicNote.GetComponent<RB_MusicNoteZone>().IntializeProperties(_currentMusicBox.ZoneProperties.Copy());
        _spriteRenderer.sprite = _currentMusicBox.NoteSprites[Random.Range(0, _currentMusicBox.NoteSprites.Count)];
        _spriteRenderer.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
    }
}
