using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputHelper : MonoBehaviour
{
    [SerializeField]
    EActorFaction eActorFaction;
    [SerializeField]
    LevelManager levelSys;
    [SerializeField]
    Transform minZ;
    [SerializeField]
    Transform maxZ;
    [SerializeField]
    Transform minX;
    [SerializeField]
    Transform maxX;
    Vector3 posResult;

    public void RandomizeBallAt(GameObject obj) {
        posResult.x = Random.Range(minX.position.x, maxX.position.x);
        posResult.z = Random.Range(minZ.position.z, minZ.position.z);
        posResult.y = obj.transform.position.y;
        obj.transform.position = posResult;
    }

    public void Interact(Vector3 pos) {
        if (eActorFaction == EActorFaction.Attacker)
        {
            levelSys.SpawnOf(eActorFaction, 0, pos);
        }
        else { 
            levelSys.SpawnOf(eActorFaction, 1, pos);
        }
    }

    public void InitGameInput(EActorFaction faction)
    {
        eActorFaction = faction;
    }

    public EActorFaction GetFaction() {
        return eActorFaction;
    }
}
