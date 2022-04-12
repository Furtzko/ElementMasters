using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    public LayerMask Gate;

    private FormationController playerCrowd;
    private Animator animator;

    private void OnEnable()
    {
        EventManager.OnGateTriggered += GateTriggered;
        EventManager.OnHadouken += Hadouken;

        playerCrowd = transform.parent.GetComponent<FormationController>();
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", true);
        animator.SetTrigger("isGateTriggered");
    }

    private void OnDisable()
    {
        EventManager.OnGateTriggered -= GateTriggered;
        EventManager.OnHadouken -= Hadouken;
    }

    private void Update()
    {
        if (transform.position.y < -1f && !GameManager.isFinishLinePassed)
        {
            playerCrowd.playerCount -= 1;
            playerCrowd.SetPlayerCount(playerCrowd.playerCount - 1);

            Destroy(gameObject);

        }
    }
        
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Gate")))
        {
            playerCrowd.triggeredGate = other.gameObject;

            EventManager._onGateTriggered();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);

            ParticleSystem.MainModule main = other.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            main.startColor = transform.GetChild(1).GetComponent<Renderer>().material.color;
            other.gameObject.GetComponentInChildren<ParticleSystem>().Play();

            playerCrowd.playerCount -= 1;
            playerCrowd.SetPlayerCount(playerCrowd.playerCount - 1);
        }
        else if (other.gameObject.CompareTag("FinishLine"))
        {
            other.enabled = false;
            EventManager._onFinishLinePassed();
        }
        else if (other.name.Equals("NoTrespassing") && !GameManager.isHadouken)
        {
            EventManager._onHadouken();
        }
    }

    private void GateTriggered()
    {
        animator.SetTrigger("isGateTriggered");
    }

    private void Hadouken()
    {
        animator.SetTrigger("isHadouken");
    }

}
