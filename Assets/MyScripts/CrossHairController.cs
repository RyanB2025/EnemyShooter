using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your Main Camera here.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Drag the GameObject holding your OrbitingGravityField script here.")]
    [SerializeField] private GravityField gravityField;

    [Header("Settings")]
    [Tooltip("Hide the default Windows/Mac mouse cursor when a bullet is loaded?")]
    [SerializeField] private bool hideSystemCursor = true;

    [Tooltip("Select the layers that should block the crosshair (e.g., Walls).")]
    [SerializeField] private LayerMask obstacleLayer;

    [Tooltip("The radius of your crosshair. This dictates how thick the collision check is.")]
    [SerializeField] private float crosshairRadius = 0.25f; // <-- NEW: Size of the CircleCast

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

        // Determine raw mouse position
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;

        Vector3 finalTargetPosition = mouseWorldPosition;

        //Handles UI
        if (gravityField != null)
        {
            Transform nextBullet = gravityField.GetNextBulletTransform(mouseWorldPosition);

            if (nextBullet != null)
            {
                // --- NEW CIRCLECAST LOGIC ---
                Vector2 origin = nextBullet.position;
                Vector2 direction = (mouseWorldPosition - nextBullet.position).normalized;
                float distance = Vector2.Distance(origin, mouseWorldPosition);

                // We shoot a circle instead of a thin line. 
                RaycastHit2D hit = Physics2D.CircleCast(origin, crosshairRadius, direction, distance, obstacleLayer);

                if (hit.collider != null)
                {
                    // hit.distance automatically calculates where the CENTER of the circle 
                    // is at the exact moment its EDGE touches the wall.
                    finalTargetPosition = origin + (direction * hit.distance);
                    finalTargetPosition.z = 0f;
                }
                // -------------------------

                //We have bullet
                targetingLaser.enabled = true;
                targetingLaser.SetPosition(0, nextBullet.position);
                targetingLaser.SetPosition(1, finalTargetPosition);

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

        // Move the physical crosshair game object
        transform.position = finalTargetPosition;

        //Right Click
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (gravityField != null)
            {
                gravityField.FireBullet(finalTargetPosition);
            }
        }
    }
}