﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class Spikes : MonoBehaviour {

    Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog> ();

    	// Use this for initialization
	void Start () 
	{
		Application.runInBackground = true; //allows unity to update when not in focus

		//************* Instantiate the OSC Handler...
	    OSCHandler.Instance.Init ();
        //OSCHandler.Instance.SendMessageToClient("pd", "/unity/death", 1);
      
        //*************
		//OSCHandler.Instance.SendMessageToClient("pd", "/unity/volume", volume);
	}

    private void OnTriggerEnter2D(Collider2D collider) {
        Player player = collider.GetComponent<Player>();
        if (player != null) {
            // Player hit Spikes
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/death", 1);
            Player.Die_Static();
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/music", 0);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/stop_music", 9);
        }
    }
}
