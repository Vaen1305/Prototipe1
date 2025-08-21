using UnityEngine;
using System.Linq;

public class GunPlayer : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    public int bulletDamage = 25;
    public string enemyTag = "Drone";

    [Header("Keybindings")]
    public KeyCode fireKey = KeyCode.Space;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetKey(fireKey) && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Transform target = FindClosestEnemy();
        if (target == null)
        {
            Debug.LogWarning("No hay drones para disparar.");
            return;
        }

        nextFireTime = Time.time + fireRate;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = bulletDamage;
            bulletScript.SetTarget(target);
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
        return bestTarget;
    }
}