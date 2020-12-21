using UnityEngine;
using UnityEngine.Assertions;

public class BoundsAligner : MonoBehaviour
{
    [SerializeField] private Bounds boundary = null;
    private Transform transformToResize = null;

    [SerializeField] private bool setXPosition = false;
    [SerializeField] private float lerpX = 0f;
    [SerializeField] private bool setYPosition = false;
    [SerializeField] private float lerpY = 0f;
    [SerializeField] private Vector3 offset = Vector3.zero;

    [SerializeField] private bool scaleWidth = false;
    [SerializeField] private float scaleWidthFactor = 1f;
    [SerializeField] private bool useLocalXScale = true;
    [SerializeField] private bool scaleHeight = false;
    [SerializeField] private float scaleHeightFactor = 1f;
    [SerializeField] private bool useLocalYScale = true;

    public void Start()
    {
        transformToResize = gameObject.transform;
        AlignPosition();
        AlignScale();
    }

    private void AlignPosition()
    {
        Vector3 newPosition = transformToResize.position;
        if (setXPosition) { newPosition.x = Mathf.Lerp(boundary.LeftBound, boundary.RightBound, lerpX); }
        if (setYPosition) { newPosition.y = Mathf.Lerp(boundary.BottomBound, boundary.TopBound, lerpY); }
        newPosition += offset;
        transformToResize.position = newPosition;
    }

    private void AlignScale()
    {
        Vector3 newScale = Vector3.one;
        if (useLocalXScale)
            newScale.x = transformToResize.localScale.x;
        if (useLocalYScale)
            newScale.y = transformToResize.localScale.y;

        if (scaleWidth) { newScale.x *= boundary.BoundWidth * scaleWidthFactor; }
        if (scaleHeight) { newScale.y *= boundary.BoundHeight * scaleHeightFactor; }
        transformToResize.localScale = newScale;
    }
}
