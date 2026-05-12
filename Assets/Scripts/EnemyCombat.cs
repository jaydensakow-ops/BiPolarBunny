using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public int damage = GameParameters.EnemyDamage;
    public Transform attackPoint;
    public float weaponRange;
    public LayerMask playerLayer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }
    }

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hits.Length > 0 && hits[0].GetComponent<PlayerHealth>() != null)
        {
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }
    }
}