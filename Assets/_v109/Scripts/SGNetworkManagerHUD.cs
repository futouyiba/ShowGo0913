using ShowGo;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mirror
{
    /// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Show Go Network Manager HUD")]
    // [RequireComponent(typeof(NetworkManager))]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
    public class SGNetworkManagerHUD : MonoBehaviour
    {
        [SerializeField]NetworkManager manager;
        [SerializeField] private int currentUser = 50000;

        public int offsetX;
        public int offsetY;

        void Awake()
        {
            // manager = GetComponent<NetworkManager>();
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            // client ready
            if (NetworkClient.isConnected && !NetworkClient.ready)
            {
                if (GUILayout.Button("Client Ready"))
                {
                    NetworkClient.Ready();
                    if (NetworkClient.localPlayer == null)
                    {
                        NetworkClient.AddPlayer();
                    }
                }
            }

            StopButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                // Server + Client
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("Host (Server + Client)"))
                    {
                        manager.StartHost();
                    }
                }

                // Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Client"))
                {
                    manager.StartClient();
                }
                // This updates networkAddress every frame from the TextField
                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                GUILayout.EndHorizontal();

                // Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");
                }
                else
                {
                    if (GUILayout.Button("Server Only")) manager.StartServer();
                }
            }
            else
            {
                // Connecting
                GUILayout.Label($"Connecting to {manager.networkAddress}..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
            }
        }

        void StatusLabels()
        {
            // host mode
            // display separately because this always confused people:
            //   Server: ...
            //   Client: ...
            if (NetworkServer.active && NetworkClient.active)
            {
                GUILayout.Label($"<b>Host</b>: running via {Transport.activeTransport}");
            }
            // server only
            else if (NetworkServer.active)
            {
                GUILayout.Label($"<b>Server</b>: running via {Transport.activeTransport}");
            }
            // client only
            else if (NetworkClient.isConnected)
            {
                GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.activeTransport}");
            }
        }

        async void StopButtons()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Host"))
                {
                    manager.StopHost();
                }
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Client"))
                {
                    manager.StopClient();
                }
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    manager.StopServer();
                }
            }

            if (NetworkServer.active)
            {
                var addInput = GUILayout.TextField("AddNum");
                if (GUILayout.Button("Add!"))
                {
                    var num = int.TryParse(addInput, out var n) ? n : 1;
                    for (int i = 0; i < num; i++)
                    {
                        currentUser++;
                        var randPos = PartyBehaviour.Instance.GetRandomWalkablePosition();
                        var fakeUserName = $"TestPlayer{(uint) currentUser}";
                        var go = Instantiate(manager.playerPrefab, randPos, Quaternion.identity);
                        go.name = fakeUserName;
                        var showGoPlayer = go.GetComponent<ShowGoPlayer>();
 
                        // var asset = await AssetCache.Instance.GetAsset<ShowGoPlayerCfg>("Cfg/ShowGoPlayerCfg");
                        Addressables.LoadAssetAsync<ShowGoPlayerCfg>("Cfg/ShowGoPlayerCfg").Completed +=
                            handle =>
                            {
                                var cfg = handle.Result;
                                NetworkServer.Spawn(go);
                                showGoPlayer.AprcId = Random.Range(0, cfg.CharacterPrefabs.Count) + 1;
                                showGoPlayer.SetUserId((uint)currentUser);
                                // Debug.Log($"add button causes aprcId set {showGoPlayer.UserId} : {showGoPlayer.AprcId}");
                                showGoPlayer.UserName = $"TestPlayer{showGoPlayer.UserId}";
                                showGoPlayer.Destination = randPos;
                                
                            };

                    }
                }
            }
        }
    }
}