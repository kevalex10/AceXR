using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private const string ROOM_NAME = "AceXR";
    private const string PLAYER_PREFAB_NAME = "PlayerXR";

    public static NetworkManager Instance;

    #region Unity Methods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion

    #region Methods
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
        Debug.Log($"Successfully connected to master.");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(PLAYER_PREFAB_NAME, Vector3.zero, Quaternion.identity);
        Debug.Log($"Successfully joined room: {ROOM_NAME}");
    }
    #endregion
}
