using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NokoGames.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Instance { get; private set; }

        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _accelerationFactor = 15f;
        [SerializeField] private float _velocityThreshold = 0.01f;
        [SerializeField] private FloatingJoystick _floatingJoystick;
        [SerializeField] private Animator _animator;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private Rigidbody _rigidbody;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _rigidbody = GetComponent<Rigidbody>();

            if (_rigidbody == null)
                Debug.LogError($"[PlayerMovement] Rigidbody bulunamadı: {gameObject.name}");
            if (_floatingJoystick == null)
                Debug.LogError("[PlayerMovement] FloatingJoystick atanmamış!");
        }

        private void FixedUpdate()
        {
            Move();
            Look();
            UpdateAnimation();
        }

        private void Move()
        {
            float horizontal = _floatingJoystick.Horizontal;
            float vertical   = _floatingJoystick.Vertical;

            Vector3 targetVelocity = new Vector3(
                horizontal * _moveSpeed,
                _rigidbody.velocity.y,
                vertical * _moveSpeed
            );

            _rigidbody.velocity = Vector3.Lerp(
                _rigidbody.velocity,
                targetVelocity,
                Time.fixedDeltaTime * _accelerationFactor
            );
        }

        private void Look()
        {
            Vector3 flatVelocity = new Vector3(
                _rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

            if (flatVelocity.sqrMagnitude < _velocityThreshold) return;

            Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.fixedDeltaTime * _rotationSpeed
            );
        }

        private void UpdateAnimation()
        {
            _animator.SetFloat(SpeedHash, _rigidbody.velocity.magnitude);
        }
    }
}