using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyFormationController : MonoBehaviour
{
    [Header(" Formation Settings ")]
    [Range(0f, 1f)]
    [SerializeField] private float radiusFactor;
    [Range(0f, 1f)]
    [SerializeField] private float angleFactor;

    public Transform Player;
    private float enemy2PlayerDistance;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float interactionDistance;

    [SerializeField] private bool isFightBegin = false;

    public List<GameObject> currentEnemiesList;

    private void OnEnable()
    {
        EventManager.OnFightBegin += FightBegin;
        EventManager.OnFightEnd += FightEnd;
    }

    private void OnDisable()
    {
        EventManager.OnFightBegin -= FightBegin;
        EventManager.OnFightEnd -= FightEnd;
    }

    private void Start()
    {
        switch (gameObject.tag)
        {
            case "FireCrowd":
                ChangeEnemyColor(Color.red);
                break;
            case "WaterCrowd":
                ChangeEnemyColor(Color.blue);
                break;
            case "EarthCrowd":
                ChangeEnemyColor(new Color32(0, 100, 0, 255));
                break;
            case "AirCrowd":
                ChangeEnemyColor(Color.gray);
                break;
            default:
                break;
        }

    }

    private void ChangeEnemyColor(Color color)
    {
        for (int i = 1; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).GetChild(1).GetComponent<Renderer>().material.color = color;
        }
    }

    void Update()
    {
        FermatSpiralPlacement();

        if (transform.childCount > 1 && !GameManager.isLevelFailed)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj)
            {
                Player = playerObj.transform;
                enemy2PlayerDistance = Vector3.Distance(Player.position, transform.position);

                if (enemy2PlayerDistance <= interactionDistance)
                {
                    if (!isFightBegin)
                    {
                        EventManager._onFightBegin();
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, Player.position, moveSpeed * Time.deltaTime);
                    }
                }
            }
            

        }
        else if(transform.childCount < 2)
        {
            Destroy(gameObject);
        }
    }

    private void FermatSpiralPlacement()
    {
        float goldenAngle = 137.5f * angleFactor;

        for (int i = 1; i < transform.childCount; i++)
        {
            float x = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i + 1));
            float z = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i + 1));

            Vector3 runnerLocalPosition = new Vector3(x, 0, z);
            transform.GetChild(i).localPosition = Vector3.Lerp(transform.GetChild(i).localPosition, runnerLocalPosition, 0.1f);
        }
    }

    private void FightBegin()
    {
        isFightBegin = true;

        currentEnemiesList = new List<GameObject>();
        for (int i = 1; i < transform.childCount; i++)
        {
            currentEnemiesList.Add(transform.GetChild(i).gameObject);
        }

    }

    private void FightEnd()
    {
        //Count text ile agent sayısı arasında conflict oluşması engelleniyor.
        int currentPlayerCount = int.Parse(transform.GetChild(0).GetComponent<TextMeshPro>().text);
        int childCount = transform.childCount - 1;
        isFightBegin = false;

        if (currentPlayerCount < childCount)
        {
            int subs = 0;
            for (int i = 0; i < childCount - currentPlayerCount; i++)
            {
                int value = childCount - subs;
                if(value >= 0 && transform.childCount >= value)
                Destroy(transform.GetChild(value).gameObject);
                subs++;
            }
        }
    }
}
