using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDraggable : MonoBehaviour {

    public Camera camera;
    float timeBetweenInteraction = 2f;
    bool interacting = false;
    GameObject currentAsset;

    private void OnMouseDrag()
    {
        MoveHand(Input.mousePosition);
    }

    public void MoveHand(Vector2 loc)
    {
        Vector3 vec = camera.ScreenToWorldPoint(new Vector3(loc.x, loc.y, 10f));
        //vec.y += 10;
        transform.position = vec;
        //get current state of the wall. If it is waiting, then return
        if (GameObject.Find("WallCamera").GetComponent<WallSceneManager>().GetState().ToLower() == "waiting")
            return;

        if (currentAsset != null && Vector2.Distance(vec, currentAsset.transform.position) < 1)
            AssetInteraction();
    }
    public void UpdateAsset(GameObject asset)
    {
        currentAsset = asset;
    }

    private void AssetInteraction()
    {
        if (!interacting)
        {
            interacting = true;
            StartCoroutine(DoInteraction());
        }
    }

    private IEnumerator DoInteraction()
    {
        // interaction goes here
        Debug.Log(currentAsset.name + " interaction!");
        // check to see if it has any audio with it
        if (currentAsset.GetComponentInChildren<AudioHolder>())
        {
            // play a random sound from those linked
            // right now it is placed on the wall camera so that nothing weird happens when the audio 
            // starts, but doesn't finish by the time it is deleted.
            AudioSource a = GameObject.Find("WallCamera").AddComponent<AudioSource>();
            a.clip = currentAsset.GetComponent<AudioHolder>().GetRandomAudio();
            a.Play();
            while (a != null && a.isPlaying)
            {
                yield return 0;
            }
            Destroy(a);
        }
        yield return new WaitForSeconds(timeBetweenInteraction);
        interacting = false;
    }
}
