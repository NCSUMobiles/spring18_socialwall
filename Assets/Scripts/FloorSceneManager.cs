using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FloorSceneManager : MonoBehaviour {

    // solar system 
    GameObject IdleScene;
    // planet zoom in
    GameObject PlanetScene;
    // use the active scene (planet) or idle scene(solar system)
    bool active;
    bool transitioning;
    // down time before you can select a new planet
    public float timeBetweenNewSwitch = 10f;
    // time before the scene switchs back to an idle state
    public int timeBeforeIdle = 5;
    // string used to find the correct video clip
    string planetName;
    // variables used to store the videos
    public bool UseVideo;
    public List<VideoClip> planetVideos;
    VideoPlayer player;
    GameObject floorStars;

    // variables used to play background music
    AudioSource bgmMusic;

    // used with 3d models, not videos
    bool timerHasntStarted, usingModel = false;
    bool enlargingPlanet;
    float timeShowingPlanet = 5f, initialVel;
    GameObject planet;
    GameObject ps;
    Vector3 scale, zoomSpeed;

    void Start () {
        IdleScene = GameObject.Find("IdleScene");
        PlanetScene = GameObject.Find("PlanetScene");
        ps = GameObject.Find("StarStreaks");
        PlanetScene.SetActive(false);
        active = transitioning = false;
        scale = new Vector3(4, 4, 4);
        zoomSpeed = new Vector3(.03f, .03f, .03f)/5f;
        planetName = "";
        floorStars = GameObject.Find("StarBackground_floor");
        bgmMusic = gameObject.GetComponent<AudioSource>();
    }

    public void UseIdleScene()
    {
        active = false;
        IdleScene.SetActive(!active);
    }

    public void UseActiveScene()
    {
        active = true;
        IdleScene.SetActive(!active);
    }

    public bool UsingActiveScene()
    {
        return active;
    }

    public void PlanetSelected(string name)
    {
        // set the zoom in scene as active and the action
        PlanetScene.SetActive(true);
        // set flag to true so it only happens once
        transitioning = true;
        // switch scenes 
        UseActiveScene();
        planetName = name;
        // make video player on camera
        player = gameObject.AddComponent<VideoPlayer>();

        player.source = VideoSource.VideoClip;
        player.playOnAwake = false;
        player.waitForFirstFrame = true;
        player.targetCamera = GameObject.Find("FloorCamera").GetComponent<Camera>();
    }
    
    // go from planet zoom in to active state
    public void TransitionBackToActive()
    {
        enlargingPlanet = false;
        ps.SetActive(true);
    }

    public bool GetTransitionState()
    {
        return transitioning;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            UseVideo = !UseVideo;
        PlayMusic();
        if (usingModel)
        {
            UsePlanetModel();
            return;
        }

        // if active and no planet selected for a few seconds, set idle
        if (planetName == "")
        {
            // TODO doesnt work
            //StartCoroutine(WaitSeconds(timeBeforeIdle)); 
            return;
        }

        // planet has been selected
        if (UseVideo && GetVideo(planetName))
        {
            ps.SetActive(false);
            player.Play();
            StartCoroutine(WaitForVideo());
            planetName = "";
        }
        else
        {
            // there isnt a video for this planet
            SelectPlanetModel();

        }

        //UsePlanetModel();
    }

    public IEnumerator CheckForActionBeforeIdle(int sec)
    {
        for (int i = 0; i < sec; i++)
        {
            // check every second
            yield return new WaitForSeconds(1);
            if (planet != null)
               yield return null;
        }
        UseIdleScene();
    }

    public bool GetVideo(string name)
    {
        Debug.Log("Planet selected: " + planetName);
        foreach (VideoClip v in planetVideos)
        {
            if (v.name.ToLower() == planetName.ToLower())
            {
                player.clip = v;
                floorStars.SetActive(false);
                return true;
            }
        }
        return false;
    }

    private IEnumerator WaitForVideo()
    {
        // wait a second so the video can start playing
        yield return new WaitForSeconds(1);
        // loop until the video is completed
        while (player.isPlaying)
        {
            yield return 0;
        }
        floorStars.SetActive(true);
        UseIdleScene();
        PlanetScene.SetActive(false);
        Destroy(gameObject.GetComponent<VideoPlayer>());
        yield return new WaitForSeconds(timeBetweenNewSwitch);
        transitioning = false;

    }

    private void SelectPlanetModel()
    {
        timerHasntStarted = true;
        // create the planet and move it to the center 
        planet = Instantiate(GameObject.Find(planetName));
        Destroy(planet.GetComponentInChildren<Light>());
        if (planetName == "Earth")
        {
            GameObject planetPair = new GameObject();
            GameObject moon = Instantiate(GameObject.Find("Moon"));
            Destroy(moon.GetComponentInChildren<Light>());
            moon.transform.SetParent(planetPair.transform);
            moon.transform.localPosition += new Vector3(-0.3f, 0.3f, 0);
            planet.transform.SetParent(planetPair.transform);
            planet.transform.localPosition = Vector3.zero;
            planet = planetPair;
        } else if (planetName == "Moon")
        {
            GameObject planetPair = new GameObject();
            GameObject moon = Instantiate(GameObject.Find("Earth"));
            Destroy(moon.GetComponentInChildren<Light>());
            moon.transform.SetParent(planetPair.transform);
            moon.transform.localPosition += new Vector3(0.3f, -0.3f, 0);
            planet.transform.SetParent(planetPair.transform);
            planet.transform.localPosition = Vector3.zero;
            planet = planetPair;
        }
        else
        {
            planet.transform.position = Vector3.zero;
        }
        planet.transform.localScale = Vector3.zero;

        if (planetName == "Moon")
        {
            // moon scales weird when it is tied to earth. 
            scale = new Vector3(7, 7, 7);
        }
        else
        {
            scale = new Vector3(4, 4, 4);
        }

        ps.SetActive(true);
        initialVel = ps.GetComponent<ParticleSystem>().main.startSpeed.constant;
        enlargingPlanet = true;
        planet.layer = 0;
        usingModel = true;
    }

    private void UsePlanetModel()
    {
        if (enlargingPlanet)
        {
            if (planet.transform.localScale.x < 1)
            {
                planet.transform.localScale += zoomSpeed * 2;
            }
            else if (planet.transform.localScale.x < scale.x)
            {
                planet.transform.localScale += zoomSpeed * 2 * Mathf.Pow(planet.transform.localScale.x, 2);
            }

            if (planet.transform.localScale.x >= scale.x && timerHasntStarted)
            {
                // start timer to switch back to idle scene
                StartCoroutine(ShowPlanetTime());
                ps.SetActive(false);
                timerHasntStarted = false;
            }
        }
        else
        {
            // initial move to bottom left corner
            if (planet.transform.position.y > -15f)
            {
                planet.transform.localScale += zoomSpeed * 5;
                planet.transform.position += new Vector3(-.075f, -.075f, 1);
            }
            else
            {
                Destroy(planet);
                enlargingPlanet = true;
                ps.SetActive(false);
                usingModel = false;
                planetName = "";
                StartCoroutine(TimeBeforeNewSwitch());
                UseIdleScene();
                Destroy(gameObject.GetComponent<VideoPlayer>());
            }

        }
    }
    // used with planet models, not videos
    private IEnumerator ShowPlanetTime()
    {
        yield return new WaitForSeconds(timeShowingPlanet);
        TransitionBackToActive();
    }

    private IEnumerator TimeBeforeNewSwitch()
    {
        yield return new WaitForSeconds(timeBetweenNewSwitch);
        transitioning = false;
    }

    private void PlayMusic()
    {
        if (bgmMusic.isPlaying)
            return;
        bgmMusic.clip = gameObject.GetComponent<AudioHolder>().GetRandomAudio();
        bgmMusic.Play();
    }
}
