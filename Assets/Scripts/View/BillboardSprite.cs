using UnityEngine;

[ExecuteAlways]
public class BillboardSprite : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position - _targetCamera.transform.forward);
    }
}