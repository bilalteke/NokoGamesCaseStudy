using UnityEngine;

namespace NokoGames.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _smoothSpeed = 0.125f;

        private void FixedUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPos = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desiredPos, _smoothSpeed);
        }
    }
}