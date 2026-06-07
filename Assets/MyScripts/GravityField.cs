using UnityEngine;
using System;

public class GravityField : MonoBehaviour
{
    [Header("Shooting Settings")]
    [Tooltip("How fast the bullet shoots towards the crosshair.")]
    [SerializeField] private float shootForce = 20f;

    [Header("Orbit Settings")]
    [Tooltip("The player object that this field will orbit around.")]
    [SerializeField] private Transform playerTarget;

    [Tooltip("How many separate gravity fields should exist?")]
    [Range(1, 10)]
    [SerializeField] private int numberOfFields = 3;

    [Tooltip("How fast the fields orbit the player.")]
    [SerializeField] private float orbitSpeed = 90f;

    [Tooltip("Distance from the player to the fields.")]
    [SerializeField] private float orbitRadius = 4f;

    [Header("Gravity Settings")]
    [Tooltip("The size of each individual gravity field's pull radius.")]
    [SerializeField] private float fieldRadius = 3f;
    [Tooltip("How strongly the field pulls bullets towards its center.")]
    [SerializeField] private float pullStrength = 150f;
    [Tooltip("The tag of the objects this field should affect.")]
    [SerializeField] private string targetTag = "bullet";

    [Header("Capture Settings")]
    [Tooltip("How close the bullet needs to be to snap to the center and stop moving.")]
    [SerializeField] private float captureRadius = 0.5f;

    private float currentAngle = 0f;

    //Bullet Slots
    private Collider2D[] nodeOccupants = new Collider2D[0];

    private void FixedUpdate()
    {
        if (playerTarget == null) return;

        //Changes # of Fields
        if (nodeOccupants.Length != numberOfFields)
        {
            Array.Resize(ref nodeOccupants, numberOfFields);
        }

        currentAngle += orbitSpeed * Time.fixedDeltaTime;
        float angleStep = 360f / numberOfFields;

        for (int i = 0; i < numberOfFields; i++)
        {
            //Calculates specific gravity node
            float nodeAngle = currentAngle + (i * angleStep);
            float rad = nodeAngle * Mathf.Deg2Rad;
            Vector2 nodePosition = (Vector2)playerTarget.position + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

            //Checks Empty Nodes
            if (nodeOccupants[i] != null)
            {
                if (!nodeOccupants[i].gameObject.activeInHierarchy)
                {
                    nodeOccupants[i] = null;
                }
                else
                {
                    float distance = Vector2.Distance(nodePosition, nodeOccupants[i].transform.position);
                    if (distance > fieldRadius * 1.5f)
                    {
                        nodeOccupants[i] = null;
                    }
                }
            }

            //Look for bullet if empty
            if (nodeOccupants[i] == null)
            {
                Collider2D[] hitObjects = Physics2D.OverlapCircleAll(nodePosition, fieldRadius);
                float closestDistance = float.MaxValue;
                Collider2D bestBullet = null;

                foreach (Collider2D hit in hitObjects)
                {
                    if (hit.CompareTag(targetTag))
                    {
                        //Check other slots for claimed bullet
                        bool alreadyClaimed = false;
                        for (int j = 0; j < nodeOccupants.Length; j++)
                        {
                            if (nodeOccupants[j] == hit)
                            {
                                alreadyClaimed = true;
                                break;
                            }
                        }

                        if (!alreadyClaimed)
                        {
                            float dist = Vector2.Distance(nodePosition, hit.transform.position);
                            if (dist < closestDistance)
                            {
                                closestDistance = dist;
                                bestBullet = hit;
                            }
                        }
                    }
                }

                //Assign best bullet
                if (bestBullet != null)
                {
                    nodeOccupants[i] = bestBullet;

                    //Change tag to PlayerBullet when grabbed
                    bestBullet.tag = "PlayerBullet";
                }
            }

            //Bullet Gravity Physics
            if (nodeOccupants[i] != null)
            {
                Rigidbody2D bulletRb = nodeOccupants[i].GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    float distanceToCenter = Vector2.Distance(nodePosition, nodeOccupants[i].transform.position);

                    if (distanceToCenter <= captureRadius)
                    {
                        //Catch Bullet
                        bulletRb.linearVelocity = Vector2.zero;
                        bulletRb.angularVelocity = 0f;
                        nodeOccupants[i].transform.position = nodePosition;
                    }
                    else
                    {
                        //Pull Bullet
                        Vector2 pullDirection = (nodePosition - (Vector2)nodeOccupants[i].transform.position).normalized;
                        bulletRb.AddForce(pullDirection * pullStrength);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (playerTarget == null) return;

        float angleStep = 360f / numberOfFields;
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

        for (int i = 0; i < numberOfFields; i++)
        {
            float drawAngle = Application.isPlaying ? currentAngle + (i * angleStep) : (i * angleStep);
            float rad = drawAngle * Mathf.Deg2Rad;
            Vector2 nodePosition = (Vector2)playerTarget.position + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

            Gizmos.DrawWireSphere(nodePosition, fieldRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(nodePosition, captureRadius);
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        }
    }

    //Bullet Fire
    public Transform GetNextBulletTransform(Vector2 targetPosition)
    {
        Transform bestBullet = null;
        float closestDistance = float.MaxValue;

        //Closest bullet to crosshair
        for (int i = 0; i < nodeOccupants.Length; i++)
        {
            if (nodeOccupants[i] != null)
            {
                float dist = Vector2.Distance(targetPosition, nodeOccupants[i].transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestBullet = nodeOccupants[i].transform;
                }
            }
        }
        return bestBullet;
    }

    //Fires best bullet
    public void FireBullet(Vector2 targetPosition)
    {
        int bestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < nodeOccupants.Length; i++)
        {
            if (nodeOccupants[i] != null)
            {
                float dist = Vector2.Distance(targetPosition, nodeOccupants[i].transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestIndex = i;
                }
            }
        }

        //Found bullet to fire
        if (bestIndex != -1)
        {
            Collider2D firedBullet = nodeOccupants[bestIndex];
            Rigidbody2D rb = firedBullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                nodeOccupants[bestIndex] = null;

                Vector2 fireDirection = (targetPosition - (Vector2)firedBullet.transform.position).normalized;
                rb.AddForce(fireDirection * shootForce, ForceMode2D.Impulse);
            }
        }
    }
}