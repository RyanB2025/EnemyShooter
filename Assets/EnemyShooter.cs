using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
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

    private void Start()
    {
        //Find Player using tag
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

        //Fire rate
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        //Calculate player location
        Vector2 directionToPlayer = (playerTarget.position - firePoint.position).normalized;

        //Spawn bullet
        GameObject firedBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        //tag enemybullet
        firedBullet.tag = "EnemyBullet";

        //Add Velocity
        Rigidbody2D rb = firedBullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = directionToPlayer * bulletSpeed;
        }
    }
}
