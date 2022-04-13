using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSystem : MonoBehaviour
{
    public CGameConfig gameConfigs;
    [Tooltip("This is total rounds.. where it is viewable from player 0 perspective. length should be odd")]
    public EActorFaction[] matchPerGame;
	
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public int GetFactionCost(EActorFaction faction) {
        int i = 0;
        while (i < gameConfigs.actorGameConfig.Length)
        {
            if (gameConfigs.actorGameConfig[i].faction == faction)
            {
                return gameConfigs.actorGameConfig[i].energyCost;
            }
            i++;
        }
        return -1;
    }

    public EActorFaction GetFactionByIndex(int iX) {
        return gameConfigs.actorGameConfig[iX].faction;
    }

    public float GetEnergyRegenSpeed(EActorFaction faction, out EActorFaction oponentFaction) {
        int i = 0;
        while (i < gameConfigs.actorGameConfig.Length) {
            if (gameConfigs.actorGameConfig[i].faction == faction) {
                if (faction == EActorFaction.Attacker)
                {
                    oponentFaction = EActorFaction.Defender;
                }
                else {
                    oponentFaction = EActorFaction.Attacker;
                }
                return gameConfigs.actorGameConfig[i].energyRegen;
            }
            i++;
        }
        oponentFaction = EActorFaction.Attacker;
        return -1f;// not found anything
    }
    
    public float GetEnergyRegenSpeed(EActorFaction faction) {
        int i = 0;
        while (i < gameConfigs.actorGameConfig.Length) {
            if (gameConfigs.actorGameConfig[i].faction == faction) {
                return gameConfigs.actorGameConfig[i].energyRegen;
            }
            i++;
        }
        Debug.Log("Not found any regen stats");
        return -1f;// not found anything
    }
}

[System.Serializable]
public enum EActorState {
    Inactive,
    Active,
    Standby
}

[System.Serializable]
public enum EActorFaction { 
    Attacker,
    Defender
}

[System.Serializable]
public class CActorsGameConfig {
    public EActorFaction faction;
    public float energyRegen;
    public int energyCost;
    public float spawnTime;
    public float reactiveTime;
    public float normalSpeed;
    public float carrySpeed;
    public float ballSpeed;
    public float returnSpeed;
}

[System.Serializable]
public class CGameConfig {
    [Header("Gameplay specific")]
    public int timeLimit;
    public float maxEnergy;
    public float energyRegenSpeed;

    //[Tooltip("in percentage of battlefield width")]
    //[Range(1f,100f)]
    //public float detectionArea;
    public CActorsGameConfig[] actorGameConfig;
    [Tooltip("The length is also for player max count")]
    public Color[] playerIdentification;
}

[System.Serializable]
public class CSaveGame{
    public int currentRoundCount;
    public float remainingTime;
}