using UnityEngine;
using System.Collections;

public class MonsterWeapon : MonoBehaviour {

	// Use this for initialization
	public GameObject monster;
	public int damage = 0;

	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.layer == LayerMask.NameToLayer ("Player"))
		{
			CharacterManager CharObject = coll.gameObject.GetComponent<CharacterManager> ();
	

			if (damage != 0)
			{
				CharObject.HitDamage (damage);
			}

		}
	}

}
