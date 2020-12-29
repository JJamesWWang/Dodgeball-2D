using TMPro;
using UnityEngine;

public class UsernameText : MonoBehaviour
{
    private Player player;
    private RectTransform rect;
    private TMP_Text usernameText;
    [SerializeField] private Vector2 offset = new Vector2(0f, 30f);

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        usernameText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        PlayerData.ClientPlayerInfoUpdated += HandlePlayerInfoUpdated;
    }

    private void OnDestroy()
    {
        PlayerData.ClientPlayerInfoUpdated -= HandlePlayerInfoUpdated;
    }

    public void SetPlayer(Player playerToFollow)
    {
        player = playerToFollow;
        usernameText.text = player.Data.Username;
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 playerPosition = player.transform.position;
        rect.position = playerPosition + offset;
    }


    private void HandlePlayerInfoUpdated(uint netId, string propertyName, object value)
    {
        uint connectionNetId = player.ConnectionNetId;
        if (connectionNetId != netId) { return; }
        if (propertyName != nameof(player.Data.Username)) { return; }

        usernameText.text = (string)value;
    }

}
