using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void OnNoteDestroyed()
    {
        foreach(var note in _musicNotes.ToList())
        {
            if(note == null)
            {
                _musicNotes.Remove(note);
            }
        }
        print(_musicNotes.Count);
        if( _musicNotes.Count <= 1 )
        {
            Destroy(gameObject);
        }
    }

    public void StartDisapear()
    {
        foreach(var note in _musicNotes)
        {
            note.StartDisapear();
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
        RB_MusicNoteZone musicNoteZone = musicNote.GetComponent<RB_MusicNoteZone>();
        _musicNotes.Add(musicNoteZone);
        musicNote.GetComponent<RB_MusicNoteZone>().IntializeProperties(_currentMusicBox.ZoneProperties.Copy());
        _spriteRenderer.sprite = _currentMusicBox.NoteSprites[Random.Range(0, _currentMusicBox.NoteSprites.Count)];
        _spriteRenderer.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f));
        musicNoteZone.EventOnDestroy.AddListener(OnNoteDestroyed);
    }
}
