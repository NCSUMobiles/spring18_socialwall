using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetBehavior : MonoBehaviour
{
    /*
     * This file is for the planets on the ground to 
     * orbit around the sun. Currently the idle scene
     */

    // Object to rotate around
    public GameObject sun;
    GameObject halo;
    public float AU = 1;
    // How fast to rotate
    public float earthDaysPerYear = 365.25f;
    // radius in miles
    public float planetRadius = 3959;
    public float sizeMultiplier = 1;
    public float distModifier = 1;
    float scaledRadius;
    float alpha;
    public float X;
    public float Y;
    bool hasNotUpdatedAU = true;


    float sunRad = 432288;

    private void Start()
    {
        //// set up light
        //halo = Instantiate(GameObject.Find("SelectedLight")) as GameObject;
        //halo.transform.parent = gameObject.transform;
        //halo.GetComponent<Light>().range = 1f;
        //halo.SetActive(false);

        if (name == "Sun_floor")
            return;

        // set up radius and scale
        scaledRadius = planetRadius * sizeMultiplier / sun.GetComponent<PlanetBehavior>().planetRadius;
        transform.localScale = new Vector3(scaledRadius, scaledRadius, scaledRadius);
    }

    // Update is called once per frame
    void Update () {
        if (name == "Sun_floor")
            return;
        if (hasNotUpdatedAU)
            UpdateAU();
        alpha += 3f * 365.25f / earthDaysPerYear;
        X = ((AU) * Mathf.Cos(alpha * 0.005f));
        Y = ((AU) * Mathf.Sin(alpha * 0.005f));
        transform.position = sun.transform.position + new Vector3(X, Y, 0);
    }

    void UpdateAU()
    {
        //Destroy(GameObject.Find("SelectedLight"));            
        // account for radius of sun
        AU += sun.transform.localScale.x/2;
        AU /= distModifier;
        hasNotUpdatedAU = false;
    }

}
