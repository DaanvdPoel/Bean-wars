using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float m_range_x = 3.0f;
    public float m_range_z = 3.0f;
    public float m_speed_x = 2.0f;
    public float m_speed_z = 2.0f;

    private Vector3 m_position;

    // Start is called before the first frame update
    void Start()
    {
        m_position = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Set the x and z position to loop between 0 and specified range
        transform.position = m_position + new Vector3(m_range_x != 0.0f ? Mathf.PingPong(Time.fixedTime * m_speed_x, m_range_x) : 0.0f,
                                                      0.0f,
                                                      m_range_z != 0.0f ? Mathf.PingPong(Time.fixedTime * m_speed_z, m_range_z) : 0.0f);
    }
}
