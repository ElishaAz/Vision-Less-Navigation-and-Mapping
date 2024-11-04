using UnityEngine;
using UnityEngine.InputSystem;

namespace Algorithms
{
    [RequireComponent(typeof(Drone.Drone))]
    public class KeyboardControl : MonoBehaviour
    {
        private Drone.Drone drone;
        private InputAction rollAction, pitchAction, yawAction, throttleAction;

        private void Awake()
        {
            drone = GetComponent<Drone.Drone>();

            rollAction = InputSystem.actions.FindAction("Roll");
            pitchAction = InputSystem.actions.FindAction("Pitch");
            yawAction = InputSystem.actions.FindAction("Yaw");
            throttleAction = InputSystem.actions.FindAction("Throttle");
        }

        private void FixedUpdate()
        {
            float roll = rollAction.ReadValue<float>();
            float pitch = pitchAction.ReadValue<float>();
            float yaw = yawAction.ReadValue<float>();
            float throttle = throttleAction.ReadValue<float>();

            // if (Input.GetKey(KeyCode.UpArrow))
            // {
            // 	pitch = 1;
            // }
            //
            // if (Input.GetKey(KeyCode.DownArrow))
            // {
            // 	pitch = -1;
            // }
            //
            // if (Input.GetKey(KeyCode.RightArrow))
            // {
            // 	yaw = 1;
            // }
            //
            // if (Input.GetKey(KeyCode.LeftArrow))
            // {
            // 	yaw = -1;
            // }
            // Debug.Log($"{roll}, {pitch}, {yaw}, {throttle}");

            drone.RC(roll, pitch, yaw, 0);
        }
    }
}