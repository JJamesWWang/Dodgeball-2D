using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Bounds _leftTeamBounds;
    [SerializeField] private Bounds _rightTeamBounds;
    [SerializeField] private Bounds _arenaBounds;
    [SerializeField] private Transform _leftTeamSpawnPoint;
    [SerializeField] private Transform _rightTeamSpawnPoint;

    public static Map Instance { get; private set; }
    public Bounds LeftTeamBounds { get { return _leftTeamBounds; } }
    public Bounds RightTeamBounds { get { return _rightTeamBounds; } }
    public Bounds ArenaBounds { get { return _arenaBounds; } }
    public Transform LeftTeamSpawnPoint { get { return _leftTeamSpawnPoint; } }
    public Transform RightTeamSpawnPoint { get { return _rightTeamSpawnPoint; } }

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } else
        {
            Instance = this;
        }
    }

}
