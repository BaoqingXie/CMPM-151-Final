

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class LevelGenerator : MonoBehaviour {

    private const float PLAYER_DISTANCE_SPAWN_LEVEL_PART = 200f;

    [SerializeField] private Transform levelPart_Start;
    [SerializeField] private List<Transform> levelPartList;
    [SerializeField] private Player player;
    Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog> ();
    public int tempo = 1;
    public int speed = 0;
    GameObject[] Coins;
    

    private Vector3 lastEndPosition;

    private void Awake() {
        lastEndPosition = levelPart_Start.Find("EndPosition").position;

        int startingSpawnLevelParts = 5;
        for (int i = 0; i < startingSpawnLevelParts; i++) {
            SpawnLevelPart();
        }
        //OSCHandler.Instance.Init ();
    }

    private void Update() {
        if (Vector3.Distance(player.GetPosition(), lastEndPosition) < PLAYER_DISTANCE_SPAWN_LEVEL_PART) {
            // Spawn another level part
            SpawnLevelPart();
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempo_main", 250/tempo);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempo_back", 2000/tempo);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempo_kick", 500/tempo);
            tempo++;
            speed += 10;
            Coins = GameObject.FindGameObjectsWithTag("Coins");
        }
            if(Coins!=null){
            for(int i =0; i<Coins.Length; i++){
                Coins[i].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 10, 0);
                if(Coins[i].transform.position.y > 48){
                    Coins[i].GetComponent<Rigidbody2D>().velocity = new Vector3(0, -10, 0);
                }
            }
            }
    }

    private void SpawnLevelPart() {
        Transform chosenLevelPart = levelPartList[Random.Range(0, levelPartList.Count)];
        Transform lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, lastEndPosition);
        lastEndPosition = lastLevelPartTransform.Find("EndPosition").position;
    }

    private Transform SpawnLevelPart(Transform levelPart, Vector3 spawnPosition) {
        Transform levelPartTransform = Instantiate(levelPart, spawnPosition, Quaternion.identity);
        return levelPartTransform;
    }

}
