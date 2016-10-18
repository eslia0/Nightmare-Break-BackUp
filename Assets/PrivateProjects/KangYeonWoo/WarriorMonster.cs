using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class WarriroMonster : Monster {
	public enum StateDirecion{
		right,
		left
	};
	Animator a;

	private float perceive = 6.0f;
	private float moveSpeed = 2f;

	private float currentDisTance;
	private Vector3 checkDirection;

	private Vector3 movePoint;
	private Vector3 idlePoint;



	private Vector3 attackPoint = new Vector3 (1, 0, 0);

	public enum StatePosition
	{
		Idle=1,
		Run,
		Attack,
		Death
	};


	//animation Set; move;
	public void Pattern(StatePosition state){
		switch(state){
		case StatePosition.Idle: 
			{this.transform.Translate (idlePoint, 0);break;}
		case StatePosition.Attack:
			{this.transform.Translate (attackPoint, 0);break;}
		case StatePosition.Run:
			{
				this.transform.Translate (movePoint*moveSpeed*Time.deltaTime, 0);
				break;
			}
		case StatePosition.Death:
			{this.gameObject.SetActive (false);break;}
		}

	}



	void Start(){
		StartCoroutine(CoChasePlayer());
	}
	//chaseplayer= method; cochaseplayer = coroutine;


	public void UpdateConduct(){
		if (IsAlive) {
			currentDisTance = Vector3.Distance (perceivePlayer.transform.position, this.gameObject.transform.position);
			checkDirection = perceivePlayer.transform.position - this.gameObject.transform.position;

			//if this object get Attackmotion pattern(stateposition.boom -> attack), and this monsterlife is 20%, boomPattern start;
			if (currentDisTance <= perceive) {
				movePoint = new Vector3 (checkDirection.x, 0, checkDirection.z);

				if (currentDisTance >= perceive * 0.2f) {
					Pattern (StatePosition.Run);
				}
				if (currentDisTance < perceive * 0.2f) {
					{Pattern (StatePosition.Attack);}	
				}
			}


		}
		if (!IsAlive) {
			Pattern (StatePosition.Death);
		}
	}

	void HitedChasePlayer(){
//		if (IsHited) {
//			changeTargetTime += Time.deltaTime;
//			if (changeTargetTime < 5) {
//
//			}
//			if (changeTargetTime >= 5) {
//				changeTargetTime = 0;
//
//				StartCoroutine(CoChasePlayer());
//			}
//		}
	}




	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.layer == LayerMask.NameToLayer ("Weapon")) {
			
			for (int i = 0; i < player.Length; i++) {
				if (player [i].name == coll.gameObject.name) {
					perceivePlayer = coll.gameObject;// getcomponemt<player>()
					StartCoroutine(CoHitedChasePlayer());
				}
			}
	 	}
	}

}
