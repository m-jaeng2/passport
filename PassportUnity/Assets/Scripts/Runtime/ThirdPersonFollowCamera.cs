using UnityEngine;

namespace RhythmPassport.Runtime
{
    public sealed class ThirdPersonFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 positionOffset = new Vector3(0f, 5.5f, -8f);
        [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.5f, 0f);
        [SerializeField] private float followSmoothTime = 0.2f;
        [SerializeField] private float rotationSmoothSpeed = 8f;

        private Vector3 velocity;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var desiredPosition = target.position + positionOffset;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                followSmoothTime);

            var lookTarget = target.position + lookOffset;
            var desiredRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotationSmoothSpeed * Time.deltaTime);
        }
    }
}
