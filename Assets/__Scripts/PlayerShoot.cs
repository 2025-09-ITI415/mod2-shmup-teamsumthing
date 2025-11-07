using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;   // Drag your bullet prefab here
    public Transform firePoint;       // Drag your FirePoint here
    public float fireRate = 0.1f;     // Smaller = faster bullets
    private float nextFireTime = 0f;

    void Update()
    {
        // If spacebar is pressed or held down
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
