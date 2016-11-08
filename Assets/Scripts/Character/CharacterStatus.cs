using UnityEngine;

[System.Serializable]
public class CharacterStatus
{
	//char status
	//name
	public string characterName;
	public int characterNum;

	//Level
	public int characherLevel;

	//exp
	public int demendExp;
	public int needExp;

	//Point
	public int healthPoint;
	public int magicPoint;

	//char AttackPoint and defensepoint
	public int attack;
	public int defense;

	//critical point
	public float critical;

	public int dreamStone;



	//char stat

	//public bool Alive{get {return this.alive;}}

	public float[] skillCoolTime;

	public string CharacterName {get {return this.characterName;}}

	public int CharacherLevel {	get {return this.characherLevel;}}

	public int DemendExp { get {return this.demendExp;}	}

	public int NeedExp {get {return this.needExp;}}

	public int HealthPoint {get {return this.healthPoint;}
		set {healthPoint = value;}}
	
	public int ManaPoint {get {return this.magicPoint;}}

	public int Attack {get {return this.attack;}}

	public int Defense {get {return this.defense;}}

	public float Critical {get {return this.critical;}}

	public int DreamStone {	get {return this.dreamStone;}}

	public float[] SkillCoolTime {get {return this.skillCoolTime;}}
}
