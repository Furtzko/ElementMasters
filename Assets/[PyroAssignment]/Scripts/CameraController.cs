using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    private Vector3 newPosition;
    [SerializeField]private float smoothTime = 0.8f; 

    void Start()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (!GameManager.isLevelFailed && !GameManager.isLevelCompleted)
        {
            if (GameManager.isHadouken && !GameManager.isLevelCompleted)
            {
                target = GameObject.FindGameObjectsWithTag("Hadouken").Where(i => i.GetComponent<HadoukenController>().isActive).FirstOrDefault().transform;
            }

            newPosition = new Vector3(transform.position.x, transform.position.y, offset.z + target.position.z);
            transform.position = Vector3.Slerp(transform.position, newPosition, smoothTime * Time.deltaTime);
        }
    }

}
