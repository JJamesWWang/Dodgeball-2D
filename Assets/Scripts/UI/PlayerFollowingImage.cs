using UnityEngine;
using UnityEngine.UI;

// Methods: SetPlayer
public class PlayerFollowingImage : MonoBehaviour
{
    private Player player;
    private RectTransform rect;
    private Image image;
    [SerializeField] private Vector2 offset;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetPlayer(Player playerToFollow)
    {
        player = playerToFollow;
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }
        if (!player.IsOnField)
        {
            image.enabled = false;
            return;
        }
        image.enabled = true;
        Vector2 playerPosition = player.transform.position;
        rect.position = playerPosition + offset;
    }
}
