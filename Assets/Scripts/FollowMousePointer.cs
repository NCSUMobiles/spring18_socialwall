using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FollowMousePointer : MonoBehaviour
{ 
    public Camera camera;
    Transform currentAsset;

    // Update is called once per frame
    void Update()
    {
        Vector2 loc = Input.mousePosition;

        Vector3 vec = camera.ScreenToWorldPoint(new Vector3(loc.x, loc.y, 10f));

        //vec.y += 10;
        transform.position = vec;

        if (currentAsset == null)
        {

        }
    }

    public void UpdateAsset(Transform currentAsset)
    {
        this.currentAsset = currentAsset;
    }
}
