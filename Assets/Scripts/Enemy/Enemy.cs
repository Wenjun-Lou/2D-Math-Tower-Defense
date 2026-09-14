using System;
using UnityEngine;

// 敌人类，负责移动和处理生命值
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;

    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;

    private Path _currentPath;
    private Vector3 _targetPosition;
    public Vector3 CurrentTargetPosition => _targetPosition;
    private int _currentWaypoint;

    private float _lives;
    private float _maxLives;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;
    private bool _hasBeenCounted = false;
    private void Awake()
    {
        _healthBarOriginalScale = healthBar.localScale;
    }
    private void Update()
    {
        if (_hasBeenCounted) return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);

        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else
            {
                _hasBeenCounted = true;
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }
    public void Initialize(Path path, float healthMultiplier)
    {
        _currentPath = path;
        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
        _hasBeenCounted = false;
        _maxLives = data.lives * healthMultiplier;
        _lives = _maxLives;
        UpdateHealthBar();
    }
    public void TakeDamage(float damage)
    {
        if (_hasBeenCounted) return;

        _lives -= damage;
        _lives = Math.Max(_lives, 0);
        UpdateHealthBar();

        if (_lives <= 0)
        {
            AudioManager.Instance.PlayEnemyDestroyed();
            _hasBeenCounted = true;
            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxLives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }
}
