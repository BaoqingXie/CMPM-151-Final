/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using V_AnimationSystem;
using CodeMonkey.Utils;
using UnityOSC;

/*
 * Simple Jump
 * */
public class Player : MonoBehaviour {

    private static Player instance;

    [SerializeField] private LayerMask platformsLayerMask;
    private Player_Base playerBase;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    private bool waitForStart;
    private bool isDead;
    Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog> ();
    List<OSCPacket> packets = new List<OSCPacket>();

    private void Awake() {
        instance = this;
        playerBase = gameObject.GetComponent<Player_Base>();
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
        waitForStart = true;
        isDead = false;
        Application.runInBackground = true; //allows unity to update when not in focus

		//************* Instantiate the OSC Handler...
	    OSCHandler.Instance.Init ();
        //OSCReciever.Open(8001);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/music", 1);
    }

    private void Update() {
        if (isDead) return;
        if (waitForStart) {
            playerBase.PlayIdleAnim();
            if (Input.GetKeyDown(KeyCode.Space)) {
                waitForStart = false;
            }
        } else {
            if (IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
                float jumpVelocity = 100f;
                rigidbody2d.velocity = Vector2.up * jumpVelocity;
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/jump", 1);
            }

            HandleMovement();

            // Set Animations
            if (IsGrounded()) {
                if (rigidbody2d.velocity.x == 0) {
                    playerBase.PlayIdleAnim();
                } else {
                    playerBase.PlayMoveAnim(new Vector2(rigidbody2d.velocity.x, 0f));
                }
            } else {
                playerBase.PlayJumpAnim(rigidbody2d.velocity);
            }
        }
        //OSCHandler.Instance.OnPacketReceived("unity", packets);
        OSCHandler.Instance.UpdateLogs();
        //Debug.Log(OSCHandler.Instance.packets);
        //OSCReciever.getNextMessage();
        Dictionary<string, ClientLog> clients = new Dictionary<string, ClientLog>();
        clients = OSCHandler.Instance.Clients;
        Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
        servers = OSCHandler.Instance.Servers;

        //Debug.Log(servers);

        foreach (KeyValuePair<string, ServerLog> author in servers)  
        {  
            Debug.Log(author.Value);
        }  
    }

    private bool IsGrounded() {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 1f, platformsLayerMask);
        return raycastHit2d.collider != null;
    }
    
    private void HandleMovement() {
        float moveSpeed = 40f;
        rigidbody2d.velocity = new Vector2(+moveSpeed, rigidbody2d.velocity.y);
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    private void Die() {
        isDead = true;
        rigidbody2d.velocity = Vector3.zero;
    }

    public static void Die_Static() {
        instance.Die();
        GameOverWindow.Show();
    }

}
