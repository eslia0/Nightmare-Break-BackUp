using UnityEngine;
using System.Collections;

public class ASDF : MonoBehaviour {
	public Transform sunrise;
	public Transform sunset;
	public float journeyTime = 1.0F;
	public Vector3 testV3;
	private float startTime;
	[SerializeField]float fracComplete;
	void Start() {
		sunrise = this.gameObject.transform;

		startTime = Time.time;
	}
	void Update() {
		Vector3 center = (sunrise.position + sunset.position) * 0.5F;

		Vector3 riseRelCenter = sunrise.position-center;
		Vector3 setRelCenter = sunset.position-center;
		center -= new Vector3(0,0,0);
		fracComplete = (Time.time - startTime) / journeyTime;
		testV3 = Vector3.Slerp (riseRelCenter, setRelCenter, journeyTime);
		transform.Translate (testV3*Time.deltaTime*Time.deltaTime);
		//transform.position = Vector3.Slerp(riseRelCenter*0.5f, setRelCenter*0.5f, fracComplete*Time.deltaTime);
		if (transform.position.x == sunset.position.x) {
			Debug.Log ("a");
		}
		transform.position += center;
	}
}
