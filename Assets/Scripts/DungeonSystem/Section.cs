using UnityEngine;
using System.Collections;

public class Section : MonoBehaviour {
	public DungeonManager dungeonManager;
	public BoomMonster[] boomMonster;
	public WarriroMonster[] warriorMonster;

	[SerializeField] int monsterCount;
	[SerializeField]private int gateNumber;
	public int GateNumber{
		get { return gateNumber;}
		set{ gateNumber = value;}
	}
	//all monster objects getting;
	//public GameObject[] monsterCrowd;

	protected bool modeForm;
	public bool ModeForm{
		get {return modeForm;}
		set {modeForm = value;}
	}

	public void MonsterSet(){
		dungeonManager = gameObject.transform.parent.GetComponent<DungeonManager>();
		boomMonster = gameObject.transform.GetComponentsInChildren<BoomMonster> ();
//		warriorMonster = gameObject.GetComponentsInChildren<WarriroMonster> ();
		monsterCount = (boomMonster.Length + warriorMonster.Length);

		for (int i = 0; i < boomMonster.Length; i++) {
			boomMonster [i].PlayerSearch ();
			boomMonster [i].MonsterSet ();
			boomMonster [i].Mode = modeForm;
			boomMonster [i].GateArrayNumber = gateNumber;
			boomMonster [i].MonsterArrayNumber = i;
		}

	}

	public void UpdateConduct(){
		for (int i = 0; i < boomMonster.Length; i++) {
			boomMonster [i].UpdateConduct ();
//			warriorMonster [i].UpdateConduct ();
		}
	}

	public void SetFalse(){
		this.gameObject.SetActive (false);
	}



	public void RemoveMonsterArray(){
		monsterCount -= 1;
		if (monsterCount == 0) {
			dungeonManager.mapNumberChange ();
		}

	}


}
