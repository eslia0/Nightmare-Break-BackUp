using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float speed;
	public Rigidbody rigid;
	public float desTime;
	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody> ();
		rigid.velocity = transform.forward * speed;
	}
	
	// Update is called once per frame
	void Update () {
		Destroy (gameObject, desTime);
	}
}
