using UnityEngine;

// 塔的子弹类，用于处理子弹的移动和碰撞逻辑
public class Projectile : MonoBehaviour
{
    private TowerData _data;

    private float _damage;
    private Vector3 _shootDirection;
    private float _projectileDuration;

    private void Start()
    {
        transform.localScale = Vector3.one * _data.projectileSize;
    }

    private void Update()
    {
        if (_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _projectileDuration -= Time.deltaTime;
            transform.position += new Vector3(_shootDirection.x, _shootDirection.y) * _data.projectileSpeed * Time.deltaTime;
        }
    }

    public void Shoot(TowerData data, float damage, Vector3 shootDirection)
    {
        _data = data;
        _damage = damage;
        _shootDirection = shootDirection;
        _projectileDuration = _data.projectileDuration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(_damage);
            gameObject.SetActive(false);
        }
    }
}
