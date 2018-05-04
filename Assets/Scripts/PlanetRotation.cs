using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour {

    /*
     * This file is used for the rotation of the planets
     * on the wall scene
     */

    public int SolarDayHours;
    public int rotationRank;
    float scaledSpeed;
    bool planetSelected;
    GameObject ps;
    ParticleSystem.EmissionModule e;
    Vector3 up, normalScale;
    Camera wallCamera;
    //Ray ray;
    List<Transform> hands;
    Light halo;
    FloorSceneManager floor;

    int particleRate = 50;
    float radius, range;

    // Use this for initialization
    void Start()
    {
        wallCamera = GameObject.FindGameObjectWithTag("WallCamera").GetComponent<Camera>();
        floor = GameObject.Find("FloorCamera").GetComponent<FloorSceneManager>();

        if (!name.ToLower().Contains("clone") && !name.ToLower().Contains("floor")
            && !name.ToLower().Contains("clouds"))
        {
            Light l = Instantiate(GameObject.Find("SelectedLight").GetComponent<Light>());
            l.transform.localPosition = transform.position + new Vector3(0,0,10);
            l.transform.parent = GameObject.Find("Lights").transform;
            l.name = string.Format("Light({0})", name);
            if (name == "Sun")
                l.GetComponent<Light>().range = 15;
            else 
                l.GetComponent<Light>().range = gameObject.transform.localScale.x / .667f;
            if (transform.localScale.x < 0.5)
                l.GetComponent<Light>().range *= 1.5f;
            range = l.GetComponent<Light>().range;
            halo = l;
        }
        normalScale = transform.localScale;
        ps = GameObject.Find("HandStars");
        e = ps.GetComponent<ParticleSystem>().emission;
        planetSelected = false;
        //if (SolarDayHours == 0)
        //    return;

        //scaledSpeed = 24f / SolarDayHours;
        scaledSpeed = 11-rotationRank;
        scaledSpeed /= 7;
        up = gameObject.transform.up;
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;

        // get all the current hands
        hands = new List<Transform>();
        StartCoroutine(WaitForHands());
    }
    
	// Update is called once per frame
	void Update () {
        transform.Rotate(up, -scaledSpeed, Space.World);
        if (name.ToLower().Contains("clouds") || name.ToLower().Contains("floor"))
            return;
        if (!floor.GetTransitionState())
        {
            foreach (Transform t in hands)
            {
                if (Vector3.Distance(t.position, transform.position) < radius)
                {
                    // at least one is touching it, light up
                    TouchPlanet();
                    planetSelected = true;
                    return;
                }
            }
            // none are touching it, unlight it
            StopTouchingPlanet();
            planetSelected = false;
        }
    }

    public void TouchPlanet()
    {
        StartCoroutine(PlanetSelect());
        transform.localScale = normalScale * 1.2f;
        e.rateOverTime = 0;
    }

    public void StopTouchingPlanet()
    {
        transform.localScale = normalScale;
        e.rateOverTime = particleRate;
    }

    public void AddHand(Transform t)
    {
        hands.Add(t);
    }
    public void RemoveHand(Transform t)
    {
        hands.Remove(t);
    }

    // Timers
    private IEnumerator PlanetSelect()
    {
        for (float i = 0; i < 1.5f; i += 0.25f)
        {
            if (!planetSelected || floor.GetTransitionState())
            {
                // planet is no longer selected.
                halo.GetComponent<Light>().range = range;
                yield break;
            }
            // this gets called an additional time each frame
            // so it expands exponentially
            halo.GetComponent<Light>().range += 0.008f;
            yield return new WaitForSeconds(0.25f);
        }
        if (planetSelected && !floor.GetTransitionState())
        {
            floor.PlanetSelected(name);
            transform.localScale = normalScale;
            e.rateOverTime = particleRate;
        }
    }

    private IEnumerator WaitForHands()
    {
        yield return new WaitForSeconds(1f);
        hands = GameObject.Find("WallCamera").GetComponent<WallSceneManager>().GetHands();
    }
}
