
using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ConnectionScreen : MonoBehaviour
{
   [SerializeField] private Button hostButton;
   [SerializeField] private Button clientButton;
   [SerializeField] private Button startButton;
   [SerializeField] private TextMeshProUGUI statusText;
   
   private bool isMaster;

   private void Awake()
   {
      hostButton.onClick.AddListener(ConnectAsHost);
      clientButton.onClick.AddListener(ConnectAsClient);
      startButton.onClick.AddListener(StartGame);
   }

   
   private void ConnectAsHost()
   {
      PhotonConnector.Instance.Connect(true,"vr_shooter");
      
      isMaster = true;
      clientButton.interactable = false;
      StartCoroutine(statusCheck());
   }

   private void ConnectAsClient()
   {
      PhotonConnector.Instance.Connect(false,"vr_shooter");
      PhotonConnector.Instance.OnJoinedLobby();
      isMaster = false;
      startButton.interactable = false;
      StartCoroutine(statusCheck());
   }

   private void StartGame()
   {
      //SceneManager.LoadScene(1);
      PhotonNetwork.LoadLevel(1);
   }
   IEnumerator statusCheck()
   {
      yield return new WaitForSeconds(5);
      while (PhotonNetwork.CurrentRoom.PlayerCount < 4)
      {
         yield return new WaitForSeconds(0.1f);
         if (isMaster)
         {
            statusText.text = "Connected player " + PhotonNetwork.CurrentRoom.PlayerCount + "/4";
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && !startButton.gameObject.activeSelf)
            {
               startButton.gameObject.SetActive(true);
            }
         }
         else
         {
            statusText.text = "Waiting for start";
         }

         
      }
   }
}

