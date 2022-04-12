using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementFightRateManager : MonoBehaviour
{
    public Dictionary<FightingSides, string> fightingSides;
    void Start()
    {
        fightingSides = new Dictionary<FightingSides, string>();

        FightingSides Fire2Air = new FightingSides("FireCrowd","AirCrowd");
        FightingSides Air2Fire = new FightingSides("AirCrowd", "FireCrowd");
        fightingSides.Add(Fire2Air, "FireCrowd");
        fightingSides.Add(Air2Fire, "FireCrowd");

        FightingSides Fire2Earth = new FightingSides("FireCrowd", "EarthCrowd");
        FightingSides Earth2Fire = new FightingSides("EarthCrowd", "FireCrowd");
        fightingSides.Add(Fire2Earth, "EarthCrowd");
        fightingSides.Add(Earth2Fire, "EarthCrowd");

        FightingSides Fire2Water = new FightingSides("FireCrowd", "WaterCrowd");
        FightingSides Water2Fire = new FightingSides("WaterCrowd", "FireCrowd");
        fightingSides.Add(Fire2Water, "WaterCrowd");
        fightingSides.Add(Water2Fire, "WaterCrowd");

        FightingSides Earth2Water = new FightingSides("EarthCrowd", "WaterCrowd");
        FightingSides Water2Earth = new FightingSides("WaterCrowd", "EarthCrowd");
        fightingSides.Add(Earth2Water, "WaterCrowd");
        fightingSides.Add(Water2Earth, "WaterCrowd");

        FightingSides Air2Water = new FightingSides("AirCrowd", "WaterCrowd");
        FightingSides Water2Air = new FightingSides("WaterCrowd", "AirCrowd");
        fightingSides.Add(Air2Water, "AirCrowd");
        fightingSides.Add(Water2Air, "AirCrowd");

        FightingSides Air2Earth = new FightingSides("AirCrowd", "EarthCrowd");
        FightingSides Earth2Air = new FightingSides("EarthCrowd", "AirCrowd");
        fightingSides.Add(Air2Earth, "AirCrowd");
        fightingSides.Add(Earth2Air, "AirCrowd");

    }

}

public struct FightingSides
{
    private string playerTag;
    private string enemyTag;

    public FightingSides(string playerTag, string enemyTag)
    {
        this.playerTag = playerTag;
        this.enemyTag = enemyTag;
    }

    public string GetPlayerTag()
    {
        return playerTag;
    }

    public string GetEnemyTag()
    {
        return enemyTag;
    }
}