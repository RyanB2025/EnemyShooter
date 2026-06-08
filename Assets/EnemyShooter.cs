using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the enemy moves.")]
    [SerializeField] private float speed = 3f;

    [Tooltip("Distance at which the enemy stops to shoot.")]
    [SerializeField] private float stoppingDistance = 5f;

    [Tooltip("Distance at which the enemy backs away from the player.")]
    [SerializeField] private float retreatDistance = 3f;

    [Header("Shooting Settings")]
    [Tooltip("The bullet prefab the enemy will shoot.")]
    [SerializeField] private GameObject bulletPrefab;

    [Tooltip("Where the bullet spawns (create an empty GameObject child on the enemy).")]
    [SerializeField] private Transform firePoint;

    [Tooltip("How fast the enemy fires (in seconds).")]
    [SerializeField] private float fireRate = 2f;

    [Tooltip("How fast the bullet travels.")]
    [SerializeField] private float bulletSpeed = 10f;

    private Transform playerTarget;
    private float fireTimer;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find Player using tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
        else
        {
            Debug.LogWarning("Enemy couldn't find the Player! Make sure your Player is tagged as 'Player'.");
        }
    }

    private void Update()
    {
        if (playerTarget == null) return;

        HandleShooting();
    }

    private void FixedUpdate()
    {
        if (playerTarget == null) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
        Vector2 directionToPlayer = (playerTarget.position - transform.position).normalized;

        if (distanceToPlayer > stoppingDistance)
        {
            // Move towards the player
            rb.linearVelocity = directionToPlayer * speed;
        }
        else if (distanceToPlayer < stoppingDistance && distanceToPlayer > retreatDistance)
        {
            // Stop moving (maintain perfect shooting distance)
            rb.linearVelocity = Vector2.zero;
        }
        else if (distanceToPlayer < retreatDistance)
        {
            // Back away if the player gets too close
            rb.linearVelocity = -directionToPlayer * speed;
        }
    }

    private void HandleShooting()
    {
        // Fire rate timer
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            // Only shoot if we have a clear line of sight
            if (HasLineOfSight())
            {
                Shoot();
                fireTimer = 0f; // Reset the timer AFTER we successfully shoot
            }
            // Note: If blocked, the timer stays at max, meaning the enemy 
            // will fire INSTANTLY the moment another enemy moves out of the way.
        }
    }

    // --- NEW RAYCAST LOGIC ---
    private bool HasLineOfSight()
    {
        if (firePoint == null || playerTarget == null) return false;

        // Calculate direction and distance to the player
        Vector2 directionToPlayer = (playerTarget.position - firePoint.position).normalized;
        float distanceToPlayer = Vector2.Distance(firePoint.position, playerTarget.position);

        // Fire a raycast from the firePoint to the Player
        // We use RaycastAll so we can ignore the enemy's own body if the firePoint is inside it
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, directionToPlayer, distanceToPlayer);

        // Check everything the ray hit on its way to the player
        foreach (RaycastHit2D hit in hits)
        {
            // Ignore this enemy's own collider and any trigger colliders (like pure detection zones)
            if (hit.collider.gameObject == this.gameObject || hit.collider.isTrigger)
                continue;

            // If the first solid thing in the way is another Enemy, block the shot!
            if (hit.collider.CompareTag("Enemy"))
            {
                // Optional: Draw a red line in the editor to easily see when a shot is blocked
                Debug.DrawRay(firePoint.position, directionToPlayer * hit.distance, Color.red);
                return false;
            }

            // If we hit the player, the line of sight is clear!
            if (hit.collider.CompareTag("Player"))
            {
                // Optional: Draw a green line in the editor to easily see when a shot is clear
                Debug.DrawRay(firePoint.position, directionToPlayer * hit.distance, Color.green);
                return true;
            }
        }

        // If we hit nothing blocking us, it's safe to shoot
        return true;
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 directionToPlayer = (playerTarget.position - firePoint.position).normalized;

        GameObject firedBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        firedBullet.tag = "EnemyBullet";

        Rigidbody2D bulletRb = firedBullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = directionToPlayer * bulletSpeed;
        }
    }
}