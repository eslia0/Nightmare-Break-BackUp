using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour {
	public BoomMonster[] flock; 
	public GameObject TestMonster;

	void MonsterSet(){
		flock = GameObject.FindObjectsOfType<BoomMonster> ();
		for (int i = 0; i < flock.Length; i++) {
			flock [i].MonsterSet ();
			flock [i].PlayerSuch ();
			flock [i].Pattern (BoomMonster.StatePosition.Idle);
		}
	}

	void Start () {
		MonsterSet ();	
	}

	void Update () {
		for (int i = 0; i < flock.Length; i++) {
			flock [i].UpdateConduct();
		}
	}





}
