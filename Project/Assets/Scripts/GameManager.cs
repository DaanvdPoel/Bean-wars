using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject deathScreen;
    [Space]
    [SerializeField] private float pathLineWidth;
    [SerializeField] private Color lineColor;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    public void PlayerLost()
    {
        Time.timeScale = 0;
        deathScreen.SetActive(true);
    }

    public void Victory()
    {
        Time.timeScale = 0;
        winScreen.SetActive(true);
    }

    private void Start()
    {
        DrawLinesBetweenPathPoints();
        
    }

    private void DrawLinesBetweenPathPoints()
    {
        // the paths that have to be drawn for the player to see
        List<List<GameObject>> pathsToDraw = new List<List<GameObject>>
        {
            RefrenceManager.instance.blueGuardpathA,
            RefrenceManager.instance.blueGuardpathB,
            RefrenceManager.instance.blueGuardpathC
        };

        // Create and set all line renderers from all the paths
        foreach (List<GameObject> path in pathsToDraw)
        {
            for (int i = 0; i < path.Count; i++)
            {
                LineRenderer lr = path[i].AddComponent<LineRenderer>();
                lr.SetPosition(0, path[i].transform.position);
                lr.SetWidth(pathLineWidth, pathLineWidth);
                Material mat = new Material(Shader.Find("Unlit/Texture"));
                lr.material = mat;
                lr.SetColors(lineColor, lineColor);
                if (i != path.Count - 1)
                    lr.SetPosition(1, path[i + 1].transform.position);
                else if (i == path.Count - 1)
                    lr.SetPosition(1, path[0].transform.position);
            }
        }
    }
}
