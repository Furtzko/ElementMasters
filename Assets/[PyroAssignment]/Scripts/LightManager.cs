using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    void Update()
    {
        if (GameManager.isFinishLinePassed)
        {
            GetComponent<Light>().intensity = Mathf.Lerp(1.6f, 0.8f, 0.3f);                                                                           
        }
    }
}
