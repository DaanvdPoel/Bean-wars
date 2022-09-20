using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TestEnum
{
    ChangeHealthUp = 1,
    ChangeHealthDown =  2
}

[System.Serializable]
public class CameraMover : MonoBehaviour
{
    public void MoveCamera(int rec)
    {
        switch (rec)
        {
            case (int)TestEnum.ChangeHealthDown:
                transform.position += Vector3.up;
                break;
            case (int)TestEnum.ChangeHealthUp:
                if (true)
                {

                }
                transform.position -= Vector3.up;
                break;
        }
    }
}
