using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public GameObject target;
    public float walkingSpeed;
    public float runningSpeed;
    public GameObject ragdoll;

    public Animator anim;
    NavMeshAgent agent;

    enum STATE {
        IDLE,
        WANDER,
        ATTACK,
        CHASE,
        DEAD
    }
    STATE state = STATE.IDLE;

    // public void setState(STATE s) {
    //     state = s;
    // }

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     if(Random.Range(0,100) < 50)
        //     {
        //         GameObject doll = Instantiate(ragdoll, this.transform.position, this.transform.rotation);
        //         doll.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
        //         Destroy(this.gameObject);
        //     }
        //     else
        //     {
        //         state = STATE.DEAD;
        //         TurnOffTriggers();
        //         anim.SetBool("isDead", true);                
        //     }

           
        //     return;
        // }

        if (target == null)
        {            
            target = GameObject.FindWithTag("Player");
            return;
        }

        // Finite State machine
        switch (state)
        {
            case STATE.IDLE:
                if(CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0,5000) < 5) {
                    state = STATE.WANDER;
                }
                
                break;
            case STATE.WANDER:
                if(!agent.hasPath)
                {
                    float newX = this.transform.position.x + Random.Range(-5,5);
                    float newZ = this.transform.position.z + Random.Range(-5,5);
                    float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
                    Vector3 dest = new Vector3(newX, newY, newZ);
                    agent.SetDestination(dest);
                    agent.stoppingDistance = 0;
                    TurnOffTriggers();
                    agent.speed = walkingSpeed;
                    anim.SetBool("isWalking", true);
                }

                if(CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0,5000) < 5) {
                    state = STATE.IDLE;
                    agent.ResetPath();
                    TurnOffTriggers();
                }

                break;
            case STATE.CHASE:
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 2f;
                TurnOffTriggers();
                agent.speed = runningSpeed;
                anim.SetBool("isRunning", true);
                
                if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    state = STATE.ATTACK;
                }

                if (ForgotPlayer())
                {
                    state = STATE.WANDER;
                    agent.ResetPath();
                }

                break;
            case STATE.ATTACK:
                TurnOffTriggers();
                anim.SetBool("isAttacking", true);
                this.transform.LookAt(target.transform.position);

                if (DistanceToPlayer() > agent.stoppingDistance + 2)
                    state = STATE.CHASE;

                break;
            case STATE.DEAD:
                Destroy(agent);
                this.GetComponent<Sink>().StartSink();
                break;
        }
    }

    public void KillZombie()
    {
        TurnOffTriggers();
        anim.SetBool("isDead", true);
        state = STATE.DEAD;
    }

    bool CanSeePlayer()
    {
        return DistanceToPlayer() < 10;
    }

    bool ForgotPlayer()
    {
        return DistanceToPlayer() > 20;
    }

    float DistanceToPlayer()
    {
        return Vector3.Distance(target.transform.position, this.transform.position);
    }
    
    void TurnOffTriggers()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isDead", false);
    }
}
