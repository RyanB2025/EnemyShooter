using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Bullet hit wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        //PlayerBullet hits an Enemy
        if (gameObject.CompareTag("PlayerBullet") && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }

        //EnemyBullet hits the Player
        if (gameObject.CompareTag("EnemyBullet") && collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
