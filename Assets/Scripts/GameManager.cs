﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.A3Practical.TankVS
{

    public class GameManager : MonoBehaviourPunCallbacks
    {

        void Start()
        {

            if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Launcher");

				return;
			}

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
            }
            else
            {
                if (TankManager.LocalPlayerInstance == null)
                {
                    if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
                    {
                        PhotonNetwork.Instantiate(this.playerPrefab2.name, new Vector3(7f, 1f, 0f), Quaternion.identity, 0);
                    }
                    else
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(-13f, 1f, 0f), Quaternion.identity, 0);

                        //hardcoded positions
                        //instantiate height has to be 0f to avoid problems with the navmesh
                        PhotonNetwork.Instantiate(this.zombiePrefab.name, new Vector3(-4.486f, 0f, 6.163f), Quaternion.identity, 0);
                        PhotonNetwork.Instantiate(this.zombiePrefab.name, new Vector3(3.360f, 0f, -9.919f), Quaternion.identity, 0);
                    }

                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }


        void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}

        #region Photon Callbacks

        

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        #endregion

        #region Public Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public GameObject playerPrefab2;
        public GameObject zombiePrefab;

        #endregion



        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion


        #region Private Methods


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Game Scene");
        }


        #endregion
    }
}
