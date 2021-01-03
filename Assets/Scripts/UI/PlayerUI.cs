using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private PlayerFollowingText usernameTextPrefab;
    [SerializeField] private PlayerFollowingImage leftTeamPlayerIndicatorPrefab;
    [SerializeField] private PlayerFollowingImage rightTeamPlayerIndicatorPrefab;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        Player.ClientPlayerSpawned += HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(Player player)
    {
        CreateUsernameText(player);
        if (player.IsLocalPlayer)
            CreatePlayerIndicator(player);

    }

    private void CreateUsernameText(Player player)
    {
        PlayerFollowingText usernameText = Utils.InstantiateOffScreen(usernameTextPrefab);
        usernameText.SetPlayer(player);
        usernameText.SetText(player.Username);
        usernameText.transform.SetParent(canvas.transform);
    }

    private void CreatePlayerIndicator(Player player)
    {
        if (player.IsSpectator) { return; }
        PlayerFollowingImage playerIndicator = InstantiatePlayerIndicator(player);
        playerIndicator.SetPlayer(player);
        playerIndicator.transform.SetParent(canvas.transform);
    }

    private PlayerFollowingImage InstantiatePlayerIndicator(Player player)
    {
        if (player.IsLeftTeam)
            return Utils.InstantiateOffScreen(leftTeamPlayerIndicatorPrefab);
        else
            return Utils.InstantiateOffScreen(rightTeamPlayerIndicatorPrefab);
    }

    private void UnsubscribeEvents()
    {
        Player.ClientPlayerSpawned -= HandlePlayerSpawned;
    }

}
