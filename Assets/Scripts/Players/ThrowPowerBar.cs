using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ThrowPowerBar : MonoBehaviour
{
    private bool fillingUp = true;
    private float fillSpeed;
    [SerializeField] private Vector2 offset = new Vector2(-5f, 5f);

    private Image indicator;
    private Camera mainCamera;
    [SerializeField] private PlayerDodgeballThrower playerArm;

    private void Start()
    {
        indicator = GetComponentInChildren<Image>();
        mainCamera = Camera.main;
        fillSpeed = 1f / playerArm.TimeTo100Percent;
    }

    private void Update()
    {
        FillIndicator();
        FollowMouse();
        CheckIfMouseIsStillHeld();
    }

    private void FillIndicator()
    {
        int sign = fillingUp ? 1 : -1;
        indicator.fillAmount = Mathf.Clamp(indicator.fillAmount + sign * fillSpeed * Time.deltaTime, 0f, 1f);
        if (indicator.fillAmount == 0f || indicator.fillAmount == 1f)
            fillingUp = !fillingUp;
    }

    private void FollowMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 position = mainCamera.ScreenToWorldPoint(mousePosition);
        transform.position = position + offset;
    }

    private void CheckIfMouseIsStillHeld()
    {
        if (!Mouse.current.leftButton.isPressed)
            Destroy(gameObject);
    }
}
