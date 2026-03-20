using UnityEngine;
using UnityEngine.InputSystem;

public class EdgeScrollCameraRotation : MonoBehaviour
{
    [Header("端の判定範囲（画面比率 0〜0.5）")]
    [Range(0f, 0.5f)]
    [SerializeField] private float _edgeThreshold = 0.1f;

    [Header("水平移動速度（単位/秒）")]
    [SerializeField] private float _horizontalSpeed = 10f;

    [Header("垂直回転速度（度/秒）")]
    [SerializeField] private float _verticalSpeed = 40f;

    [Header("X軸 移動制限")]
    [SerializeField] private float _minX = -10f;
    [SerializeField] private float _maxX = 10f;

    [Header("Pitch 回転制限（絶対角度）")]
    [SerializeField] private float _minPitch = -45f;
    [SerializeField] private float _maxPitch = 45f;

    private float _currentX;
    private float _currentPitch;
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
        _currentX = 0f;
        _currentPitch = transform.eulerAngles.x;
        // 0〜360 を -180〜180 に変換
        if (_currentPitch > 180f) _currentPitch -= 360f;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        var mousePos = Mouse.current.position.ReadValue();
        float screenW = Screen.width;
        float screenH = Screen.height;

        // マウスが画面外なら何もしない
        if (mousePos.x < 0 || mousePos.x > screenW || mousePos.y < 0 || mousePos.y > screenH)
            return;

        // 正規化座標 (0〜1)
        float normalizedX = mousePos.x / screenW;
        float normalizedY = mousePos.y / screenH;

        // 各方向の回転入力を算出（端に近いほど1に近づく）
        float horizontal = 0f;
        float vertical = 0f;

        if (normalizedX < _edgeThreshold)
            horizontal = -1f + normalizedX / _edgeThreshold; // -1〜0
        else if (normalizedX > 1f - _edgeThreshold)
            horizontal = (normalizedX - (1f - _edgeThreshold)) / _edgeThreshold; // 0〜1

        if (normalizedY < _edgeThreshold)
            vertical = 1f - normalizedY / _edgeThreshold; // 0〜1 (下端→上向き回転=pitch正)
        else if (normalizedY > 1f - _edgeThreshold)
            vertical = -((normalizedY - (1f - _edgeThreshold)) / _edgeThreshold); // -1〜0 (上端→下向き回転)

        _currentX += horizontal * _horizontalSpeed * Time.deltaTime;
        _currentPitch += vertical * _verticalSpeed * Time.deltaTime;

        _currentX = Mathf.Clamp(_currentX, _minX, _maxX);
        _currentPitch = Mathf.Clamp(_currentPitch, _minPitch, _maxPitch);

        var angles = transform.eulerAngles;
        transform.position = _initialPosition + transform.right * _currentX;
        transform.eulerAngles = new Vector3(_currentPitch, angles.y, angles.z);
    }
}
