using UnityEngine;
using System.Collections;

public class CharterPlayer : MonoBehaviour 
{

	public GameObject MonsterA;
	public GameObject User;
	public Vector3 disvec;

	public GameObject EsPadaSword;

	// Use this for initialization
	void Start () 
	{
		//MonsterA = GameObject.FindGameObjectWithTag ("Monster");
	}

	// Update is called once per frame
	void Update () 
	{

		if (Input.GetButtonDown ("Skill1"))
		{
			Maelstrom ();
		}
		else if (Input.GetButtonDown ("Skill2"))
		{
			Espada ();
		}

	}


	public void Maelstrom()
	{
		Vector3 velocity = Vector3.zero;
		float maelstromDistance = Vector3.Distance (MonsterA.transform.position, this.transform.position);

		float maelstromSpeed = 10.0f;


		if (maelstromDistance <= 10.0f)
		{
			MonsterA.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y,this.transform.position.z);
		}

		Debug.Log ("mael");

	}

	public void Espada()
	{
		GameObject EspadaTemp =(GameObject)Instantiate (EsPadaSword, transform.position + new Vector3 (3.0f, 5.0f, 0f), transform.rotation);
		Destroy (EspadaTemp, 1.0f);
	}

	void OnColliderEnter(Collision coll)
	{
		if (coll.gameObject.layer == LayerMask.NameToLayer ("Monster"))
		{
			coll.transform.position = new Vector3 (MonsterA.transform.position.x -1.0f, MonsterA.transform.position.y -1.0f,MonsterA.transform.position.z -1.0f);
			Debug.Log ("Incoll");
		}
	}
}