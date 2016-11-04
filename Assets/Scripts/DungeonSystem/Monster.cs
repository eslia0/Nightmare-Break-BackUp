using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
public class Monster : MonoBehaviour {
	//public DungeonManager dungeonManager;

	public GameObject[] player;
//	public Animator animator;
//	public AnimationState aniState;

	public GameObject targetPlayer;
	//public BattleCarculationManager BCM;
    
	private Vector3 leftVector3 = new Vector3(0,180,0);
	private Vector3 rightVector3 = new Vector3(0,0,0);


	//mode,gateArraynumber,monsterArraynumber
	protected bool moveAble;
	public bool MoveAble{
		get{ return moveAble;}
		set{ moveAble = value;}
	}

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
	[SerializeField]protected bool isAttack;
	private bool isHited;

	public float[] playerToMonsterDamage;
	public float[] aggroRank; //playertoMonsterdamage/currentdistancePlayer;
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

	public GameObject attackCollider;
	public void MonsterSet(){
		isAlive = true;
		isHited = false;
		moveAble = true;
//		animator = this.gameObject.GetComponent<Animator> ();
		StartCoroutine(LookatChange ());
		maxLife = 100;
		currentLife = 100;
//		attackCollider = GameObject.FindGameObjectWithTag("Finish");
//		attackCollider.SetActive (false);
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

	public enum StateDirecion{
		right,
		left
	};
	public StateDirecion stateDirecion;

	IEnumerator LookatChange(){
		while (true) {
			if (!isAttack) {
				if (targetPlayer != null) {
					if ((targetPlayer.transform.position.z - transform.position.z) >= 0) {
						LookAtPattern (StateDirecion.right);
					}
					if ((targetPlayer.transform.position.z - transform.position.z) < 0) {
						LookAtPattern (StateDirecion.left);
					}
				}
			}
			yield return new WaitForSeconds (2f);
		}
	}

	public void AttackEnd(){
		
		StartCoroutine (LookatChange ());
		//Debug.Log ("animator.SetInteger (State, 0)");
		moveAble=true;
		isAttack = false;
		//animator.SetInteger ("State", 0);
		attackCollider.SetActive (false);
		Debug.Log ("AttackEnd");
	}
	public void AttackStart(){
		moveAble = false;
		isAttack = true;
		StopCoroutine (LookatChange ());
		Debug.Log ("AttackStart");
	}
	public void AnimatorReset(){
//		animator.SetInteger ("State", 0);
		isAttack = false;
	}
	public void AttackBlitz()
	{
		attackCollider.SetActive (true);
	}

	public void LookAtPattern(StateDirecion state){
		switch(state){
		case StateDirecion.right: 
			{transform.rotation = Quaternion.Euler(rightVector3);break;}
		case StateDirecion.left:
			{transform.rotation = Quaternion.Euler(leftVector3);break;}
		}
	}
}
