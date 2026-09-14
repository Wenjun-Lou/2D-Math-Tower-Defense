using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// 塔类，负责处理塔的行为和属性
public class Tower : MonoBehaviour
{
    private CircleCollider2D _circleCollider;

    [SerializeField] private TowerData data;
    public TowerData Data => data;

    private List<Enemy> _enemiesInRange;
    private ObjectPooler _projectilePool;
    private float _shootTimer;

    public static event Action<Tower> OnTowerClicked;
    [SerializeField] private LayerMask towerLayerMask;
    [SerializeField] private Collider2D clickCollider;
    public static bool towerPanelOpen { get; set; } = false;

    private Platform _platform;

    [SerializeField] private int level = 1;
    public int Level => level;

    public float CurrentDamage => data.damage * Mathf.Pow(1f + data.damageGrowth, level - 1);
    public float CurrentRange => data.range * Mathf.Pow(1f + data.rangeGrowth, level - 1);
    public float CurrentShootInterval => data.shootInterval / Mathf.Pow(1f + data.attackSpeedGrowth, level - 1);
    public int UpgradeCost => Mathf.RoundToInt((data.cost / 4) * Mathf.Pow(1f + data.upgradeCostGrowth, level));

    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        _circleCollider = GetComponentInChildren<CircleCollider2D>();
        _circleCollider.radius = data.range;
        _enemiesInRange = new List<Enemy>();
        _projectilePool = GetComponent<ObjectPooler>();
        _shootTimer = data.shootInterval;
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0)
        {
            _shootTimer = data.shootInterval;
            Shoot();
        }

        if (Tower.towerPanelOpen || Time.timeScale == 0f || EventSystem.current.IsPointerOverGameObject()) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, towerLayerMask);

            if (hit.collider == clickCollider)
            {
                OnTowerClicked?.Invoke(this);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1, 0), CurrentRange);
    }

    public void AddEnemy(Enemy enemy)
    {
        bool _wasEmpty = _enemiesInRange.Count == 0;

        _enemiesInRange.Add(enemy);

        if (_wasEmpty && data.attackSound != null)
        {
            AudioManager.Instance.PlaySound(data.attackSound);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (_enemiesInRange.Contains(enemy))
        {
            _enemiesInRange.Remove(enemy);
        }
    }

    private void Shoot()
    {
        _enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);

        if (_enemiesInRange.Count > 0)
        {
            GameObject projectile = _projectilePool.GetPooledObject();
            projectile.transform.position = transform.position + Vector3.up;
            projectile.SetActive(true);
            Vector2 _shootDirection = (_enemiesInRange[0].transform.position - transform.position).normalized;
            projectile.GetComponent<Projectile>().Shoot(data, CurrentDamage, _shootDirection);
        }
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesInRange.Remove(enemy);
    }

    public void SetPlatform(Platform platform)
    {
        _platform = platform;
    }

    public void Upgrade()
    {
        level++;
        _circleCollider.radius = CurrentRange;
        _shootTimer = CurrentShootInterval;
    }

    public void DestroyTower()
    {
        _platform.ResetPlatform();
        Destroy(gameObject);
    }
}
