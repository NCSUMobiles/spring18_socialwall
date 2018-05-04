using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchStars : MonoBehaviour {

    ParticleSystem ps;

    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> outside = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

    // Update is called once per frame
    void Start () {
        ps = GetComponent<ParticleSystem>();
	}

    private void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
        int numOutside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, outside);
        int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        // iterate through the particles which entered the trigger and make them red
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            p.velocity += new Vector3(0, 1, 0);
            enter[i] = p;
        }

        // re-assign the modified particles back into the particle system
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}
