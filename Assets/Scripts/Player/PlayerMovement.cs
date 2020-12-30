using UnityEngine;
using Mirror;

// Methods: CmdMoveTowards, StopMovement
public class PlayerMovement : NetworkBehaviour
{
    private Player player;
    private Vector2 destination;
    private bool reachedDestination = true;
    [SerializeField] private float movementSpeed = 160f;
    [SerializeField] private float angularRotationSpeed = 15f;
    [Tooltip("If the Player is within this distance, it will stop moving.")]
    [SerializeField] private float stopDistance = 1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    #region Server

    [Command]
    public void CmdMoveTowards(Vector2 point)
    {
        MoveTowards(point);
    }

    [Server]
    private void MoveTowards(Vector2 point)
    {
        Bounds movementBounds = player.IsLeftTeam ? Map.Instance.LeftTeamBounds : Map.Instance.RightTeamBounds;
        if (movementBounds.Contains(point))
            SetDestination(point);
    }

    [Server]
    private void SetDestination(Vector2 point)
    {
        destination = point;
        reachedDestination = false;
    }

    [Server]
    public void StopMovement()
    {
        reachedDestination = true;
    }

    [ServerCallback]
    private void Update()
    {
        if (reachedDestination) { return; }
        MoveTowardsDestination();
        RotateTowardsDestination();
        CheckReachedDestination();
    }

    [Server]
    private void MoveTowardsDestination()
    {
        Vector2 position = transform.position;
        Vector2 destinationDirection = (destination - position).normalized;
        position += destinationDirection * movementSpeed * Time.deltaTime;
        transform.position = position;
    }

    [Server]
    private void RotateTowardsDestination()
    {
        Vector2 position = transform.position;
        Vector2 vectorToDestination = destination - position;
        float theta = Mathf.Atan2(vectorToDestination.y, vectorToDestination.x) * Mathf.Rad2Deg;
        Quaternion rotatedZAxis = Quaternion.AngleAxis(theta, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotatedZAxis, angularRotationSpeed * Time.deltaTime);
    }

    [Server]
    private void CheckReachedDestination()
    {
        Vector2 position = transform.position;
        Vector2 distanceToDestination = destination - position;
        if (distanceToDestination.sqrMagnitude < stopDistance * stopDistance)
            reachedDestination = true;
    }

    #endregion

}
