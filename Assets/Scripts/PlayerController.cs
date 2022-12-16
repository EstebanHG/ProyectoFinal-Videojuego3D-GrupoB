using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public int damage;
    [SerializeField]
    public float shootingSpeed;
    [SerializeField]
    public float range; 
    [SerializeField]
    public float reloadSpeed;
    [SerializeField]
    public float shootQuantity;
    [SerializeField]
    public int maxMunition;
    [SerializeField]
    public int munition;
    [SerializeField]
    int bulletsShot = 1;
    [SerializeField]
    public int clipSize;
    bool shooting;
    bool readyToShoot;
    public bool reloading;
    [SerializeField]
    public GameObject fpsCam;
    [SerializeField]
    public GameObject mainCam;
    [SerializeField]
    public GameObject crosshair;
    [SerializeField]
    public Transform AttackPoint;
    [SerializeField]
    public Transform bulletExit;
    [SerializeField]
    public RaycastHit rayImpact;
    [SerializeField]
    public GameObject bulletMuzzle;
    [SerializeField]
    public GameObject surfaceImpact;
    HealthController health;

    [SerializeField]
    float normalSens = 1.0f;

    [SerializeField]
    float aimSens = 0.5f;

    [SerializeField]
    LayerMask aimMask;
    [SerializeField]
    float crosshairRotationSpeed;
    [SerializeField]
    float aimInterpolation;
    bool isAiming;
    bool aiming = false;
    ThirdPersonController thirdPersonController;

    [SerializeField]
   TextMeshProUGUI munitionText;
    [SerializeField]
   TextMeshProUGUI totalAmmoText;



    private void Attack()
    {

        shooting = Input.GetKey(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && munition >= 0 && munition < clipSize && (int)shootQuantity > 0 && !reloading)
        {
            Reload();
        }

        if (Input.GetKeyDown(KeyCode.R) && munition >= 0 && munition < clipSize && (int)shootQuantity > 0 && !reloading)
        {

            Reload();

        }

        // Se dispara mientras se cumplen distintas condiciones
        if (readyToShoot && shooting && !reloading && munition > 0 && aiming == true)
        {
            ShootFromCam();
        }
    }

    private void Shoot(Transform shootOrigin, Vector3 shootDirection)
    {

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Transform hitObject = null;

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 1000.0F, aimMask))
        {
            mouseWorldPosition = raycastHit.point;
            hitObject = raycastHit.transform;
            if (raycastHit.collider.tag == "Enemy")
            {
                raycastHit.collider.gameObject.GetComponent<HealthController>().Damage(damage);
            }
        }

        // Efecto de disparo mediante raycast

        /**if (Physics.Raycast(shootOrigin.position, shootDirection, out rayImpact, range))
        {
            Debug.LogWarning(rayImpact.collider.name);

            if (rayImpact.collider.tag == "Enemy")
            {
                rayImpact.collider.gameObject.GetComponent<HealthController>().Damage(damage);
            }
        }**/
    }

    private void ShootFromCam()
    {
        readyToShoot = false;
        Vector3 direction = fpsCam.transform.forward;
        Shoot(fpsCam.transform, direction);
        InstanciateEffects();

    }
    private void InstanciateEffects()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        
        Vector3 projectileDirection = (mouseWorldPosition - bulletExit.transform.position).normalized;
        GameObject clone1 = Instantiate(surfaceImpact, rayImpact.point, Quaternion.Euler(0, 180, 0));
        Destroy(clone1, 0.5f);

        GameObject clone2 = Instantiate(bulletMuzzle, bulletExit.position, AttackPoint.rotation);
        clone2.GetComponent<Rigidbody>().AddForce(AttackPoint.forward * 4500);
        Destroy(clone2, 0.3f);

        munition--;
        bulletsShot--;

        Invoke("repositioningShot", shootingSpeed);

        if (bulletsShot > 0 && munition > 0)
            Invoke("Shoot", shootingSpeed);
    }
    private void repositioningShot()
    {
        readyToShoot = true;
    }
    public void Reload()
    {
        reloading = true;
        Invoke("finishReloading", reloadSpeed);
    }


    private void finishReloading()
    {

        if (shootQuantity >= clipSize)
        {
            shootQuantity -= Mathf.Clamp(clipSize - munition, 0, maxMunition);
            munition = clipSize;

        }
        else
        {
            munition = (int)shootQuantity;
            shootQuantity = 0;
        }


        reloading = false;

    }
    private void Awake()
    {
        readyToShoot = true;
        munition = (int)clipSize;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(AttackPoint.position, AttackPoint.forward * range);
        thirdPersonController = GetComponent<ThirdPersonController>();

    }

    private void Update()
    {
        bool isAiming = Input.GetKey(KeyCode.Mouse1);
       
        if(isAiming == true && !fpsCam.activeInHierarchy)
        {
            mainCam.SetActive(false);
            fpsCam.SetActive(true);
            aiming = true;

            StartCoroutine(ShowReticle());
        }
        else if(isAiming == false && !mainCam.activeInHierarchy)
        {
            mainCam.SetActive(true);
            fpsCam.SetActive(false);
            crosshair.SetActive(false);
            aiming = false;

        }

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Transform hitObject = null;

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 1000.0F, aimMask))
        {
            mouseWorldPosition = raycastHit.point;
            hitObject = raycastHit.transform;
        }

        if(isAiming)
        {
            Vector3 aimTarget = mouseWorldPosition;
            aimTarget.y = transform.position.y;
            Vector3 aimDirection = (aimTarget  - transform.position).normalized;
            transform.forward = 
                Vector3.Lerp(transform.forward, aimDirection, crosshairRotationSpeed * Time.deltaTime);
            
        }

        Attack();
        munitionText.text = string.Concat(munition.ToString());
        totalAmmoText.text = string.Concat(shootQuantity.ToString());
        
        
    }
    void Start() 
    {
        health = GetComponent<HealthController>();
    }

    IEnumerator ShowReticle()
    {
       yield return new WaitForSeconds(0.25F);
       crosshair.SetActive(enabled);
    }
}