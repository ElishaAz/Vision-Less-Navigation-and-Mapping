#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Drone.Sensors
{
    public class CrashDetector : MonoBehaviour
    {
        [SerializeField] private bool pauseOnCrash = true;
        public bool InCrash { get; private set; }
        public int Crashes { get; private set; }

        private void OnCollisionEnter(Collision other)
        {
            if (Crashes == 0 && pauseOnCrash)
            {
#if UNITY_EDITOR
                EditorApplication.isPaused = true;
#endif
                pauseOnCrash = false;
            }

            InCrash = true;
            Crashes++;
        }

        private void OnCollisionExit(Collision other)
        {
            InCrash = false;
        }
    }
}