using UnityEngine;

// 控制敌人动画的脚本，当敌人改变目标点时触发动画更新
public class AnimateOnWaypointChange : MonoBehaviour
{
    private Animator _animator;
    private Enemy _enemy;
    private Vector3 _lastTarget;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
        if (_animator == null) return;
        _lastTarget = _enemy.CurrentTargetPosition;
    }

    private void Update()
    {
        Vector3 currentTarget = _enemy.CurrentTargetPosition;
        if (currentTarget != _lastTarget)
        {
            UpdateAnimation(currentTarget);
            _lastTarget = currentTarget;
        }
    }

    private void UpdateAnimation(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            _animator.SetFloat("moveX", dir.x > 0 ? 1 : -1);
            _animator.SetFloat("moveY", 0);
        }
        else
        {
            _animator.SetFloat("moveX", 0);
            _animator.SetFloat("moveY", dir.y > 0 ? 1 : -1);
        }
    }
}
