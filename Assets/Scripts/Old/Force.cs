using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().AddForce(0, 100, 0, ForceMode.Impulse);
	}

	// Update is called once per frame
	void Update () {

	}
}
