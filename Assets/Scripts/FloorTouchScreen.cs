using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTouchScreen : MonoBehaviour {

    Vector2 pos;
    ParticleSystem ps;
    List<ParticleSystem> particleSystems; 
	// Use this for initialization
	void Start () {
        ps = GameObject.Find("FeetStars").GetComponent<ParticleSystem>();
        particleSystems = new List<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        // get all touches
        Touch[] touches = Input.touches;
        
        // spawn a particle system at each touch
        for (int i = 0; i < touches.Length; i++)
        {
            ParticleSystem temp = Instantiate(ps);
            temp.transform.position = touches[i].position;
            particleSystems.Add(temp);
        }

        // go through all particle systems and delete the ones that aren't alive
        foreach (ParticleSystem p in particleSystems)
        {
            if (!p.IsAlive())
                Destroy(p);
        }
	}
}
