using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] spawnPoint;

    private void Awake()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            newPlayer.GetComponent<PhotonView>().ViewID = i;
            newPlayer.transform.position = spawnPoint[i].transform.position;
        }
    }
    
}
