using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gate : MonoBehaviour
{
    public Player Player;
    public Bonus bonus;

    void Start()
    {
        //Gatelere random bonus textler atanır.
        bonus = BonusUtils.GetRandomBonus();
        transform.parent.GetChild(3).GetComponent<TextMeshPro>().text = BonusUtils.GetBonusString(bonus);
    }

}
