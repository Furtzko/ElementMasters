using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwervingController : MonoBehaviour
{
    private SwerveInputSystem _swerveInputSystem;
    [SerializeField] private float swerveSpeed = 0.5f;
    [SerializeField] private float maxSwerveAmount = 1f;
    [SerializeField] private float forwardSpeed = 3.5f;

    private void OnEnable()
    {
        EventManager.OnHadouken += Hadouken;
    }

    private void OnDisable()
    {
        EventManager.OnHadouken -= Hadouken;
    }

    private void Awake()
    {
        _swerveInputSystem = GetComponent<SwerveInputSystem>();
    }

    private void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.5f, 1.5f), transform.position.y, transform.position.z);

        float swerveAmount = Time.deltaTime * swerveSpeed * _swerveInputSystem.MoveFactorX;
        swerveAmount = Mathf.Clamp(swerveAmount, -maxSwerveAmount, maxSwerveAmount);
        if (GameManager.isFinishLinePassed || GameManager.isHadouken)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.localPosition.x, 0, Time.deltaTime * 1), transform.position.y, transform.position.z);
            transform.Translate(0, 0, forwardSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(swerveAmount, 0, forwardSpeed * Time.deltaTime);
        }
        
    }

    private void Hadouken()
    {
        Invoke("IncreaseForwardSpeed", 1f);
    }

    private void IncreaseForwardSpeed()
    {
        forwardSpeed = 10f;
    }
}
