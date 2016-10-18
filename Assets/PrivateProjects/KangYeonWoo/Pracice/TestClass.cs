using UnityEngine;
using System.Collections;

public class TestClass : MonoBehaviour {

	bool state;
	int type;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		type = Random.Range (0, 3);

//		state = Pattern1 ();
	}

	public bool Pattern1 (int _condition, int _hp, int _sp, int _action){
		return state;
	}

//	public bool pattern2 (int _condition){
//	}

//	public bool pattern3(int _condition){
//	}
}
