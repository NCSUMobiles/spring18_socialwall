using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSceneManager : MonoBehaviour {
    // List of all possible assets to spawn
    List<GameObject> assets;
    List<GameObject> hands;
    FloorSceneManager floor;
    
    // Semophore to keep track of the current state of events
    State state;
    bool hasTimerStarted;
    bool floorAsset;

    // GameObject to spawn next
    GameObject currentAsset;
    Vector3 assetVelocity;

    // Variables to fade the asset in/out
    SpriteRenderer assetSprite;
    Color c;

    // How long to keep each event going
    // TODO maybe make this asset-dependent
    public int timeOfAssetEvent = 20;

    // how long to wait in between each event
    // TODO maybe make this asset-dependent
    public int timeBetweenEvents = 5;

    public GameObject HandObject;

    Color[] colors;

    private enum State
    {
        waiting = 0,
        spawning,
        moving,
        deleting,
    }

    // Use this for initialization
    void Start () {
        assets = new List<GameObject>();
        // store all child assets in the array
        foreach (Transform t in GameObject.Find("TimedEvents").GetComponentsInChildren<Transform>())
        {
            if (t.name != "TimedEvents" && t.name != "AssetParticles")
            {
                assets.Add(GameObject.Find(t.name));
                Debug.Log(t.name);
                assets[assets.Count-1].SetActive(false);
            }
        }

        state = State.waiting;
        hasTimerStarted = false;

        hands = new List<GameObject>
        {
            GameObject.Find("HandObject")
        };

        colors = new Color[4];
        colors[0] = Color.red;
        colors[1] = Color.blue;
        colors[2] = Color.green;
        colors[3] = Color.yellow;

        floor = GameObject.Find("FloorCamera").GetComponent<FloorSceneManager>();


    }
	
	// Update is called once per frame
	void Update () {
        // spawn more hands
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddHand();
        }
        
        switch (state)
        {
            case State.waiting:
                if (hasTimerStarted)
                    return;
                else
                    StartCoroutine(StartSpawnTimer());
                break;
            case State.spawning:
                if (floorAsset && floor.UsingActiveScene())
                    return;
                SpawnAsset();
                break;
            case State.moving:
                if (hasTimerStarted)
                    MoveAsset();
                else
                    StartCoroutine(StartMoveTimer());
                break;
            case State.deleting:
                DeleteAsset();
                break;
            default:
                // shouldn't ever hit this, but if it does, set state to spawning
                state = State.waiting;
                break;
        }
	}

    private IEnumerator StartSpawnTimer()
    {
        // randomly select an asset
        currentAsset = assets[Random.Range(0, assets.Count)];

        Debug.Log("Next asset: " + currentAsset.name);
        floorAsset = Random.Range(0, 3) == 2;
        Debug.Log("Spawning on the Floor: " + floorAsset);
        // wait the time
        hasTimerStarted = true;
        yield return new WaitForSeconds(timeBetweenEvents);
        // change the state
        state = State.spawning;
        hasTimerStarted = false;

    }
    private void SpawnAsset()
    {
        // instantiate a new asset
        currentAsset = Instantiate(currentAsset);
        currentAsset.SetActive(true);
        foreach (Transform t in currentAsset.GetComponentsInChildren<Transform>())
        {
            if (floorAsset)
            {
                t.localScale /= 2;
                t.gameObject.layer = 10;
                currentAsset.transform.parent = GameObject.Find("FloorCamera").transform;
            }
            else
            { 
                t.gameObject.layer = 8;
                currentAsset.transform.parent = GameObject.Find("WallCamera").transform;
            }
        }
        // get the color so that it can fade in/out
        assetSprite = currentAsset.GetComponent<SpriteRenderer>();
        c = assetSprite.material.color;
        c.a = 0f;
        assetSprite.material.color = c;

        // randomly pick spawn location and velocity
        bool xLeft = Random.Range(0, 2) == 0;
        bool yAbove = Random.Range(0, 2) == 0;
        float xMod = 1, yMod = 1;
        float xCoord, yCoord;
        if (floorAsset) {
            xCoord = Random.value * 3;
            yCoord = Random.value * 7;
        }
        else
        {
            xCoord = Random.value * 10;
            yCoord = Random.value * 10;
        }
        if (xLeft)
            xMod *= -1;
        if (yAbove)
            yMod *= -1;

        currentAsset.transform.localPosition = new Vector3(xMod*xCoord, yMod*yCoord, 5);
        if (currentAsset.name.ToLower().Contains("comet"))
        {
            // if its a comet, make it go faster
            xMod *= 2.5f;
            yMod *= 2.5f;
        }
        assetVelocity = new Vector3(0.01f * -xMod, 0.01f * -yMod, 0);

        // rotate towards direction of movement
        float angle = Mathf.Atan2(-assetVelocity.x, assetVelocity.y) * Mathf.Rad2Deg;
        currentAsset.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // start fading in
        StartCoroutine(FadeAsset(true));

        Debug.Log("Position of asset: " + currentAsset.transform.localPosition);

        // pass the transform to the mouse pointer script so it knows what to interact with            // update all hands to remove the asset
        foreach (MouseDraggable m in GameObject.Find("Hands").GetComponentsInChildren<MouseDraggable>())
            m.UpdateAsset(currentAsset);
        state = State.moving;

    }

    private IEnumerator StartMoveTimer()
    {
        hasTimerStarted = true;
        yield return new WaitForSeconds(timeOfAssetEvent);
        if (state == State.moving)
            state = State.deleting;
    }

    private void MoveAsset()
    {
        if (floorAsset && floor.UsingActiveScene())
            state = State.deleting;
        currentAsset.transform.position += assetVelocity;
    }

    private void DeleteAsset()
    {
        // switch the starting and ending colors so it fades out and then delete it
        StartCoroutine(FadeAsset(false));
        state = State.waiting;
        hasTimerStarted = false;
    }

    private IEnumerator FadeAsset(bool fadeIn)
    {
        float step = 0.05f;
        if (fadeIn)
        {
            for (float f = step; f <= 1f; f += step)
            {
                c = assetSprite.material.color;
                c.a = f;
                assetSprite.material.color = c;
                yield return new WaitForSeconds(step);
            }
        } else
        {
            GameObject old = currentAsset;
            // kill the particle system if it has one
            if (old.GetComponentInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule e = old.GetComponentInChildren<ParticleSystem>().emission;
                e.rateOverTime = 0;
            }
            for (float f = 1f; f >= step; f -= step)
            {
                c = assetSprite.material.color;
                c.a = f;
                assetSprite.material.color = c;
                old.transform.position += assetVelocity;
                yield return new WaitForSeconds(step);
            }
            Destroy(old);
            // update all hands to remove the asset
            foreach (MouseDraggable m in GameObject.Find("Hands").GetComponentsInChildren<MouseDraggable>())
                m.UpdateAsset(null);
        }
    }

    public List<Transform> GetHands()
    {
        List<Transform> newList = new List<Transform>();
        foreach (GameObject g in hands)
        {
            newList.Add(g.transform);
        }
        return newList;
    }

    public void AddHand()
    {
        GameObject newHand = Instantiate(HandObject);
        newHand.transform.parent = GameObject.Find("Hands").transform;
        newHand.transform.localPosition = new Vector3(0, 0, 0);
        hands.Add(newHand);
        ParticleSystem.MainModule p = newHand.GetComponentInChildren<ParticleSystem>().main;
        p.startColor = colors[Mathf.RoundToInt(Random.Range(0, colors.Length))];

        foreach (PlanetRotation planet in GameObject.Find("WallCamera").GetComponentsInChildren<PlanetRotation>())
        {
            planet.AddHand(newHand.transform);
        }
    }

    public string GetState()
    {
        return state.ToString();
    }

}
