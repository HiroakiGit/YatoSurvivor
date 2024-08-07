using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suica : MonoBehaviour
{
    public float damage = 1; //�^����_���[�W

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // �e��j��
        }
        else if (other.CompareTag("Map") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject); // �e��j��
        }
    }
}
