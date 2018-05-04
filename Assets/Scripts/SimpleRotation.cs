using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour {

    public float speed = 1;

    // Update is called once per frame
    void Update () {
        transform.Rotate(transform.up, speed, Space.World);
    }
}
