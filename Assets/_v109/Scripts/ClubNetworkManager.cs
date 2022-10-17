using DG.Tweening;
using Mirror;
using UnityEngine;

namespace ShowGo
{
    public class ClubNetworkManager : NetworkManager
    {
        public bool bIsServer = false;
        public string autoServerAddress = "192.168.31.129";

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {

            if (conn.identity == null)
            {
                Debug.LogWarning("on server disconnect but conn has no identity");
                return;
            }

            var showGoPlayer = conn.identity.GetComponent<ShowGoPlayer>();
            if (showGoPlayer == null)
            {
                Debug.LogWarning("on server disconnect but identity has no show go player");

                return;
            }
            
            DOTween.KillAll(showGoPlayer.transform);
            var userIdToRemove = showGoPlayer.UserId;
            SceneShowgoMgr.Instance.RemoveUser(showGoPlayer.UserId);
            foreach (var networkConnectionToClient in NetworkServer.connections.Values)
            {
             var observerIdentity = networkConnectionToClient.identity;
             if (observerIdentity == null)
                 continue;
             var observerShowGoPlayer = observerIdentity.GetComponent<ShowGoPlayer>();
             if (observerShowGoPlayer == null)
             {
                 continue;
             }
             observerShowGoPlayer.CleanUpUIForOtherPlayer(userIdToRemove);
            }
            base.OnServerDisconnect(conn);

        }

        // public override async void OnClientDisconnect()
        // {
        //     if (SceneShowgoMgr.Instance != null && SceneShowgoMgr.Instance.Me != null)
        //     {
        //         DOTween.KillAll(SceneShowgoMgr.Instance.Me.transform);
        //     }
        //
        //     // SceneShowgoMgr.Instance.ClearEverything();
        //     // StartClient();
        //     // do
        //     // {
        //         // await TimerComponent.Instance.WaitAsync(3000);
        //         // StartClient();
        //     // }  while(!NetworkClient.active);
        //     base.OnClientDisconnect();
        // }

        public override void Start()
        {
            if (!bIsServer)
            {
                networkAddress = autoServerAddress;
                StartClient();
            }
            base.Start();

            // else
            // {
                // if (!NetworkServer.active)
                // {
                //     StartServer();
                // }
            // }
        }

        //20221005�������
        public override void OnStartClient()
        {
            WaitingPageControl.instance?.OnStartClient();
            base.OnStartClient();
        }

        // public override void OnClientSceneChanged()
        // {
        //     if (networkSceneName == offlineScene)
        //     {
        //         SceneShowgoMgr.Instance.ClearEverything();
        //         networkAddress = autoServerAddress;
        //         StartClient();
        //     }
        //
        //     if (networkSceneName == onlineScene)
        //     {
        //         ContinentalMessenger.Instance.GetMyInfoByUserId();
        //     }
        //     base.OnClientSceneChanged();
        // }

        // private void OnApplicationPause(bool pauseStatus)
        // {
        //     Debug.Log("club network manager on application pause");
        //     
        //     if (bIsServer)
        //     {
        //         return;
        //     }
        //     
        //     if (pauseStatus)
        //     {
        //         Debug.Log("on application pause, about to stop client.");
        //         // SceneShowgoMgr.Instance.ClearEverything();
        //         StopClient();
        //     }
        //     else
        //     {
        //         // if (!NetworkClient.active)
        //         // {
        //             // networkAddress = autoServerAddress;
        //             // Debug.Log("on application unpause, about to start client");
        //             // StartClient();
        //     // }
        //
        //     // if (!NetworkClient.ready)
        //     // {
        //     //     NetworkClient.Ready();
        //     // }
        //     }
        // }


        // public float updateReconnectInterval = 10f;
        // public float checkAccumulatedTime = 0f;
        // private void Update()
        // {
        //     if (bIsServer)
        //     {
        //         return;
        //     }
        //     checkAccumulatedTime += Time.deltaTime;
        //     if (checkAccumulatedTime>=updateReconnectInterval)
        //     {
        //         checkAccumulatedTime = 0f;
        //         if (!NetworkClient.active)
        //         {
        //             networkAddress = autoServerAddress;
        //             StartClient();
        //         }
        //
        //         // if (!NetworkClient.isConnected)
        //         // {
        //             // NetworkClient.Connect(autoServerAddress);
        //         // }
        //     }
        // }
    }
}