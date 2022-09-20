using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefrenceManager : MonoBehaviour
{
    public static RefrenceManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    public Transform playerBase;
    public Transform enemyBase;
    [Space]
    public List<GameObject> redGuardpathA;
    public List<GameObject> redGuardpathB;
    public List<GameObject> redGuardpathC;
    [Space]          
    public List<GameObject> blueGuardpathA;
    public List<GameObject> blueGuardpathB;
    public List<GameObject> blueGuardpathC;

}
