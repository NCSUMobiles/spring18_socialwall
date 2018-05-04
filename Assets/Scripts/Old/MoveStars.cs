using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MoveStars : MonoBehaviour
{
    public ParticleSystem p;
    public ParticleSystem.Particle[] particles;
    public ParticleSystem.TrailModule trails;
    public float affectDistance;
    float angle, dist;
    double deleteDist;
    Transform thisTransform;
    Vector3 pos, closestPoint, fVector;

    void Start()
    {
        thisTransform = transform;
        p = (ParticleSystem)(GameObject.Find("BackgroundStars").GetComponent(typeof(ParticleSystem)));
        deleteDist = 0;
    }

    void Update()
    {
        InitializeIfNeeded();
        int numParticles = p.GetParticles(particles);

        Vector2 loc = Input.mousePosition;
        Vector3 vec = Camera.main.ScreenToWorldPoint(new Vector3(loc.x, loc.y, 20f));
        transform.position = vec;

        for (int i = 0; i < numParticles; i++)
        {
            dist = Vector2.Distance(thisTransform.position, particles[i].position);
            // delete particles in the small distance so that they do not freak out and streak across screen
            if (deleteDist > dist)
            {
                particles[i].remainingLifetime = 0.01f;
                p.SetParticles(particles, particles.Length);
            }
            // if particle in range of sphere, gravity it
            else if (affectDistance > dist)
            {
                particles[i] = DoMath(particles[i]);
                p.SetParticles(particles, particles.Length);
            }

        }
    }

    void InitializeIfNeeded()
    {
        if (p == null)
            p = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < p.main.maxParticles)
            particles = new ParticleSystem.Particle[p.main.maxParticles];

    }

    private ParticleSystem.Particle DoMath(ParticleSystem.Particle par)
    {
        pos = par.position;
        //closestPoint = Physics.ClosestPoint(pos, thisTransform.GetComponent<SphereCollider>(), thisTransform.position, new Quaternion());
        //dist = Vector3.Magnitude(closestPoint);
        //angle = 1 - (float)Math.Sin(Vector3.Angle(thisTransform.transform.position, pos));//, Vector3.right));
        fVector = Vector3.up * 0.012f;// * angle;



        // get distance to the surface of the sphere
        if (pos.y > thisTransform.transform.position.y)
        {
            if (pos.x - thisTransform.transform.position.x < -0.2)
                // top left
                pos += fVector;
            else if (pos.x - thisTransform.transform.position.x > 0.2)
                // top right
                pos -= fVector;

        }
        else
        {
            if (pos.x - thisTransform.transform.position.x < -0.2)
                // top left
                pos -= fVector;
            else if (pos.x - thisTransform.transform.position.x > 0.2)
                // bottom right
                pos += fVector;
        }
        par.position = pos;
        return par;


    }
}