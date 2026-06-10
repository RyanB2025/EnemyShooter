using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;
    public GameManager gameManager; // Assign this in the Unity Inspector

    private void Start()
    {
        //Start health
        currentHealth = maxHealth;
        Debug.Log($"Player spawned with {currentHealth} health.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check if object is Enemy
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        //Debug log
        Debug.Log($"Player took {damageAmount} damage! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Trigger the lose panel
        if (gameManager != null)
        {
            gameManager.TriggerLoseState();
        }

        // Disable player object
        gameObject.SetActive(false);
    }
}
