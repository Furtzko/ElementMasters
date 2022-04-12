using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class FormationController : MonoBehaviour
{
    [Header(" Formation Settings ")]
    [Range(0f, 1f)]
    [SerializeField] private float radiusFactor;
    [Range(0f, 1f)]
    [SerializeField] private float angleFactor;
    [SerializeField] private float movementSpeed;

    [Header(" Settings ")]
    [SerializeField] private Player playerPrefab;


    [SerializeField] private bool isFightBegin = false;

    private Transform Enemy;
    public List<GameObject> EnemiesInRange = new List<GameObject>();

    public GameObject triggeredGate;

    public int playerCount;
    public List<GameObject> currentPlayersList;
    public TextMeshPro playerCountText;


    private void OnEnable()
    {
        EventManager.OnFightBegin += FightBegin;
        EventManager.OnFightEnd += FightEnd;
        EventManager.OnGateTriggered += GateTriggered;
        EventManager.OnLevelFail += LevelFail;
        EventManager.OnFinishLinePassed += FinishLinePassed;
        EventManager.OnHadouken += Hadouken;
    }

    private void OnDisable()
    {
        EventManager.OnFightBegin -= FightBegin;
        EventManager.OnFightEnd -= FightEnd;
        EventManager.OnGateTriggered -= GateTriggered;
        EventManager.OnLevelFail -= LevelFail;
        EventManager.OnFinishLinePassed -= FinishLinePassed;
        EventManager.OnHadouken -= Hadouken;
    }

    private void Start()
    {
        playerCountText = transform.GetChild(0).GetComponent<TextMeshPro>();
        playerCount = 1;
        SetPlayerCount(playerCount);

        movementSpeed = 0.5f;
    }


    void Update()
    {
        FermatSpiralPlacement(movementSpeed);

        if (transform.childCount > 1)
        {
            if (isFightBegin)
            {
                if (EnemiesInRange.Count > 0 && EnemiesInRange.Find(x => x != null))
                {
                    Enemy = EnemiesInRange.Find(x => x != null && x.CompareTag("Enemy")).transform;

                    transform.position = Vector3.Lerp(transform.position, Enemy.position, 0.5f * Time.deltaTime);
                }
                else
                {
                    if (EnemiesInRange.Count > 0 && !EnemiesInRange.Find(x => x != null))
                    {
                        EventManager._onFightEnd();
                    }

                }
            }

            if (GameManager.isFinishLinePassed)
            {
                for (int i = 1; i < transform.childCount; i++)
                {
                    transform.GetChild(i).transform.position = Vector3.Lerp(transform.GetChild(i).transform.position, transform.GetChild(0).transform.position, 2f * Time.deltaTime);

                    if (Vector3.Distance(transform.GetChild(0).transform.position, transform.GetChild(i).transform.position) < 0.3f)
                    {
                        Destroy(transform.GetChild(i).gameObject);
                    }
                }
            }
        }
        else if (transform.childCount == 1 && GameManager.isFinishLinePassed && !GameManager.isHadouken)
        {
            EventManager._onHadouken();
        }
        else if(transform.childCount < 2 && !GameManager.isFinishLinePassed)
        {
            EventManager._onLevelFail();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !EnemiesInRange.Contains(other.gameObject)) 
        {
            EnemiesInRange.Add(other.gameObject);
        }
        
    }

    IEnumerator SetMovementSpeed()
    {
        movementSpeed = 5f;
        yield return new WaitForSeconds(1f);
        movementSpeed = 0.5f;
    }

    private void GateTriggered()
    {
        StartCoroutine(SetMovementSpeed());

        //Gateden kalabalık geçildiği koşullarda gate birden fazla kez triggerlanabiliyordu, bunu önlemek için konuldu.
        if (!triggeredGate.GetComponent<Collider>().isTrigger)
        {
            return;
        }
        triggeredGate.GetComponent<Collider>().isTrigger = false;
        triggeredGate.transform.parent.parent.GetChild((triggeredGate.transform.parent.GetSiblingIndex() + 1) % 2).GetChild(0).GetComponent<Collider>().isTrigger = false;

        playerCount = int.Parse(transform.Find("PlayerCountText").GetComponent<TextMeshPro>().text);

        switch (triggeredGate.tag)
        {
            case "FireGate":
                changePlayerColor(Color.red, playerCount);
                gameObject.tag = "FireCrowd";
                break;
            case "WaterGate":
                changePlayerColor(Color.blue, playerCount);
                gameObject.tag = "WaterCrowd";
                break;
            case "EarthGate":
                changePlayerColor(new Color32(0, 100, 0, 255), playerCount);
                gameObject.tag = "EarthCrowd";
                break;
            case "AirGate":
                changePlayerColor(Color.gray, playerCount);
                gameObject.tag = "AirCrowd";
                break;
            default:
                break;
        }

        int agentsToAdd = BonusUtils.GetRunnersAmountToAdd(playerCount, triggeredGate.GetComponent<Gate>().bonus);

        playerCount += agentsToAdd;

        SetPlayerCount(playerCount);

        AddRunners(agentsToAdd);

    }

    private void changePlayerColor(Color color, int playerCount)
    {
        for(int i = 0; i < playerCount; i++)
        {
            transform.GetChild(i+1).GetChild(1).GetComponent<Renderer>().material.color = color;
        }
    }

    private void FightBegin()
    {
        gameObject.GetComponent<SwervingController>().enabled = false;
        isFightBegin = true;

        currentPlayersList = new List<GameObject>();
        for (int i = 1; i < transform.childCount; i++)
        {
            currentPlayersList.Add(transform.GetChild(i).gameObject);
        }

    }

    private void FightEnd()
    {
        gameObject.GetComponent<SwervingController>().enabled = true;
        isFightBegin = false;
        EnemiesInRange = new List<GameObject>();

        //Count text ile agent sayısı arasında conflict oluşması engelleniyor.
        int currentPlayerCount = int.Parse(transform.GetChild(0).GetComponent<TextMeshPro>().text);
        int childCount = transform.childCount - 1;

        if (currentPlayerCount < childCount)
        {
            int subs = 0;
            for(int i = 0; i < childCount - currentPlayerCount; i++)
            {
                Destroy(transform.GetChild(childCount - subs).gameObject);
                subs++;
            }
        }

    }

    private void FermatSpiralPlacement(float movementSpeed)
    {
        float goldenAngle = 137.5f * angleFactor;

        for (int i = 1; i < transform.childCount; i++)
        {
            float x = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Cos(Mathf.Deg2Rad * goldenAngle * (i + 1));
            float z = radiusFactor * Mathf.Sqrt(i + 1) * Mathf.Sin(Mathf.Deg2Rad * goldenAngle * (i + 1));

            Vector3 runnerLocalPosition = new Vector3(x, 0, z);
            transform.GetChild(i).localPosition = Vector3.Lerp(transform.GetChild(i).localPosition, runnerLocalPosition, movementSpeed * Time.deltaTime);
        }
    }

    public void AddRunners(int amount)
    {
        if (playerPrefab == null)
        {
            playerPrefab = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        for (int i = 0; i < amount; i++)
        {
            Player playerInstance = Instantiate(playerPrefab, transform.position + (Vector3.up * i / 2), transform.rotation, transform);

            playerInstance.name = "Player_" + playerInstance.transform.GetSiblingIndex();
        }
    }

    public void SetPlayerCount(int count)
    {
        playerCountText.text = count.ToString();
    }

    private void LevelFail()
    {
        gameObject.GetComponent<SwervingController>().enabled = false;
        Destroy(gameObject);
    }

    private void FinishLinePassed()
    {
        Invoke("DestroyCountText", 1f); 
    }

    private void DestroyCountText()
    {
        Destroy(transform.GetChild(0).gameObject);
    }

    private void Hadouken()
    {
        transform.GetChild(0).parent = null;

        Invoke("DestroyCrowd", 0.5f);
    }

    private void DestroyCrowd()
    {
        Destroy(gameObject);
    }

}
