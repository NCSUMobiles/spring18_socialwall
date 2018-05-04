using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidPlanet : MonoBehaviour {

    public ParticleSystem p;
    public ParticleSystem.Particle[] particles;
    public float affectDistance;
    float sqrDist;
    Transform thisTransform;


	// Use this for initialization
	void Start () {
        thisTransform = transform;
        p = (ParticleSystem)(GameObject.Find("Stars").GetComponent(typeof(ParticleSystem)));
        sqrDist = affectDistance * affectDistance;

	}
	
	// Update is called once per frame
	void Update () {
        InitializeIfNeeded();
        float dist;
        // GetParticles is allocation free because we reuse the particles buffer between updates
        int numParticlesAlive = p.GetParticles(particles);

        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            dist = Vector3.SqrMagnitude(thisTransform.position - particles[i].position);
            if (sqrDist > dist)
            {
                if (particles[i].position.y > thisTransform.position.y)
                    particles[i].velocity += Vector3.Normalize(Vector3.Lerp(particles[i].position, transform.position, Time.deltaTime));
                else
                    particles[i].velocity -= Vector3.Normalize(Vector3.Lerp(particles[i].position, transform.position, Time.deltaTime));
            }
            if (Vector3.Magnitude(particles[i].totalVelocity) > 3)
            {
                particles[i].velocity = Vector3.ClampMagnitude(particles[i].totalVelocity, 3);
            }
        }

        // Apply the particle changes to the particle system
        p.SetParticles(particles, numParticlesAlive);
        p.SetParticles(particles, particles.Length);
	}

    void InitializeIfNeeded()
    {
        if (p == null)
            p = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < p.main.maxParticles)
            particles = new ParticleSystem.Particle[p.main.maxParticles];
    }
}
