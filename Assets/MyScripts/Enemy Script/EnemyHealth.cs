using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    // Reference to the spawner
    private EnemySpawner spawner;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Enemy spawned with {currentHealth} health.");

        // FIXED WARNING: Replaced FindObjectOfType with FindFirstObjectByType
        spawner = FindFirstObjectByType<EnemySpawner>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check object hits enemy and tagged as "PlayerBullet"
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            TakeDamage(25);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        //Health debug
        Debug.Log($"Enemy took {damageAmount} damage! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy destroyed!");

        // Notify the spawner that this enemy has died BEFORE destroying it
        if (spawner != null)
        {
            spawner.EnemyDefeated(); // This causes the error if EnemySpawner isn't updated!
        }
        else
        {
            Debug.LogWarning("No EnemySpawner found in the scene! Make sure you have one.");
        }

        Destroy(gameObject);
    }
}