using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your Main Camera here.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Drag the GameObject holding your OrbitingGravityField script here.")]
    [SerializeField] private OrbitingGravityField gravityField;

    [Header("Settings")]
    [Tooltip("Hide the default Windows/Mac mouse cursor when a bullet is loaded?")]
    [SerializeField] private bool hideSystemCursor = true;

    private LineRenderer targetingLaser;
    private SpriteRenderer crosshairSprite;

    private void Awake()
    {
        //cache assets
        targetingLaser = GetComponent<LineRenderer>();
        crosshairSprite = GetComponent<SpriteRenderer>();

        //Optimize line
        targetingLaser.positionCount = 2;
        targetingLaser.useWorldSpace = true;
    }

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        //Follows mouse
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;
        transform.position = mouseWorldPosition;

        //Handles UI
        if (gravityField != null)
        {
            Transform nextBullet = gravityField.GetNextBulletTransform(mouseWorldPosition);

            if (nextBullet != null)
            {
                //We have bullet
                targetingLaser.enabled = true;
                targetingLaser.SetPosition(0, nextBullet.position);
                targetingLaser.SetPosition(1, mouseWorldPosition);

                crosshairSprite.enabled = true;
                if (hideSystemCursor) Cursor.visible = false;
            }
            else
            {
                //We don't have bullet
                targetingLaser.enabled = false;

                crosshairSprite.enabled = false;
                if (hideSystemCursor) Cursor.visible = true;
            }
        }

        //Right Click
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (gravityField != null)
            {
                gravityField.FireBullet(mouseWorldPosition);
            }
        }
    }
}