using UnityEngine;
using System.Collections;

public class Sense : MonoBehaviour {
	public bool bDebug = true;
	//public Aspect.aspect aspectName = Aspect.aspect.Enemy;
	public float detectionRate = 1.0f;

	protected float elaspedTime = 0.0f;

	protected virtual void Initialize(){}
	protected virtual void UpdateSense(){}

	void Start(){
		elaspedTime = 0.0f;
		Initialize ();
	}

	void Update(){
		UpdateSense ();
	}

}
