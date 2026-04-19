using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NokoGames.Stack
{
    public class StackItem : MonoBehaviour
    {
        [SerializeField] private ItemDefinition _itemDefinition;
        [SerializeField] private float _stackSpacing = 0.3f;

        public ItemDefinition ItemDefinition => _itemDefinition;
        public float StackSpacing => _stackSpacing;
        public bool IsAnimating { get; private set; }
        public Vector3 OriginalScale { get; private set; }
        public Quaternion OriginalRotation { get; private set; }

        [HideInInspector] public Vector3 velocityRef;

        private void Awake()
        {
            OriginalScale = transform.localScale;
            OriginalRotation = transform.localRotation;
        }

        public void SetAnimating(bool value)
        {
            IsAnimating = value;
        }

        public void OnPickedUp() { }
        public void OnDropped() => velocityRef = Vector3.zero;

        public void Follow(Transform target, float followSpeed, float rotationSpeed, float spacing)
        {
            Vector3 targetPos = target.position + target.up * spacing;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocityRef, followSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationSpeed);
        }
    }
}

