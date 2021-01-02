using UnityEngine;
using TMPro;

// Methods: SetPlayer
public class PlayerFollowingText : MonoBehaviour
{
    private Player player;
    private RectTransform rect;
    private TMP_Text text;
    [SerializeField] private Vector2 offset = new Vector2(0f, 30f);
    [SerializeField] private bool clientOnly;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
    }
    public void SetPlayer(Player playerToFollow)
    {
        player = playerToFollow;
    }

    public void SetText(string text)
    {
        this.text.text = text;
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
