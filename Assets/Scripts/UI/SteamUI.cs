using UnityEngine;
using Mirror;
using Steamworks;

public class SteamUI : MonoBehaviour
{
    private SteamRoom room;

    private void Start()
    {
        room = (SteamRoom)NetworkManager.singleton;
    }

    public void HandleHostLobbyClicked()
    {
        room.StartHost();
    }

    public void HandleJoinFriendClicked()
    {
        SteamFriends.OpenOverlay("friends");
    }
}
