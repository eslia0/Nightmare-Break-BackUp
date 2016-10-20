using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
public class Monster : MonoBehaviour {
	//List<GameObject> player;
	public GameObject[] player;
	public GameObject monster;

	public GameObject perceivePlayer;
	public BattleCarculationManager BCM;

	public int stageLevel;
	public float hp;

	public float RunRange;// == perceive;
	public float attackRange;
	public float attackCycle;
	//monster Speed variable;

	private bool isAlive;
	private bool isAttack;
	private bool isHited;

	public float[] playerToMonsterDamage;
	public float[] aggroRank;
	public float changeTargetTime=0;
	 

	[SerializeField]private float[]currentDisTanceArray;
	private Vector3 checkDirection;
	public bool IsAttack{
		get{ return isAttack;}
	}
	public bool IsAlive{
		get{ return isAlive;}
	}
	public bool IsHited{
		get{ return isHited;}
		set{ isHited = value;}
	}

	//start monster Haves infomation setting(playersuch,monsterset)
	public void PlayerSuch(){
		
		player= GameObject.FindGameObjectsWithTag("Player");
		currentDisTanceArray = new float[player.Length];
		playerToMonsterDamage = new float[player.Length];
	}
	public void MonsterSet(){
		monster = this.gameObject;
		isAlive = true;
		isHited = false;
		BCM = GameObject.Find ("BattleCarculationManager").GetComponent<BattleCarculationManager> ();
	}


	public virtual void HitDamage(float _Damage){
		
	}





	private IEnumerator coNormalChasePlayer()
	{
		while (true) {
			if (isAlive) {
				if(!isHited){NormalchasePlayer ();
					yield return new WaitForSeconds (3f);}
			}
			if (!isAlive) {
				StopCoroutine (coNormalChasePlayer ());
			}
		}
	}

	public IEnumerator CoChasePlayer(){
		return coNormalChasePlayer ();
	}

	private IEnumerator coHitedChasePlayer(){
		while (true) {
			if (isAlive) {
				if (isHited) {
					HitedchasePlayer ();
					yield return new WaitForSeconds (2f);
				}
			}
			if (!isAlive) {
				StopCoroutine (coHitedChasePlayer ());
			}
		}
		
	}

	public IEnumerator CoHitedChasePlayer(){
		return coHitedChasePlayer();
	}

	//coutine need this method;
	public void NormalchasePlayer(){
		for (int i = 0; i < player.Length; i++) {
			currentDisTanceArray [i] = Vector3.Distance(player [i].transform.position, monster.transform.position);		
		}
		for (int j = 0; j < player.Length; j++) {
			if (currentDisTanceArray [j] <= Mathf.Min (currentDisTanceArray)) {
				perceivePlayer = player [j];
			}
		}

	}
	//coutine need this method;
	public void HitedchasePlayer(){
		for (int i = 0; i < player.Length; i++) {
			currentDisTanceArray [i] = Vector3.Distance(player [i].transform.position, monster.transform.position);		
			if (currentDisTanceArray [i] < 2f) {
				currentDisTanceArray [i] = 2f;
			}
			aggroRank [i] = playerToMonsterDamage [i] *(1/(currentDisTanceArray [i]*0.5f));
		}

		for (int j = 0; j < player.Length; j++) {
			if (aggroRank [j] <= Mathf.Max (aggroRank)) {
				perceivePlayer = player [j];
			}
		}
	}

}
