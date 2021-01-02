using System.Data;
using UnityEngine;
using Mirror;

// Methods: CmdMoveTowards, StopMovement
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : NetworkBehaviour
{
    private Player player;
    private Vector2 destination;
    private bool hasReachedDestination = true;
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 160f;
    [SerializeField] private float angularRotationSpeed = 15f;

    private Bounds bounds;
    [Tooltip("If the Player is within this distance, it will stop moving.")]
    [SerializeField] private float stopDistance = 1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    [ServerCallback]
    private void Start()
    {
        SetMovementBounds();
    }

    private void SetMovementBounds()
    {
        var playerSprite = player.GetComponent<SpriteRenderer>();
        float xOffset = playerSprite.size.x / 2f;
        float yOffset = playerSprite.size.y / 2f;
        Bounds teamBounds = player.IsLeftTeam ? Map.Instance.LeftTeamBounds : Map.Instance.RightTeamBounds;
        bounds = Bounds.AddComponent(gameObject,
            teamBounds.LeftBound + xOffset, teamBounds.RightBound - xOffset,
            teamBounds.BottomBound + yOffset, teamBounds.TopBound + yOffset);
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
        if (bounds.Contains(point))
            SetDestination(point);
        else
            SetBoundedDestination(point);
    }

    [Server]
    private void SetDestination(Vector2 destinationPoint)
    {
        destination = destinationPoint;
        hasReachedDestination = false;
    }

    [Server]
    private void SetBoundedDestination(Vector2 destinationPoint)
    {
        Vector2 intersectionWithYBound = CalculateIntersectionWithYBound(destinationPoint);
        Vector2 intersectionWithXBound = CalculateIntersectionWithXBound(destinationPoint);
        if (bounds.Contains(intersectionWithYBound))
            SetDestination(intersectionWithYBound);
        else
            SetDestination(intersectionWithXBound);
    }

    [Server]
    private Vector2 CalculateIntersectionWithYBound(Vector2 destinationPoint)
    {
        Vector2 startPoint = transform.position;
        Vector2 vectorToPoint = destinationPoint - startPoint;
        float boundY = vectorToPoint.y > 0 ? bounds.TopBound : bounds.BottomBound;
        float slope = vectorToPoint.y / vectorToPoint.x;
        return startPoint + CalculateYBoundedVector(startPoint.y, boundY, slope);
    }

    [Server]
    private Vector2 CalculateYBoundedVector(float startY, float stopY, float slope)
    {
        float distance = stopY - startY;
        float x = distance / slope;
        float y = distance;
        return new Vector2(x, y);
    }

    [Server]
    private Vector2 CalculateIntersectionWithXBound(Vector2 destinationPoint)
    {
        Vector2 startPoint = transform.position;
        Vector2 vectorToPoint = destinationPoint - startPoint;
        float boundX = vectorToPoint.x > 0 ? bounds.RightBound : bounds.LeftBound;
        float slope = vectorToPoint.y / vectorToPoint.x;
        return startPoint + CalculateXBoundedVector(startPoint.x, boundX, slope);
    }

    [Server]
    private Vector2 CalculateXBoundedVector(float startX, float stopX, float slope)
    {
        float distance = stopX - startX;
        float y = slope * distance;
        float x = distance;
        return new Vector2(x, y);
    }

    [Server]
    public void StopMovement()
    {
        hasReachedDestination = true;
    }

    [ServerCallback]
    private void Update()
    {
        if (hasReachedDestination) { return; }
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
        position.x = Mathf.Clamp(position.x, bounds.LeftBound, bounds.RightBound);
        position.y = Mathf.Clamp(position.y, bounds.BottomBound, bounds.TopBound);
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
        Vector2 vectorToDestination = destination - position;
        if (distanceToDestination.sqrMagnitude < stopDistance * stopDistance)
            hasReachedDestination = true;
    }

    #endregion

}
