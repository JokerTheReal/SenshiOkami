using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{

    //Distance entre l'ennemi et le joueur
    private float Distance;

    public Transform target;
    NavMeshAgent agent;

    //Distance de poursuite
    public float chaseRange = 10;

    //porté des attaques 
    public float attackRange = 1f;

    //Cooldown des attaques 
    public float attackRepeatTime = 1;
    private float attackTime;

    //Montants des dégats infligés
    public float TheDamage;

    //Animation de l'ennemi
    public Animation animations;

    public float enemyHealth;
    private bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animations = gameObject.GetComponent<Animation>();
        attackTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {

            target = GameObject.Find("Player").transform;

            Distance = Vector3.Distance(target.position, transform.position);

            //Quand l'ennemi est loin 
            if (Distance > chaseRange)
            {
                Idle();
            }

            //Quand l'ennemi est assez proche mais pas pour attaquer
            if (Distance < chaseRange && Distance > attackRange)
            {
                chase();
            }

            //Quand l'ennemi est assez proche pour attaquer
            if (Distance < attackRange)
            {
                attack();
            }
        }
        //agent.SetDestination(target.position);
    }

    //Poursuite
    void chase()
    {
        animations.Play("NWalk");
        agent.destination = target.position;
    }

    //Comnbat
    void attack()
    {
        //agent.destination = transform.position;
        //agent.SetDestination(target.position);

        //Si pas de cooldown
        if (Time.time > attackTime)
        {
            animations.Play("NAttack2");
            //target.GetComponent<PlayerInventory>().ApplyDamage(TheDamage);
            Debug.Log("L'ennemi a envoyé" + TheDamage + " points de dégâts");
            attackTime = Time.time + attackRepeatTime;
            ApplyDamage(TheDamage);
        }
    }

    void Idle()
    {
        animations.Play("NIdle");
    }

    public void ApplyDamage(float TheDamage)
    {
        if (!isDead)
        {
            enemyHealth = enemyHealth - TheDamage;
            print(gameObject.name + "a subit" + TheDamage*2 + "points de dégâts");

            if(enemyHealth <= 0)
            {
                Dead();
            }
        }
    }

    public void Dead()
    {
        isDead = true;
        animations.Play("NDie");
    }

}
