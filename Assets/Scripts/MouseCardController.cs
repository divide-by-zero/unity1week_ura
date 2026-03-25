using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCardController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private IMouseHoverable _currentHovered;
    private Vector2? _overridePosition;
    private bool _cardJustHovered;

    /// <summary>外部からマウス位置を上書きする。nullを渡すと実際のマウス位置に戻る。</summary>
    public void SetOverridePosition(Vector2? position)
    {
        _overridePosition = position;
    }

    private void Update()
    {
        Vector2 screenPos;
        if (_overridePosition.HasValue)
        {
            screenPos = _overridePosition.Value;
        }
        else
        {
            if (Pointer.current == null) return;
            screenPos = Pointer.current.position.ReadValue();
        }

        var ray = _camera.ScreenPointToRay(screenPos);

        IMouseHoverable newHovered = null;
        if (Physics.Raycast(ray, out var hit))
        {
            newHovered = hit.collider.GetComponentInParent<IMouseHoverable>();
        }

        if (newHovered != _currentHovered)
        {
            _currentHovered?.OnHoverExit();
            _currentHovered = newHovered;
            _currentHovered?.OnHoverEnter();
            _cardJustHovered = true;
        }
        else
        {
            _cardJustHovered = false;
        }

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            var isTouchPress = Touchscreen.current != null
                               && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

            if (isTouchPress && _cardJustHovered)
            {
                // タッチ1タップ目: hoverのみ、clickは抑制
            }
            else if (newHovered is IMouseClickable clickable)
            {
                clickable.OnClick();
            }
        }
    }
}
