using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<CharacterController2D>().Victory();
        }
    }
}
