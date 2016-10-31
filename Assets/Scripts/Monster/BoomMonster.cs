using UnityEngine;
using System.Collections;


public class BoomMonster : Monster {
	public enum StateDirecion{
		right,
		left
	};

	private float searchRange = 6.0f;
	private float moveSpeed = 1f;

	private float currentDisTance;


	private Vector3 movePoint;
	private Vector3 idlePoint = new Vector3(0,0,0);

	private Vector3 boomPoint = new Vector3(100,100,100);
	private Vector3 attackPoint = new Vector3 (1, 0, 0);

	public enum StatePosition
	{
		Idle=1,
		Run,
		Attack,
		Boom,
		TakeDamage,
		Death
	};


	public void AnimatorReset(){
		animator.SetInteger ("State", 0);
	}

    //animation Set; move;
    public void Pattern(StatePosition state)
    {
        switch (state)
        {
            case StatePosition.Idle:
                {
                    this.transform.Translate(idlePoint * Time.deltaTime, 0);
                    animator.SetInteger("State", 0);
                    break;
                }
            case StatePosition.Boom:
                {
                    idlePoint = this.gameObject.transform.position;
                    StartCoroutine("BoomCoroutine"); break;
                } // animator boom -> setintter 4
            case StatePosition.Attack:
                {
                    AttackProcess();
                    break;
                }
            case StatePosition.Run:
                {
                    AnimatorReset();
                    this.transform.Translate(movePoint * moveSpeed * Time.deltaTime, 0);
                    animator.SetInteger("State", 2);
                    break;
                }

            case StatePosition.TakeDamage:
                {
                    animator.SetTrigger("TakeDamage");
                    AnimatorReset();
                    break;
                }
            case StatePosition.Death:
                {
                    MonsterArrayEraser(this.gameObject);
                    break;
                }
        }
    }

	IEnumerator BoomCoroutine() {
		AnimatorReset ();
		transform.position = idlePoint;
		animator.SetInteger ("State", 4);
		yield return new WaitForSeconds (3f);
		animator.SetTrigger("Death");
		yield return new WaitForSeconds (3f);
		transform.position = boomPoint;
		Pattern (StatePosition.Death);
		StopCoroutine (BoomCoroutine());
	}

//	void Start(){
//		PlayerSuch ();
//		MonsterSet ();
//
//	}
	//chaseplayer= method; cochaseplayer = coroutine;


	void AttackProcess(){
		AnimatorReset ();
		attackCycle += Time.deltaTime;

			if (attackCycle >= 1) {
			if(currentDisTance <= 0.2f)
			{
				transform.Translate (movePoint * moveSpeed * Time.deltaTime, 0);
				Debug.Log (movePoint);
			}
			animator.SetInteger ("State", 3);

			isAttack = true;
				//a.SetTrigger();

			}

			if (attackCycle > 2.5f) {
			AnimatorReset ();
				attackCycle = 0;

		}
	}

	public void LookAtPattern(StateDirecion state){
		switch(state){
		case StateDirecion.right: 
			{Debug.Log ("a"); break;}// turn animator left;right
		case StateDirecion.left:
			{Debug.Log ("b"); break;}// turn animator left;right
		}
	}

	void LookatChange(){
		if (!isAttack) {
			if ((targetPlayer.transform.position.x - transform.position.x) >= 0) {
				LookAtPattern (StateDirecion.right);
			}
			if ((targetPlayer.transform.position.x - transform.position.x) < 0) {
				LookAtPattern (StateDirecion.left);
			}
		}
	}

    public void UpdateConduct()
    {
        if (IsAlive)
        {
            ChasePlayer();// playerchase;
                          //LookatChange ();//monsterlookatcontrol;
            if (targetPlayer != null)
            {
                currentDisTance = Vector3.Distance(targetPlayer.transform.position, this.gameObject.transform.position);
                checkDirection = targetPlayer.transform.position - this.gameObject.transform.position;

                if (currentDisTance > searchRange)
                {
                    Pattern(StatePosition.Idle);
                }
                //if this object get Attackmotion pattern(stateposition.boom -> attack), and this monsterlife is 20%, boomPattern start;
                if (currentDisTance <= searchRange)
                {
                    movePoint = new Vector3(checkDirection.x, 0, checkDirection.z);

                    if (currentDisTance >= searchRange * 0.2f)
                    {
                        Pattern(StatePosition.Run);
                    }
                    if (currentDisTance < searchRange * 0.2f)
                    {
                        {
                            Pattern(StatePosition.Attack);
                        }
                    }
                    if (currentLife / maxLife < 0.2)
                    {
                        Pattern(StatePosition.Boom);
                    }
                }
            }
        }
        if (!IsAlive)
        {
            Pattern(StatePosition.Death);
        }
    }

	//AttackMotion end
	public void AttackBlitz()
	{
		Debug.Log("biltz");
		Debug.Log (movePoint);
		if(Vector3.Distance(movePoint,this.gameObject.transform.position)>0.5f)
		{
			movePoint=movePoint*0 ;
		}
	}




	 
	void OnTriggerEnter(Collider coll){
		Pattern (StatePosition.TakeDamage);
		Debug.Log ("hit");
		if (coll.gameObject.layer == LayerMask.NameToLayer("Weapon")) {
			Debug.Log (coll.gameObject.transform.parent);
			//BCM.DamageCarculateProcess (coll.gameObject.transform.parent.gameObject, this.gameObject, coll.gameObject);//this method need conference;
			//BCM.DamageCarculateProcess(coll.transform.parent.GetComponent<CharcterPlayer>())

			//BCM.DamageCarculateProcess (coll.gameObject.transform.parent.GetComponent<CharcterPlayer> (), this.gameObject.GetComponent<Monster> (), coll.gameObject);
		}
		//if(coll.gameObject.name == "sword"){Debug.Log ("hit");}
	}





}
