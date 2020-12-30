using UnityEngine;

// Properties: LeftBound, RightBound, TopBound, BottomBound, BoundWidth, BoundHeight
// Methods: Contains
public class Bounds : MonoBehaviour
{
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    [SerializeField] private float bottomBound;
    [SerializeField] private float topBound;

    public float LeftBound { get { return leftBound; } }
    public float RightBound { get { return rightBound; } }
    public float BottomBound { get { return bottomBound; } }
    public float TopBound { get { return topBound; } }

    public float BoundWidth { get { return rightBound - leftBound; } }
    public float BoundHeight { get { return topBound - bottomBound; } }

    public bool Contains(Vector2 point)
    {
        return point.x < RightBound && point.x > LeftBound && point.y < TopBound && point.y > BottomBound;
    }
}
