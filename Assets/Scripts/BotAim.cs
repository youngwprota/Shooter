using System.Collections;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;

public class BotAim : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRadius;
    [SerializeField] private SphereCollider detectionArea;
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private ParticleSystem muzzleFlash;    
    [SerializeField] private AudioClip shootSound;     
    [SerializeField] private Light shootLight;  
    [SerializeField] private Character characterScript;  



    private bool canShoot = true;

    void Awake()
    {
        shootLight.enabled = false; 
    }
    void Start()
    {
        detectionArea.isTrigger = true;
        detectionArea.radius = detectionRadius;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 directionToTarget = other.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation * Quaternion.Euler(-90, 0, 0), Time.deltaTime * 2f);

            if (canShoot)
            {
                StartCoroutine(Shoot());
                characterScript.hp -= 2;
            }
        }
    }

    public IEnumerator Shoot()
    {
        canShoot = false;

        muzzleFlash.Play();
        shootLight.enabled = true;

        if (shootSound != null && shootAudioSource != null)
        {
            shootAudioSource.PlayOneShot(shootSound);
        }

        Debug.Log("Выстрел!");

        yield return new WaitForSeconds(0.2f); 
        shootLight.enabled = false;

        yield return new WaitForSeconds(0.3f);
        canShoot = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.up * -detectionRadius / 100;
        Gizmos.DrawRay(transform.position, direction);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
