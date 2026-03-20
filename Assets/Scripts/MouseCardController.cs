using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCardController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private IMouseHoverable _currentHovered;
    private Vector2? _overridePosition;

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
            if (Mouse.current == null) return;
            screenPos = Mouse.current.position.ReadValue();
        }

        var ray = _camera.ScreenPointToRay(screenPos);

        IMouseHoverable newHovered = null;
        if (Physics.Raycast(ray, out var hit))
        {
            newHovered = hit.collider.GetComponentInParent<IMouseHoverable>();
        }

        if (newHovered == _currentHovered) return;

        _currentHovered?.OnHoverExit();
        _currentHovered = newHovered;
        _currentHovered?.OnHoverEnter();
    }
}
