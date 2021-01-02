using UnityEngine;

// Properties: LeftTeamBounds, RightTeamBounds, ArenaBounds, LeftTeamSpawnPoint, RightTeamSpawnPoint
// Methods: GetSpawnPoint
public class Map : MonoBehaviour
{
    [SerializeField] private Bounds leftTeamBounds;
    [SerializeField] private Bounds rightTeamBounds;
    [SerializeField] private Bounds arenaBounds;
    [SerializeField] private Transform[] leftTeamSpawnPoints;
    [SerializeField] private Transform[] rightTeamSpawnPoints;
    private int leftTeamSpawnIndex;
    private int rightTeamSpawnIndex;

    public Bounds LeftTeamBounds { get { return leftTeamBounds; } }
    public Bounds RightTeamBounds { get { return rightTeamBounds; } }
    public Bounds ArenaBounds { get { return arenaBounds; } }

    public Transform GetSpawnPoint(bool isLeftTeam)
    {
        if (isLeftTeam)
            return GetLeftTeamSpawnPoint();
        else
            return GetRightTeamSpawnPoint();
    }

    private Transform GetLeftTeamSpawnPoint()
    {
        Transform spawnPoint = leftTeamSpawnPoints[leftTeamSpawnIndex];
        IncrementSpawnIndex(true);
        return spawnPoint;
    }

    private Transform GetRightTeamSpawnPoint()
    {
        Transform spawnPoint = rightTeamSpawnPoints[rightTeamSpawnIndex];
        IncrementSpawnIndex(false);
        return spawnPoint;
    }

    private void IncrementSpawnIndex(bool isLeftTeam)
    {
        if (isLeftTeam)
            leftTeamSpawnIndex = (leftTeamSpawnIndex + 1) % leftTeamSpawnPoints.Length;
        else
            rightTeamSpawnIndex = (rightTeamSpawnIndex + 1) % rightTeamSpawnPoints.Length;
    }

}
