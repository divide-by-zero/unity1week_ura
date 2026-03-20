using UnityEngine;

namespace InfinitePorker
{
    public class AutoRotation : MonoBehaviour
    {
        [SerializeField] private float _speed;

        void Update()
        {
            transform.Rotate(new Vector3(0, 0, _speed * Time.deltaTime));
        }
    }
}