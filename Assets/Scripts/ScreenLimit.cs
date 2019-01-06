using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLimit : MonoBehaviour {

    private float playerWidth;
    private float playerHalfWidth;
    private float screenWidth; 

    void Start ()
    {
        screenWidth = 25.0f;
        playerWidth = gameObject.GetComponent<BoxCollider2D>().size.x;
        playerHalfWidth = playerWidth / 2f;
    }

    void Update ()
    {

        if (transform.position.x + playerHalfWidth > screenWidth/2)
            transform.position = new Vector2(screenWidth/2 - playerHalfWidth, transform.position.y); 

        if (transform.position.x - playerHalfWidth <  -screenWidth/2)
            transform.position = new Vector2(-screenWidth/2 + playerHalfWidth, transform.position.y); 
    }
}
