using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{

    //================================================================================
    /* Clase para
    */
    //================================================================================
    // Variables
    Transform player;
    float distance;
    public float turretRange;
    public float projectileSpeed;
    public Transform turretHead, barrel;
    public GameObject projectile;
    public Transform turret;
    public float fireRate, nextShot;
    public RaycastHit rayImpact;
    public Transform attackPoint;
    public int damage;
    HealthController health;
    public GameObject bulletMuzzle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = GetComponent<HealthController>();
    }

    void Update()
    {
        distance = Vector3.Distance(player.position, transform.position);
        if (distance <= turretRange)
        {

            attackPoint.LookAt(player);
            turretHead.LookAt(player);
            if (Time.time >= nextShot)
            {
                nextShot = Time.time + 1f / fireRate;
                ShootFromTurret();
            }
        }
        /**else
        {
            TurretController[] scripts = gameObject.GetComponents<TurretController>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = false;
            }
            turret.transform.Rotate(38, 0, 0);
            // ANIM_Torreta.SetBool(ANIM_Morir, true);
        }**/
    }

    public void ShootFromTurret()
    {
        Shoot(attackPoint, attackPoint.forward);
        InstanciateEffect();

    }

    void Shoot(Transform origenDelDisparo, Vector3 direcionDelDisparo)
    {
        // Efecto de disparo mediante raycast
        if (Physics.Raycast(origenDelDisparo.position, direcionDelDisparo, out rayImpact, turretRange))
        {
            Debug.LogWarning(rayImpact.collider.name);
            if (rayImpact.collider.tag == "Player")
            {
                rayImpact.collider.gameObject.GetComponent<HealthController>().Damage(damage);
            }

        }
    }

    private void InstanciateEffect()
    {
        // Se instancia los graficos del efecto al disparar de la arma y el impacto de las balas en superficies
        GameObject clon = Instantiate(projectile, barrel.position, turretHead.rotation);
        clon.GetComponent<Rigidbody>().AddForce(turretHead.forward * projectileSpeed);
        GameObject clon2 = Instantiate(bulletMuzzle, attackPoint.position, Quaternion.identity);
        Destroy(clon, 0.5F);
        Destroy(clon2, 0.5f);


        Invoke("Shoot", projectileSpeed);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(attackPoint.position, attackPoint.forward * turretRange);
    }


}
