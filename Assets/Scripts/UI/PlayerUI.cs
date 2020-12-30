using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private UsernameText usernameTextPrefab;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        Player.ClientPlayerSpawned += HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(Player player)
    {
        UsernameText usernameText = Instantiate(usernameTextPrefab, Vector3.zero, Quaternion.identity);
        usernameText.SetPlayer(player);
        usernameText.transform.SetParent(canvas.transform);
    }

    private void UnsubscribeEvents()
    {
        Player.ClientPlayerSpawned -= HandlePlayerSpawned;
    }

}
