using UnityEngine;

// ËþµÄ¹¥»÷·¶Î§¼ì²âÆ÷
public class TowerAttackRange : MonoBehaviour
{
    private Tower _tower;

    private void Awake()
    {
        _tower = GetComponentInParent<Tower>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _tower.AddEnemy(collision.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _tower.RemoveEnemy(collision.GetComponent<Enemy>());
        }
    }
}
