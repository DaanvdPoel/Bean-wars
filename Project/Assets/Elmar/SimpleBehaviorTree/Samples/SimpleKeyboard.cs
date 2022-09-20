using UnityEngine;

namespace SimpleBehaviorTree.Examples
{
    public class SimpleKeyboard : MonoBehaviour
    {
        [Header("Steering Settings")]
        public float m_Speed = 4.0f;

        private void Update()
        {
            Vector3 velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * m_Speed;
            transform.position += velocity * Time.deltaTime;
            transform.LookAt(transform.position + velocity.normalized);
            Debug.DrawRay(transform.position, velocity, Color.red);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.Label(transform.position, "Player (keyboard)");
            UnityEditor.Handles.EndGUI();
#endif
        }
    }
}
