using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
public class Monster : MonoBehaviour {
	//public DungeonManager dungeonManager;

	public GameObject[] player;
	public Animator animator;

	public GameObject targetPlayer;
	//public BattleCarculationManager BCM;
    
	

	//mode,gateArraynumber,monsterArraynumber
	[SerializeField]private bool mode;
	public bool Mode{
		set { mode = value;}
	}
	[SerializeField]private int gateArrayNumber;
	public int GateArrayNumber{
		set{ gateArrayNumber = value;}
	}

	[SerializeField]private int monsterArrayNumber;
	public int MonsterArrayNumber{
		set{ monsterArrayNumber = value;}
	}

	//server send to this class monsterinfomation;
	protected int stageLevel;
	[SerializeField]protected float currentLife;
	[SerializeField]protected float maxLife;

	//monster getting variable;
	public float RunRange;// == perceive;
	public float attackRange;
	public float attackCycle;
	protected float IdleRandomTime=0;
	//monster Speed variable;

	private bool isAlive;
	protected bool isAttack;
	private bool isHited;

	public float[] playerToMonsterDamage;
	public float[] aggroRank;
	public float changeTargetTime=0;
	 

	[SerializeField]private float[]currentDisTanceArray;
	protected Vector3 checkDirection; // monster chaseplayer and move variable;
//	public bool IsAttack{
//		get{ return isAttack;}
//	}
	public bool IsAlive{
		get{ return isAlive;}
	}
	public bool IsHited{
		get{ return isHited;}
		set{ isHited = value;}
	}

	//start monster Haves infomation setting(playersuch,monsterset)
	public void PlayerSearch(){
		
		player= GameObject.FindGameObjectsWithTag("Player");
		currentDisTanceArray = new float[player.Length];
		playerToMonsterDamage = new float[player.Length];
	}
	public void MonsterSet(){
		isAlive = true;
		isHited = false;
		//BCM = GameObject.Find ("BattleCarculationManager").GetComponent<BattleCarculationManager> ();
		//dungeonManager = GameObject.Find ("DungeonManager").GetComponent<DungeonManager> (); error ! Delete or change;
		animator = this.gameObject.GetComponent<Animator> ();
	}


	public virtual void HitDamage(float _Damage){

    }


    public void ChasePlayer(){
		//Debug.Log (changeTargetTime);
		if(!isHited)
		{
			changeTargetTime +=Time.deltaTime;
			if(changeTargetTime >=3){
				changeTargetTime = 0;
				NormalchasePlayer();
			}
		}
		if(isHited){
			changeTargetTime += Time.deltaTime;
			if(changeTargetTime >=2){
				changeTargetTime = 0;
				HitedchasePlayer();
			}
		}
	}

    

	//coutine need this method;
	public void NormalchasePlayer(){
		for (int i = 0; i < player.Length; i++) {
			currentDisTanceArray [i] = Vector3.Distance(player [i].transform.position, transform.position);		
		}
		for (int j = 0; j < player.Length; j++) {
			if (currentDisTanceArray [j] <= Mathf.Min (currentDisTanceArray)) {
                targetPlayer = player [j];
			}
		}

	}
	//coutine need this method;
	public void HitedchasePlayer(){
		for (int i = 0; i < player.Length; i++) {
			currentDisTanceArray [i] = Vector3.Distance(player [i].transform.position, transform.position);		
			if (currentDisTanceArray [i] < 2f) {
				currentDisTanceArray [i] = 2f;
			}
			aggroRank [i] = playerToMonsterDamage [i] *(1/(currentDisTanceArray [i]*0.5f));
		}

		for (int j = 0; j < player.Length; j++) {
			if (aggroRank [j] <= Mathf.Max (aggroRank)) {
                targetPlayer = player [j];
			}
		}
	}

	public void MonsterArrayEraser(GameObject thisGameObject){
		//gameObject = null;
		isAlive=false;
		thisGameObject.SetActive (false);
		if (!mode) {
//			section.RemoveMonsterArray ();
		}

		if (mode) {
		
		}

		//mode = asf1.GetComponent<DungeonManager> ().modeForm;
		//dungeonManager.MonsterArrayAliveCheck(asdf)
	}

}
