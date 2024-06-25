using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_Cinematique : MonoBehaviour
{
	public GameObject Portal;
	public GameObject PNJ;
	public Transform Player;
	public Rigidbody PlayerRigidBody;
	public Animator _anim;
	public GameObject Dial;
	public Transform Wp;
	public float alphaPnj = 0f;
	private float _openGateSpeed = 0f;
	private bool _cinematiqueStart = false;
	[SerializeField] private RB_Dialogue _dialogue;
	public float WalkSpeed = 5;


	private void Awake()
	{

	}
	// Start is called before the first frame update
	void Start()
	{
		Portal.SetActive(false);
		PNJ.SetActive(false);
        //Dial.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		if (!_cinematiqueStart)
			StartCinematique();
	}

	void StartCinematique()
	{
		Vector3 walkDirection = Wp.position - Player.position;
		float walkDistance = walkDirection.magnitude;
		walkDirection = walkDirection.normalized;
        PlayerRigidBody.MovePosition(Player.position + (walkDirection * WalkSpeed * Time.deltaTime));
		
		if(walkDistance > 3) _anim.Play("Walk_Cinematique");
		else
		{
			_anim.Play("Player_Idle_Down");

			Portal.SetActive(true);
			if (_openGateSpeed < 2f)
			{
				_openGateSpeed += Time.deltaTime;
				Portal.transform.localScale = new Vector3(_openGateSpeed, 3, 1);
			}
			else
			{
				_openGateSpeed = 2f;
				Portal.transform.localScale = new Vector3(_openGateSpeed, 3, 1);
				PNJ.SetActive(true);
			}
			if (PNJ.active == true && alphaPnj < 1)
			{
				alphaPnj += Time.deltaTime;
				PNJ.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, alphaPnj);
			}
			if (PNJ.active == true && alphaPnj > 1)
			{
				PNJ.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
                RB_AudioManager.Instance.PlayMusic("Theme_Robert");
                _dialogue.StartDialogue();
				_cinematiqueStart = true;
			}

		}
	}

	public void QuitCinematique()
	{
		RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1);
	}
}

