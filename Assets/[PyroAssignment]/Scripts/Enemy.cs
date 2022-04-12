using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Enemy : MonoBehaviour
{

    private FormationController PlayerFormationController;
    private EnemyFormationController EnemyFormationController;
    [SerializeField] private float interactionDistance;
    public GameObject FireSplash;
    public GameObject WaterSplash;
    public GameObject EarthSplash;
    public GameObject AirSplash;

    private GameObject manager;
    private Animator animator;
    private ParticleSystem blastParticleSystem;
    private ElementFightRateManager fightRateManager;

    private int playerSubsCount;
    private int enemySubsCount;

    private void OnEnable()
    {
        EventManager.OnFightBegin += FightBegin;
        EventManager.OnFightEnd += FightEnd;
        EventManager.OnLevelFail += LevelFail;
    }

    private void OnDisable()
    {
        EventManager.OnFightBegin -= FightBegin;
        EventManager.OnFightEnd -= FightEnd;
        EventManager.OnLevelFail -= LevelFail;
    }

    void Start()
    {
        PlayerFormationController = GameObject.Find("Crowd").GetComponent<FormationController>();
        EnemyFormationController = transform.parent.GetComponent<EnemyFormationController>();
        manager = GameObject.Find("Managers");
        if (manager)
        {
            blastParticleSystem = manager.GetComponent<ParticleSystem>();
            fightRateManager = manager.GetComponent<ElementFightRateManager>();
        }
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (EnemyFormationController.Player == null && !GameManager.isLevelFailed)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if (EnemyFormationController.Player)
            {
                EnemyFormationController.Player = player.transform;
            }
        }

        transform.LookAt(EnemyFormationController.Player);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            other.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
            CollisionCalc(other.gameObject, gameObject);
        }
    }

    /**
     * Karşılaşan grupların element tipine göre, aynı tipte ise her çarpışmada 1'er agent yok olur. 
     * Aynı tipte değillerse güçlü taraftan 1 giderken zayıf taraftan 2 gider.
     */
    private void CollisionCalc(GameObject player, GameObject enemy)
    {
        var playerTag = player.gameObject.transform.parent.tag;
        var enemyTag = enemy.gameObject.transform.parent.tag;
        playerSubsCount = 0;
        enemySubsCount = 0;

        var rand = new System.Random();
        var emitParams = new ParticleSystem.EmitParams();

        if (playerTag.Equals(enemyTag))
        {
            StartCoroutine(DestroyObject(player, playerTag, emitParams));
            StartCoroutine(DestroyObject(enemy, enemyTag, emitParams));
        }
        else
        {
            FightingSides sides = new FightingSides(playerTag, enemyTag);
            string winnerTag = fightRateManager.fightingSides[sides];

            if (winnerTag.Equals(playerTag))
            {
                StartCoroutine(DestroyObject(player, playerTag, emitParams));

                StartCoroutine(DestroyAnotherObject(enemy, rand, enemyTag, emitParams));
                StartCoroutine(DestroyObject(enemy, enemyTag, emitParams));
            }
            else
            {
                StartCoroutine(DestroyObject(enemy, enemyTag, emitParams));

                StartCoroutine(DestroyAnotherObject(player, rand, playerTag, emitParams));
                StartCoroutine(DestroyObject(player, playerTag, emitParams));
            }
        }

        PlayerFormationController.playerCount -= playerSubsCount;
        PlayerFormationController.SetPlayerCount(PlayerFormationController.playerCount - playerSubsCount);

        int enemycount = transform.parent.childCount -1;
        transform.parent.GetChild(0).GetComponent<TextMeshPro>().text = (enemycount - enemySubsCount).ToString();

    }

    IEnumerator DestroyObject(GameObject obj, string objTag, ParticleSystem.EmitParams emitParams)
    {        
        InstantiateSplash(objTag, obj.transform.position);
        emitParams.position = obj.transform.position;
        emitParams.startColor = obj.transform.GetChild(1).GetComponent<Renderer>().material.color;
        blastParticleSystem.Emit(emitParams, 1);

        Destroy(obj);

        if (obj.CompareTag("Player"))
        {
            playerSubsCount += 1;
        }
        else
        {
            enemySubsCount += 1;
        }

        yield return new WaitForSeconds(0.25f);

    }

    IEnumerator DestroyAnotherObject(GameObject obj, System.Random rand, string objTag, ParticleSystem.EmitParams emitParams)
    {
        var objList = new List<GameObject>();

        if (obj.CompareTag("Player"))
        {
            objList = obj.transform.parent.GetComponent<FormationController>().currentPlayersList.Where(i => i != null && i != obj && i.GetComponent<Collider>().enabled == true).ToList();
        }
        else
        {
            objList = obj.transform.parent.GetComponent<EnemyFormationController>().currentEnemiesList.Where(i => i != null && i != obj && i.GetComponent<Collider>().enabled == true).ToList();
        }
        
        if(objList.Count > 0)
        {
            var anyOtherObject = objList.ElementAt(rand.Next(0, objList.Count() - 1));

            if (anyOtherObject)
            {
                InstantiateSplash(objTag, anyOtherObject.transform.position);
                emitParams.position = anyOtherObject.transform.position;
                emitParams.startColor = anyOtherObject.transform.GetChild(1).GetComponent<Renderer>().material.color;
                blastParticleSystem.Emit(emitParams, 1);

                Destroy(anyOtherObject);

                if (obj.CompareTag("Player"))
                {
                    playerSubsCount += 1;
                }
                else
                {
                    enemySubsCount += 1;
                }
            }
        }

        yield return new WaitForSeconds(0.25f);
    }

    private void InstantiateSplash(string tag, Vector3 position)
    {
        switch (tag)
        {
            case "FireCrowd":
                Instantiate(FireSplash, position, FireSplash.transform.rotation);
                break;
            case "WaterCrowd":
                Instantiate(WaterSplash, position, WaterSplash.transform.rotation);
                break;
            case "EarthCrowd":
                Instantiate(EarthSplash, position, EarthSplash.transform.rotation);
                break;
            case "AirCrowd":
                Instantiate(AirSplash, position, AirSplash.transform.rotation);
                break;
            default:
                break;
        }
    }

    private void FightBegin()
    {
        animator.SetBool("isRunning", true);
    }

    private void FightEnd()
    {
        animator.SetBool("isRunning", false);
    }

    private void LevelFail()
    {
        animator.enabled = false;
    }
}
