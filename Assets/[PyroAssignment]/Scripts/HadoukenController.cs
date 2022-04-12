using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HadoukenController : MonoBehaviour
{
    public bool isActive;

    private int playerCount;
    private int maxCollisionCount;
    private int hadoukenCollisionCount;
    public float multiplierValue;
    public int score;

    public GameObject FireBurst;
    public GameObject WaterBurst;
    public GameObject EarthBurst;
    public GameObject AirBurst;
    private GameObject activeBlast;

    private Transform crowd;

    private void OnEnable()
    {
        EventManager.OnFinishLinePassed += FinishLinePassed;
        EventManager.OnHadouken += Hadouken;
    }

    private void OnDisable()
    {
        EventManager.OnFinishLinePassed -= FinishLinePassed;
        EventManager.OnHadouken -= Hadouken;
    }

    private void Start()
    {
        crowd = GameObject.Find("Crowd").transform;

        isActive = false;
        playerCount = 0;
        hadoukenCollisionCount = 0;
    }

    void Update()
    {
        if (GameManager.isFinishLinePassed || GameManager.isHadouken)
        {
            if(transform.localScale.magnitude <= new Vector3(1.5f, 1.5f, 1.5f).magnitude)
            {
                transform.localScale += new Vector3(0.005f, 0.005f, 0.005f);
            }
        }
    }

    private void FinishLinePassed()
    {
        playerCount = crowd.GetComponent<FormationController>().playerCount;
        maxCollisionCount = CalcMaxCollisionCount(playerCount);

        switch (crowd.tag)
        {
            case "FireCrowd":
                if (transform.name.Equals("HadoukenFire"))
                {
                    Enable(crowd);
                    activeBlast = FireBurst;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case "WaterCrowd":
                if (transform.name.Equals("HadoukenWater"))
                {
                    Enable(crowd);
                    activeBlast = WaterBurst;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case "EarthCrowd":
                if (transform.name.Equals("HadoukenEarth"))
                {
                    Enable(crowd);
                    activeBlast = EarthBurst;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case "AirCrowd":
                if (transform.name.Equals("HadoukenAir"))
                {
                    Enable(crowd);
                    activeBlast = AirBurst;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }

    }

    private void Enable(Transform crowd)
    {
        transform.position = crowd.transform.position + Vector3.up * 2;
        GetComponent<SwervingController>().enabled = true;
        isActive = true;
    }

    private void Hadouken()
    {
        activeBlast.transform.position = transform.position;
        activeBlast.GetComponent<ParticleSystem>().Emit(700);

        transform.position = transform.position + Vector3.down;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Multiplier"))
        {
            hadoukenCollisionCount++;

            if (hadoukenCollisionCount.Equals(maxCollisionCount))
            {
                score = Mathf.FloorToInt(playerCount * multiplierValue);
                EventManager._onLevelCompleted();
                gameObject.SetActive(false);
            }
        }
    }

    private int CalcMaxCollisionCount(int playerCount)
    {
        if(playerCount > 580)
        {
            multiplierValue = 5f;
            return 20;
        }
        else if (playerCount < 40)
        {
            multiplierValue = 1.2f;
            return 1;
        }
        else
        {
            multiplierValue =  1.2f + ( Mathf.FloorToInt( (playerCount - 10) / 30 ) * 0.2f );

            return Mathf.FloorToInt( (multiplierValue - 1.2f) / 0.2f ) + 1;
        }

    }
}
