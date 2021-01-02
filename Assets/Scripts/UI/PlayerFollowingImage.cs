using UnityEngine;

// Methods: SetPlayer
public class PlayerFollowingImage : MonoBehaviour
{
    private Player player;
    private RectTransform rect;
    [SerializeField] private Vector2 offset;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
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
        Vector2 playerPosition = player.transform.position;
        rect.position = playerPosition + offset;
    }
}
