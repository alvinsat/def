using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    int playerIndex;
    [SerializeField]
    EActorState actorState;
    [SerializeField]
    EActorFaction actorFaction;
    [SerializeField]
    CActorsGameConfig actorConfig;
    LevelManager levelSys;
    bool isHoldingBall;
    [SerializeField]
    sCollidersType triggeres;
    float tInactive;
    Vector3 originalPos;
    int actorId;
    bool isDefenderAtHome;
    bool isDefenderLookingTarget;
    bool isDefenderChasing;
    bool isAttackerChasingBall;
    Vector3 targetGate;
    Transform targetedActor;
    Transform ballPosition;


    // Start is called before the first frame update
    void Start()
    {
        actorId = gameObject.GetInstanceID();
        originalPos = transform.position;
    }

    public void InitPlayerData(int playerIx, EActorFaction faction) {
        playerIndex = playerIx;
        actorFaction = faction;
        levelSys = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        actorConfig = levelSys.GetActorSpecs(faction);
        actorState = EActorState.Inactive;
    }

    // Update is called once per frame
    void Update()
    {
        StateManagement();
        DefenderBehaviour();
    }

    public void LookAroundAttacker() { 
    
    }

    public EActorFaction GetFaction() {
        return actorFaction;
    }

    void StateManagement() {
        // inactive state
        if (actorState == EActorState.Inactive)
        {
            if (actorFaction == EActorFaction.Attacker)
            {
                Debug.Log("Is greyscaled");
                if (tInactive < actorConfig.reactiveTime)
                {
                    tInactive += 1f * Time.deltaTime * Time.timeScale;
                }
                else
                {
                    ballPosition = levelSys.TakeBallObj().transform;
                    isAttackerChasingBall = true;
                    tInactive = 0f;
                    actorState = EActorState.Active;
                    Debug.Log("Is activated");
                }
                triggeres.anyactor.enabled = false;
            }
            else if (actorFaction == EActorFaction.Defender)
            {
                triggeres.anyactor.enabled = false;
                if (!isDefenderAtHome)
                {
                    if (Vector3.Distance(transform.position, originalPos) < 3f)
                    {
                        isDefenderAtHome = true;
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, originalPos, actorConfig.returnSpeed * Time.deltaTime * Time.timeScale);
                    }
                }
                else {
                    if (tInactive < actorConfig.reactiveTime)
                    {
                        tInactive += 1f * Time.deltaTime * Time.timeScale;
                    }
                    else {
                        actorState = EActorState.Active;
                        Debug.Log("Defender is activate");
                    }
                }
            }
        }
        // standby
        else if (actorState == EActorState.Standby)
        {
            // modify radius to 35% of battlefield
            isDefenderLookingTarget = true;
            triggeres.anyactor.enabled = true;
        }
        // active
        else if (actorState == EActorState.Active) 
        {
            if (actorFaction == EActorFaction.Defender)
            {
                actorState = EActorState.Standby;
                Debug.Log("Defender entering stand by");
            }
            else if (actorFaction == EActorFaction.Attacker) {
                if (!LevelManager.isBallCaptured)
                {
                    // goto ball
                    transform.position = Vector3.MoveTowards(transform.position, ballPosition.position, actorConfig.normalSpeed * Time.deltaTime * Time.timeScale);
                }
                else {
                    isAttackerChasingBall = false;
                    // goto gate
                    transform.position = Vector3.MoveTowards(transform.position, targetGate, actorConfig.carrySpeed * Time.deltaTime * Time.timeScale);
                }
            }
        }
    }

    void DefenderBehaviour() {
        if (isDefenderChasing) {
            transform.position = Vector3.MoveTowards(transform.position, targetedActor.position, actorConfig.normalSpeed * Time.deltaTime * Time.timeScale);
            // TODO: target chased, awaiting attackers 
        }
    }

    public void ReleaseBall() {
        if (isHoldingBall) {
            isHoldingBall = false;
            ballPosition.SetParent(null);
        }
    }

    void OnTriggerEnter(Collider col) {
        if (isDefenderLookingTarget)
        {
            if (col.TryGetComponent(out Actor lockTarget))
            {
                if (lockTarget.GetFaction() != actorFaction)
                {
                    if (lockTarget.isHoldingBall)
                    {
                        isDefenderLookingTarget = false;
                        isDefenderChasing = true;
                        targetedActor = lockTarget.transform;
                    }
                }
            }
        }
        else if (isAttackerChasingBall) {
            if (col.gameObject.CompareTag("ball")) {
                isHoldingBall = true;
                isAttackerChasingBall = false;
                ballPosition.SetParent(transform);
                LevelManager.isBallCaptured = true;
            }
        }
    }

    [System.Serializable]
    struct sCollidersType {
        public Collider solid;// trigger solid with environment
        public Collider system;// trigger for collision system
        public Collider anyactor;// trigger solid with any actor
    }
}
