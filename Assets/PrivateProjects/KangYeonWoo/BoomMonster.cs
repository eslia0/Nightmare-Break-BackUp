using UnityEngine;
using System.Collections;

public class BoomMonster : Monster {
	public enum StateDirecion{
		right,
		left
	};
	Animator a;

	private float perceive = 6.0f;
	private float moveSpeed = 1f;

	private float currentDisTance;
	private Vector3 checkDirection;

	private Vector3 movePoint;
	private Vector3 idlePoint;

	private Vector3 boomPoint = new Vector3(100,100,100);
	private Vector3 attackPoint = new Vector3 (1, 0, 0);

	public enum StatePosition
	{
		Idle=1,
		Run,
		Attack,
		Boom,
		Death
	};


	//animation Set; move;
	public void Pattern(StatePosition state){
		switch(state){
		case StatePosition.Idle: 
			{this.transform.Translate (idlePoint, 0);break;}
		case StatePosition.Boom:
			{idlePoint = this.gameObject.transform.position;
				StartCoroutine("BoomCoroutine");break;}
		case StatePosition.Attack:
			{this.transform.Translate (attackPoint, 0);break;}
		case StatePosition.Run:
			{
			this.transform.Translate (movePoint*moveSpeed*Time.deltaTime, 0);
			break;
			}
		case StatePosition.Death:
			{this.gameObject.SetActive (false);	break;}
		}
	}

	IEnumerator BoomCoroutine() {
		monster.gameObject.transform.position = idlePoint;
		yield return new WaitForSeconds (3f);
		monster.gameObject.transform.position = boomPoint;
		Pattern (StatePosition.Death);
		yield return new WaitForSeconds (0.5f);

		StopCoroutine (BoomCoroutine());
	}

	void Start(){
		PlayerSuch ();
		MonsterSet ();
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
					{
						//Pattern (StatePosition.Boom);
					}	
				}
			}

		}
		if (!IsAlive) {
			Pattern (StatePosition.Death);
		}
	}

	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
			Debug.Log (coll.gameObject.transform.parent);
			BCM.DamageCarculateProcess (coll.gameObject.transform.parent.gameObject, this.gameObject, coll.gameObject);//this method need conference;

		}
		//if(coll.gameObject.name == "sword"){Debug.Log ("hit");}
	}


}
