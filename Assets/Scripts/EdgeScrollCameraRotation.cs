using UnityEngine;
using UnityEngine.InputSystem;

public class EdgeScrollCameraRotation : MonoBehaviour
{
    [Range(0f, 0.5f)] [SerializeField] private float _edgeThreshold = 0.1f;

    [SerializeField] private float _horizontalSpeed = 10f;

    [SerializeField] private float _verticalSpeed = 40f;

    [SerializeField] private float _dragSensitivity = 0.2f;

    [SerializeField] private float _keyboardSpeed = 30f;

    [SerializeField] private float _minX = -10f;
    [SerializeField] private float _maxX = 10f;

    [SerializeField] private float _minPitch = -45f;
    [SerializeField] private float _maxPitch = 45f;

    private readonly bool _enableEdgeScroll = false;
    private readonly bool _enableKeyboard = false;

    private float _currentX;
    private float _currentPitch;
    private Vector2 _prevMousePos;
    private bool _isDragging;

    private void Start()
    {
        transform.position = new Vector3(0f, 11f, -9.5f);

        _currentX = 0f;
        _currentPitch = transform.eulerAngles.x;
        // 0〜360 を -180〜180 に変換
        if (_currentPitch > 180f) _currentPitch -= 360f;
    }

    private void Update()
    {
        if (Pointer.current == null) return;

        var mousePos = Pointer.current.position.ReadValue();
        float screenW = Screen.width;
        float screenH = Screen.height;

        float horizontal = 0f;
        float vertical = 0f;

        // 右ドラッグ or 2本指タッチ
        bool isDragInput = (Mouse.current != null && Mouse.current.leftButton.isPressed)
                           || (Touchscreen.current != null
                               && Touchscreen.current.touches[0].press.isPressed
                               && Touchscreen.current.touches[1].press.isPressed);
        if (isDragInput)
        {
            if (_isDragging)
            {
                var delta = mousePos - _prevMousePos;
                horizontal = delta.x * _dragSensitivity;
                vertical = -delta.y * _dragSensitivity;
            }

            _isDragging = true;
            _prevMousePos = mousePos;
        }
        else
        {
            _isDragging = false;

            // エッジスクロール
            if (_enableEdgeScroll)
            {
                // マウスが画面外なら何もしない
                if (mousePos.x < 0 || mousePos.x > screenW || mousePos.y < 0 || mousePos.y > screenH)
                    return;

                // 正規化座標 (0〜1)
                float normalizedX = mousePos.x / screenW;
                float normalizedY = mousePos.y / screenH;

                // 各方向の回転入力を算出（端に近いほど1に近づく）
                if (normalizedX < _edgeThreshold)
                    horizontal = -1f + normalizedX / _edgeThreshold; // -1〜0
                else if (normalizedX > 1f - _edgeThreshold)
                    horizontal = (normalizedX - (1f - _edgeThreshold)) / _edgeThreshold; // 0〜1

                if (normalizedY < _edgeThreshold)
                    vertical = 1f - normalizedY / _edgeThreshold; // 0〜1 (下端→上向き回転=pitch正)
                else if (normalizedY > 1f - _edgeThreshold)
                    vertical = -((normalizedY - (1f - _edgeThreshold)) / _edgeThreshold); // -1〜0 (上端→下向き回転)

                // クリックされていないなら何もしない
                if (Pointer.current.press.isPressed)
                {
                    horizontal *= _horizontalSpeed * Time.deltaTime;
                    vertical *= _verticalSpeed * Time.deltaTime;
                }
            }
        }

        // キーボード入力
        if (_enableKeyboard)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.upArrowKey.isPressed)
                    vertical -= _keyboardSpeed * Time.deltaTime;
                if (keyboard.downArrowKey.isPressed)
                    vertical += _keyboardSpeed * Time.deltaTime;
                if (keyboard.leftArrowKey.isPressed)
                    horizontal -= _keyboardSpeed * Time.deltaTime;
                if (keyboard.rightArrowKey.isPressed)
                    horizontal += _keyboardSpeed * Time.deltaTime;
            }
        }

        _currentX += horizontal;
        _currentPitch += vertical;

        _currentX = Mathf.Clamp(_currentX, _minX, _maxX);
        _currentPitch = Mathf.Clamp(_currentPitch, _minPitch, _maxPitch);

        var angles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(_currentPitch, angles.y, angles.z);
    }
}