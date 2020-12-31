using UnityEngine;

// Properties: LeftBound, RightBound, TopBound, BottomBound, BoundWidth, BoundHeight
// Methods: Contains
public class Bounds : MonoBehaviour
{
    [SerializeField] private float leftBound = 0f;
    [SerializeField] private float rightBound = 0f;
    [SerializeField] private float bottomBound = 0f;
    [SerializeField] private float topBound = 0f;

    public float LeftBound { get { return leftBound; } }
    public float RightBound { get { return rightBound; } }
    public float BottomBound { get { return bottomBound; } }
    public float TopBound { get { return topBound; } }

    public float BoundWidth { get { return rightBound - leftBound; } }
    public float BoundHeight { get { return topBound - bottomBound; } }

    public bool Contains(Vector2 point)
    {
        return point.x <= RightBound && point.x >= LeftBound &&
            point.y <= TopBound && point.y >= BottomBound;
    }

    private void Start()
    {
        if (!AreBoundsSet())
            Debug.LogWarning("Bounds set to 0.");
    }

    private bool AreBoundsSet()
    {
        return leftBound != 0f || rightBound != 0f ||
            bottomBound != 0f || topBound != 0f;
    }
}
