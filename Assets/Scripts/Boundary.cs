using UnityEngine;

public class Boundary : MonoBehaviour
{
    
    [SerializeField] private float _LeftBound;
    [SerializeField] private float _RightBound;
    [SerializeField] private float _TopBound;
    [SerializeField] private float _BottomBound;

    public float LeftBound { get { return _LeftBound; } }
    public float RightBound { get { return _RightBound; } }
    public float TopBound { get { return _TopBound; } }
    public float BottomBound { get { return _BottomBound; } }

    public float BoundWidth { get { return _RightBound - _LeftBound; } }
    public float BoundHeight { get { return _TopBound - _BottomBound; } }
}
