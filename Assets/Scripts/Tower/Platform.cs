using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// 建塔平台脚本---负责检测玩家点击的平台，并处理防御塔放置
public class Platform : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public static event Action<Platform> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;

    public static bool towerSelectPanelOpen { get; set; } = false;

    private Sprite _originalSprite;
    [SerializeField] private Sprite towerPlaceholderSprite;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalSprite = _spriteRenderer.sprite;
    }

    private void Update()
    {
        if (towerSelectPanelOpen || Time.timeScale == 0f || EventSystem.current.IsPointerOverGameObject())
            return;

        if (transform.childCount == 0)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, platformLayerMask);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                _spriteRenderer.sprite = towerPlaceholderSprite;
            } else
            {
                _spriteRenderer.sprite = _originalSprite;
            }
        }

        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchPressed = false;

        Vector2 screenPos = Vector2.zero;

        if (mousePressed)
        {
            screenPos = Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.wasPressedThisFrame)
                {
                    touchPressed = true;
                    screenPos = touch.position.ReadValue();
                    break;
                }
            }
        }

        if (mousePressed || touchPressed)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if (platform != null && platform.transform.childCount == 0)
                {
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
    }

    public void PlaceTower(TowerData data)
    {
        GameObject towerObject = Instantiate(data.prefab, transform.position, Quaternion.identity,transform);

        Tower tower = towerObject.GetComponent<Tower>();

        if (tower != null)
        {
            tower.SetPlatform(this);
        }

        _spriteRenderer.sprite = null;
    }

    public void ResetPlatform()
    {
        _spriteRenderer.sprite = _originalSprite;
    }
}
