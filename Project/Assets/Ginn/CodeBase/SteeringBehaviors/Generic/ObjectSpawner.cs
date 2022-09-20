using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public enum SpawnMode { RandomSpawn, RowColSpawn }

    [Header("Flocking")]
    public SpawnMode     m_mode     = SpawnMode.RandomSpawn;
    public GameObject    m_prefab;

    [Header("RandomSpawn")]
    public int           m_numSpawns =  50;
    public float         m_xMin      = -25.0f;
    public float         m_xMax      =  25.0f;
    public float         m_zMin      = -25.0f;
    public float         m_zMax      =  25.0f;

    [Header("RowColSpawn")]
    public int           m_numRows   =  10;
    public int           m_numCols   =  10;
    public float         m_colWidth  =   5.0f;
    public float         m_rowHeight =   5.0f;
    public float         m_topLeftX  = -22.5f;
    public float         m_topLeftZ  =  22.5f;

    void Start()
    {
        switch (m_mode)
        {
            case SpawnMode.RandomSpawn: RandomSpawn(); break;
            case SpawnMode.RowColSpawn: RowColSpawn(); break;
        }
    }

    //------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------
    void RandomSpawn()
    {
        for (int i = 0; i < m_numSpawns; i++)
        {
            float   spawnPointX   = Random.Range(m_xMin, m_xMax);
            float   spawnPointY   = Random.Range(m_zMin, m_zMax);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0.0f, spawnPointY);
            Instantiate(m_prefab, spawnPosition, Quaternion.identity);
        }
    }

    //------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------
    void RowColSpawn()
    {
        float y = m_topLeftZ;
        for (int row = 0; row < m_numRows; ++row)
        {
            float x = m_topLeftX;
            for (int col=0; col<m_numCols; ++col)
            {
                Vector3 spawnPosition = new Vector3(x, 0.0f, y);
                Instantiate(m_prefab, spawnPosition, Quaternion.identity);
                x += m_colWidth;
            }
            y -= m_rowHeight;
        }
    }
}
