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

    private void OnNoteDestroyed() //When a note is destroyed
    {
        foreach(var note in _musicNotes.ToList())
        {
            if(note == null) //If the note is destroyed
            {
                _musicNotes.Remove(note); //Remove it from the list
            }
        }
        if( _musicNotes.Count <= 1 )
        {
            print(_musicNotes[0]);
            Destroy(gameObject); //If the list is empty, destroy the zone
        }
    }

    public void StartDisapear() //Destroy one by one notes
    {
        foreach(var note in _musicNotes)
        {
            note.StartDisapear();
        }
    }


    public void StopTakeAway() //Stop the take away
    {
        foreach(RB_MusicNoteZone note in _musicNotes)
        {
            note.StopTakeAway();
        }
    }

    private IEnumerator DelaySpawnMusicNote() //Spawn all the music note with a delay
    {
        yield return waitForPointOneSec;
        var musicNote = Instantiate(_MusicNotePrefab, _transform);
        RB_MusicNoteZone musicNoteZone = musicNote.GetComponent<RB_MusicNoteZone>(); //Spawn the note
        _musicNotes.Add(musicNoteZone); //Add it to the list
        musicNote.GetComponent<RB_MusicNoteZone>().IntializeProperties(_currentMusicBox.ZoneProperties.Copy()); //Initialize the properties
        _spriteRenderer.sprite = _currentMusicBox.NoteSprites[Random.Range(0, _currentMusicBox.NoteSprites.Count)]; //Set the sprite randomly
        _spriteRenderer.color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f)); //Set the color randomly
        musicNoteZone.EventOnDestroy.AddListener(OnNoteDestroyed); //Add listener when the note is destroyed
    }
}
