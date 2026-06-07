using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Enemy spawned with {currentHealth} health.");
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

        Destroy(gameObject);
    }
}