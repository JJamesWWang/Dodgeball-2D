using TMPro;
using UnityEngine;

public class UsernameText : MonoBehaviour
{
    private Player player;
    private RectTransform rect;
    private TMP_Text usernameText;
    [SerializeField] private Vector2 offset = new Vector2(0f, 30f);

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        usernameText = GetComponent<TMP_Text>();
        PlayerConnection.ClientPlayerInfoUpdated += HandlePlayerInfoUpdated;
    }

    private void OnDestroy()
    {
        PlayerConnection.ClientPlayerInfoUpdated -= HandlePlayerInfoUpdated;
    }

    public void SetPlayer(Player playerToFollow)
    {
        player = playerToFollow;
        GetComponent<TMP_Text>().text = player.Connection.Username;
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
        uint playerConnectionNetId = player.ConnectionNetId;
        if (playerConnectionNetId != netId) { return; }
        if (propertyName != nameof(player.Connection.Username)) { return; }

        usernameText.text = (string)value;
    }

}
