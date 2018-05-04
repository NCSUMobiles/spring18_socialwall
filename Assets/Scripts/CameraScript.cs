using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float aspectX;
    public float aspectY;

	// Use this for initialization
	void Start () {
        float targetAspect = aspectX / aspectY;
        float windowAspect = (float)Screen.width / (float)Screen.height;

        float scaleHeight = windowAspect / targetAspect;
        Camera camera = gameObject.GetComponent<Camera>();

        if (scaleHeight > 1f)
        {
            Rect rect = camera.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
            camera.rect = rect;
        } else
        {
            float scaleWidth = 1f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
            camera.rect = rect;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
