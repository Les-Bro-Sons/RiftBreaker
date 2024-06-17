using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Cinematique : MonoBehaviour
{
    public GameObject Portal;
    public GameObject PNJ;
    public GameObject Player;
	public float alphaPnj = 0f;
    private float openGateSpeed = 0f;
	private bool cinematiqueStart = false;

	private void Awake()
	{
		Portal.SetActive(false);
		PNJ.SetActive(false);
		
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(!cinematiqueStart) 
		StartCinematique();
	}

    void StartCinematique()
    {
		
		Portal.SetActive(true);
		if (openGateSpeed < 1f)
		{
			openGateSpeed += Time.deltaTime;
			Portal.transform.localScale = new Vector3(openGateSpeed, 2,1);
		}
		else
		{
			openGateSpeed = 1f;
			Portal.transform.localScale = new Vector3(openGateSpeed, 2,1);
			PNJ.SetActive(true);
		}
		if (PNJ.active == true && alphaPnj < 1)
		{
			alphaPnj += Time.deltaTime;
			PNJ.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, alphaPnj);
		}
		if(PNJ.active == true && alphaPnj > 1)
		{
			PNJ.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
			cinematiqueStart = true;
		}

		
    }

}
