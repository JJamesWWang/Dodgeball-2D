using UnityEngine;
using TMPro;

// Methods: SetPlayer
public class UsernameText : MonoBehaviour
{
    private Player player;
    private RectTransform rect;
    private TMP_Text text;
    [SerializeField] private Vector2 offset = new Vector2(0f, 30f);

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
    }
    public void SetPlayer(Player playerToFollow)
    {
        player = playerToFollow;
        text.text = player.Username;
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
}
