using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour {

    public Transform connectedBlackhole;

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (Input.GetKeyUp(KeyCode.E))
            {
                if (player.GetComponent<CharacterController2D>().moveBetweenBlackholes)
                {
                    player.GetComponent<CharacterController2D>().Transition(connectedBlackhole.localPosition);
                }
            }
        }
    }

}
