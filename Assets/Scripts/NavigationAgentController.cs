using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationAgentController : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    float chaseRange = 5.0F;

    [SerializeField]
    float rotationSpeed = 2.5F;

    [SerializeField]
    Transform attackPoint;

    [SerializeField]
    float attackRange = 10F;

    [SerializeField]
    float attackDamage =  8.0F;
    [SerializeField]
    public Transform drone;
    [SerializeField]
    public float projectileSpeed;
    [SerializeField]
    public float projectileRate;
    [SerializeField]
    public GameObject projectile;
    [SerializeField]
    public GameObject bulletMuzzle;
    NavMeshAgent navAgent;
    public RaycastHit rayImpact;
    [SerializeField]
    float distanceToTarget;

    bool chaseTarget;
    bool wasChaseTarget;
    bool isAttacking;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
    
    }

    void Update()
    {
        distanceToTarget = 
            Vector3.Distance(target.position, transform.position);
        
        wasChaseTarget = chaseTarget;

        chaseTarget = 
            (distanceToTarget < chaseRange);
        

        if (chaseTarget)
        {
            attackPoint.LookAt(target);
            drone.LookAt(target);
            EngageTarget();
        }
        else 
        {
           StopChasingTarget();
        }
        
        if(distanceToTarget <= navAgent.stoppingDistance)
        {
            AttackTarget();
        }
        

    }

    void StopChasingTarget()
    {
        if (!wasChaseTarget)
        {
            ChaseTarget(transform.position);
        }

    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(attackPoint.position, attackPoint.forward * attackRange);
    }

    void EngageTarget()
    {
        
        FaceTarget();
        ChaseTarget(target.position);
    }

    void FaceTarget()
    {
        Vector3 direction = 
           -(target.position - transform.position).normalized;

        Quaternion lookRotation =
            Quaternion.LookRotation
                (new Vector3(direction.x, 0.0F, direction.z));
        
        transform.rotation = 
            Quaternion.Slerp 
               (transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    void AttackTarget()
    {
        Shoot(attackPoint, attackPoint.forward);
        InstanciateEffect();
        
    }

    void ChaseTarget(Vector3 position)
    {
        navAgent.SetDestination(position);
    }

    void OnAttackEnded()
    {
        isAttacking = false;
    }

        void Shoot(Transform origenDelDisparo, Vector3 direcionDelDisparo)
    {
        // Efecto de disparo mediante raycast
        if (Physics.Raycast(origenDelDisparo.position, direcionDelDisparo, out rayImpact, attackRange))
        {
            Debug.LogWarning(rayImpact.collider.name);
            if (rayImpact.collider.tag == "Player" || rayImpact.collider.tag == "Enemy")
            {
                rayImpact.collider.gameObject.GetComponent<HealthController>().Damage(attackDamage);
            }

        }
    }

    private void InstanciateEffect()
    {
        // Se instancia los graficos del efecto al disparar de la arma y el impacto de las balas en superficies
        GameObject clon = Instantiate(projectile, drone.position, attackPoint.rotation);
        clon.GetComponent<Rigidbody>().AddForce(attackPoint.forward * projectileSpeed);
        GameObject clon2 = Instantiate(bulletMuzzle, attackPoint.position, Quaternion.identity);
        Destroy(clon, 0.15F);
        Destroy(clon2, 0.3f);


        Invoke("Shoot", projectileRate);
    }

}
