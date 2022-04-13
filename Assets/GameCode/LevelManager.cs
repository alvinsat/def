using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    Slider[] uPlayerEnergyVis;// progression
    [SerializeField]
    Slider[] uPlayerEnergyValue;// highlighted

    [SerializeField]
    TextMeshProUGUI[] txtPlayerId;

    [SerializeField]
    float[] energyValueRegen;
    MainSystem mainSys;
    int i = 0;
    int playerCount;
    [SerializeField]
    int attackerCost;
    [SerializeField]
    int defenderCost;

    [SerializeField]
    TextMeshProUGUI txtTime;

    [SerializeField]
    GameObject templateActor;
    [SerializeField]
    int poolCount;

    [SerializeField]
    int nowMatchRound;
    int maxMatchRound;
    float nowTimeLimit;

    [SerializeField]
    sActors[] poolActors;

    [SerializeField]
    Transform[] gates;

    bool isStartTime;

    [SerializeField]
    Transform maxLeftBattlefield;
    [SerializeField]
    Transform maxRightBattlefield;

    public static bool isBallCaptured;
    [SerializeField]
    GameObject ballTracked;

    int attackerIndex;
    int defenderIndex;

    Vector3 calculatedPos;
    [SerializeField]
    [Tooltip("array order is the same as actorConfig")]
    GameInputHelper[] inputHelpers;

    // Start is called before the first frame update
    void Start()
    {
        mainSys = GameObject.Find("MainSystem").GetComponent<MainSystem>();
        playerCount = mainSys.gameConfigs.playerIdentification.Length;
        maxMatchRound = mainSys.matchPerGame.Length;
        // init energy vis and Sys
        InitEnergySys();
        energyValueRegen = new float[playerCount];
        // spawn players
        SpawnPlayersPool();
        // set game time
        nowTimeLimit = mainSys.gameConfigs.timeLimit;
        txtTime.SetText(nowTimeLimit.ToString()+"s");
        // Recolor all ally
        RecolorInit();
        // Input zone identify
        InputZoneIdentify();
        // sapwn cost
        InitSpawnCost();
        // init regen speed
        InitRegenSpeed();
        // Randomize ball
        SpawnBallRandomize();
    }

    public void SpawnBallRandomize() {
        // find attacker land
        i = 0;
        while (i < inputHelpers.Length) {
            if (inputHelpers[i].GetFaction() == EActorFaction.Attacker) {
                inputHelpers[i].RandomizeBallAt(ballTracked);
                ballTracked.SetActive(true);
            }
            i++;
        }
    }

    void RecolorInit()
    {
        i = 0;
        while (i < playerCount) {
            gates[i].GetComponent<MeshRenderer>().material.color = mainSys.gameConfigs.playerIdentification[i];
            int j = 0;
            while (j < gates[i].transform.childCount) {
                gates[i].transform.GetChild(j).GetComponent<MeshRenderer>().material.color = mainSys.gameConfigs.playerIdentification[i];
                j++;
            }
            i++;
        }
    }

    void InitSpawnCost() {
        attackerCost = mainSys.GetFactionCost(EActorFaction.Attacker);
        defenderCost = mainSys.GetFactionCost(EActorFaction.Defender);
    }

    void InputZoneIdentify() {
        EActorFaction faction = mainSys.matchPerGame[nowMatchRound];
        string result = "";
        inputHelpers[0].InitGameInput(mainSys.matchPerGame[nowMatchRound]);
        if (faction == EActorFaction.Attacker)
        {
            result = "(Attacker)";
        }
        else {
            result = "(Defender)";
        }
        txtPlayerId[0].SetText("Player 1 " + result);

        faction = mainSys.matchPerGame[nowMatchRound+1];
        if (faction == EActorFaction.Attacker)
        {
            result = "(Attacker)";
        }
        else
        {
            result = "(Defender)";
        }
        inputHelpers[1].InitGameInput(mainSys.matchPerGame[nowMatchRound + 1]);
        txtPlayerId[1].SetText("Player 2"+ result);
    }

    public GameObject TakeBallObj() {
        return ballTracked;
    }

    //public float GetDefenderDetectionRadius() {
    //    float dist = Vector3.Distance(maxLeftBattlefield.position, maxRightBattlefield.position);
    //    return (dist / 2.8f)/2f;
    //}

    public void StartGameTime() {
        isStartTime = true;
    }

    public void StopGameTime() {
        isStartTime = false;
    }

    public CActorsGameConfig GetActorSpecs(EActorFaction faction) {
        CActorsGameConfig cActors = new CActorsGameConfig();
        i = 0;
        while (i < mainSys.gameConfigs.actorGameConfig.Length) {
            if (mainSys.gameConfigs.actorGameConfig[i].faction == faction) {
                return mainSys.gameConfigs.actorGameConfig[i];
            }
            i++;
        }
        return cActors;
    }

    void InitRegenSpeed() {
        SetRegenSpeed(0, mainSys.GetEnergyRegenSpeed(mainSys.matchPerGame[nowMatchRound]));
        SetRegenSpeed(1, mainSys.GetEnergyRegenSpeed(mainSys.matchPerGame[nowMatchRound+1]));
    }


    GameObject o;
    Actor a;
    void SpawnPlayersPool()
    {
        // i = 0;
        // while (i < 2) {
        // o = Instantiate(templateActor);
        // a = o.GetComponent<Actor>();
        // a.InitPlayerData(i, mainSys.matchPerGame[nowMatchRound+i]);
        // o.SetActive(false);
        // poolActors[i].actors = new List<GameObject>();
        // poolActors[i].actors.Add(o);
        // int j = 0;
        // while (j < poolCount - 1) {
        // Instantiate(o);
        // poolActors[i].actors.Add(o);
        // j++;
        // }
        // i++;
        // }
        EActorFaction indexOfFaction = mainSys.matchPerGame[nowMatchRound];
        int iX = 0;
        if (indexOfFaction == EActorFaction.Attacker)
        {
            iX = 0;
        }
        else {
            iX = 1;
        }

        o = Instantiate(templateActor);
        a = o.GetComponent<Actor>();
        a.InitPlayerData(iX, mainSys.matchPerGame[nowMatchRound]);
        o.SetActive(false);
        poolActors[iX].actors = new List<GameObject>();
        poolActors[iX].actors.Add(o);
        int j = 0;
        while (j < poolCount - 1)
        {
            Instantiate(o);
            poolActors[iX].actors.Add(o);
            j++;
        }

        indexOfFaction = mainSys.matchPerGame[nowMatchRound + 1];
        iX = 0;
        if (indexOfFaction == EActorFaction.Attacker)
        {
            iX = 0;
        }
        else
        {
            iX = 1;
        }

        o = Instantiate(templateActor);
        a = o.GetComponent<Actor>();
        a.InitPlayerData(iX, mainSys.matchPerGame[nowMatchRound + 1]);
        o.SetActive(false);
        poolActors[iX].actors = new List<GameObject>();
        poolActors[iX].actors.Add(o);
        j = 0;
        while (j < poolCount - 1)
        {
            Instantiate(o);
            poolActors[iX].actors.Add(o);
            j++;
        }
    }

    void SetRegenSpeed(int playerIndex, float val) {
        energyValueRegen[playerIndex] = val;
    }

    void InitEnergySys() {
        energyValueRegen = new float[playerCount];
        i = 0;
        Color tempColor;
        while (i < playerCount)
        {
            uPlayerEnergyValue[i].value = 0f;
            uPlayerEnergyValue[i].maxValue = mainSys.gameConfigs.maxEnergy;
            uPlayerEnergyValue[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = mainSys.gameConfigs.playerIdentification[i];
            tempColor = mainSys.gameConfigs.playerIdentification[i];
            tempColor.r -= .3f;
            if (tempColor.r < 0f)
            {
                tempColor.r = 0f;
            }
            uPlayerEnergyVis[i].value = 0f;
            uPlayerEnergyVis[i].maxValue = mainSys.gameConfigs.maxEnergy;
            uPlayerEnergyVis[i].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = tempColor;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnergyRegenUiUpdate();
        TimeUpdates();
    }

    void TimeUpdates() {
        if (isStartTime) {
            nowTimeLimit -= 1f * Time.deltaTime * Time.timeScale;
            txtTime.SetText(nowTimeLimit.ToString() + "s");
        }
    }


    void EnergyRegenUiUpdate() {
        uPlayerEnergyVis[0].value += energyValueRegen[0] * Time.deltaTime * Time.timeScale;
        uPlayerEnergyVis[1].value += energyValueRegen[1] * Time.deltaTime * Time.timeScale;
        i = 0;
        while (i < playerCount) {
            if (uPlayerEnergyVis[i].value > 1f && uPlayerEnergyVis[i].value < 2f) {
                uPlayerEnergyValue[i].value = 1f;
            } else if (uPlayerEnergyVis[i].value > 2f && uPlayerEnergyVis[i].value < 3f)
            {
                uPlayerEnergyValue[i].value = 2f;
            } else if (uPlayerEnergyVis[i].value > 3f && uPlayerEnergyVis[i].value < 4f)
            {
                uPlayerEnergyValue[i].value = 3f;
            } else if (uPlayerEnergyVis[i].value > 4f && uPlayerEnergyVis[i].value < 5f)
            {
                uPlayerEnergyValue[i].value = 4f;
            } else if (uPlayerEnergyVis[i].value > 5f && uPlayerEnergyVis[i].value < 6f)
            {
                uPlayerEnergyValue[i].value = 5f;
            } else if (uPlayerEnergyVis[i].value >= 6f)
            {
                uPlayerEnergyValue[i].value = 6f;
            }
            i++;
        }
    }

    public void SpawnOf(EActorFaction faction, int playerIndex, Vector3 worldPos) {
        // cost and payment
        if (faction == EActorFaction.Attacker)
        {
            if (uPlayerEnergyValue[playerIndex].value < attackerCost)
            {
                Debug.Log("Not spawn attacker Energy is insufficient");
                return;
            }
            else
            {
                uPlayerEnergyVis[playerIndex].value -= attackerCost;
                uPlayerEnergyValue[playerIndex].value -= attackerCost;
            }
        }
        else if (faction == EActorFaction.Defender) {
            if (uPlayerEnergyValue[playerIndex].value < defenderCost)
            {
                Debug.Log("Not spawn defender Energy is insufficient");
                return;
            }
            else {
                uPlayerEnergyVis[playerIndex].value -= defenderCost;
                uPlayerEnergyValue[playerIndex].value -= defenderCost;
            }
        }
        // seek in pool
        i = 0;
        if (faction == EActorFaction.Attacker)
        {
            while (i < poolActors[0].actors.Count)
            {
                if (!poolActors[0].actors[i].activeInHierarchy)
                {
                    calculatedPos.x = worldPos.x;
                    calculatedPos.z = worldPos.z;
                    calculatedPos.y = poolActors[0].actors[i].transform.position.y;
                    poolActors[0].actors[i].transform.position = calculatedPos;
                    poolActors[0].actors[i].SetActive(true);
                    Debug.Log("Attacker is spawned");
                    return;
                }
                i++;
            }
        }
        else if(faction == EActorFaction.Defender) {
            while (i < poolActors[1].actors.Count)
            {
                if (!poolActors[1].actors[i].activeInHierarchy)
                {
                    calculatedPos.x = worldPos.x;
                    calculatedPos.z = worldPos.z;
                    calculatedPos.y = poolActors[1].actors[i].transform.position.y;
                    poolActors[1].actors[i].transform.position = calculatedPos;
                    poolActors[1].actors[i].SetActive(true);
                    Debug.Log("Defender is spawned");
                    return;
                }
                i++;
            }
        }
    }

    [System.Serializable]
    public struct sActors {
        public EActorFaction faction;
        public List<GameObject> actors;
    }

}
