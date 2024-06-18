using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Cinematique : MonoBehaviour
{
	public GameObject Portal;
	public GameObject PNJ;
	public GameObject Player;
	public GameObject Dial;
	public float alphaPnj = 0f;
	private float _openGateSpeed = 0f;
	private bool _cinematiqueStart = false;
	[SerializeField]
	private RB_DialogueManager _dialManager;


	private void Awake()
	{
		Portal.SetActive(false);
		PNJ.SetActive(false);
		//Dial.SetActive(false);
		_dialManager = RB_DialogueManager.Instance;

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (!_cinematiqueStart)
				StartCinematique();
		}

		void StartCinematique()
		{

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
				_dialManager.StartDialogue();
				_cinematiqueStart = true;
			}


		}

	}
}
