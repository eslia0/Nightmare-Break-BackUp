using UnityEngine;
using System.Collections;

public class Perspective : Sense {

	public int FieldOfView = 30;
	public int ViewDistance = 100;

	private Transform playerTrans;
	private Vector3 rayDirecion;

	protected override void Initialize(){
		playerTrans = GameObject.Find("Player").transform;
	}

	protected override void UpdateSense(){
		elaspedTime += Time.deltaTime;

		//if(elaspedTime >= detectionRate) detecta
	}

	void DetectAspect(){
		RaycastHit hit;
		rayDirecion = playerTrans.position - transform.position;

		if ((Vector3.Angle (rayDirecion, transform.forward)) < FieldOfView) {
			if (Physics.Raycast (transform.position, rayDirecion, out hit, ViewDistance)) {
			//assp
//				if(sdpect != null){
//					if (aspect.aspectName == aspectName) {
//						print ("aa");
//					}
//				}
//			}
		}
	}
	}
}

