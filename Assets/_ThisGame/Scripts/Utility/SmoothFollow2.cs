//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ALIyerEdon;
namespace ALIyerEdon
{
    public class SmoothFollow2 : MonoBehaviour
    {
        public Transform target;

        public float smooth = 0.33f;
        public float distance = 4.56f;
        public float height = 0.5f;
        public float Angle = 6.39f;
        [HideInInspector] public float daynamicCameraIntensity = 0.5f;

        public LayerMask lineOfSightMask = 0;

        private Rigidbody myRigidbody;
        private float yVelocity = 0.0f;
        private float xVelocity = 0.0f;

        bool started;

        IEnumerator Start()
        {

            yield return new WaitForEndOfFrame();

            target = GameObject.FindGameObjectWithTag("Player").transform;

            myRigidbody = target.GetComponent<Rigidbody>();

            //Face the camera to the car forward at start
            transform.position = new Vector3(target.position.x,
                target.position.y + 300f, target.position.z);

            transform.LookAt(GameObject.FindGameObjectWithTag("Player")
                    .GetComponent<Car_AI>().rayPosition);

            started = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!started)
                return;

            // Damp angle from current y-angle towards target y-angle
            float xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x,
            target.eulerAngles.x + Angle, ref xVelocity, smooth);

            float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            target.eulerAngles.y, ref yVelocity, smooth);

            // Look at the target
            transform.eulerAngles = new Vector3(xAngle, yAngle, 0.0f);

            var direction = transform.rotation * -Vector3.forward;
            var targetDistance = AdjustLineOfSight(target.position + new Vector3(0, height, 0), direction);

            transform.position = target.position + new Vector3(0, height, 0) + direction * targetDistance;

        }

        float AdjustLineOfSight(Vector3 target, Vector3 direction)
        {
            RaycastHit hit;

            if (Physics.Raycast(target, direction, out hit, distance, lineOfSightMask.value))
                return hit.distance;
            else
                return distance;
        }
    }
}