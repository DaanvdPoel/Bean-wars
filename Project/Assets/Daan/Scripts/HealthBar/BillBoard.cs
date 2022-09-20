using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private Transform camara;

    private void Start()
    {
        FindObjectOfType<Camera>();
    }

    //rotates the health bar to the camera
    private void LateUpdate()
    {
        transform.LookAt(transform.position + camara.forward);
    }
}
