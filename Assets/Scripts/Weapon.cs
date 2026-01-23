using UnityEditor.XR;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Transform bulletSpawnPoint;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private int currentBullets;
    [SerializeField]
    private int maxMagazine;
    [SerializeField]
    private int totalBullets;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private bool automatic;
    private float timePass;
    private bool triggeredPress;
    private LevelManager lm;

    public UnityEvent reloadEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        timePass += Time.deltaTime;

        if (triggeredPress == true)
        {
            Shoot();
            if(automatic == false)
            {
                triggeredPress = false;
            }
        }
    }

    public void Shoot()
    {
        if (currentBullets > 0 & timePass>=fireRate)
        {
            // Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)); Desde un punto en pixeles de la pantalla
            Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)); // desde un porcentaje, donde 00 es la esqina inferior izquierda 
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 bulletDirection = (hit.point - bulletSpawnPoint.position).normalized;
                GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                bulletClone.GetComponent<Rigidbody>().linearVelocity = bulletDirection * bulletSpeed;
                //AudioManager.instance.PlaySFX( , bulletSpawnPoint.position );
            }
            else
            {
                GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                bulletClone.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
            }
            currentBullets--;
            timePass = 0;
            lm.UpdateBullets();
        }
    }

    public void EnemyShoot(Transform _player)
    {

        if (timePass >= fireRate)
        {
            Debug.Log("Shoot");

            if (currentBullets > 0)
            {
                Debug.Log("Te he dado");
                GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                Vector3 direction = (_player.position + new Vector3(0, 1.4f, 0) - bulletSpawnPoint.position).normalized;
                bulletClone.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed;
                currentBullets--;
                timePass = 0;
            }
            else
            {
                reloadEnemy.Invoke();
            }
        }
    }

    public void Triggered()
    {
        triggeredPress = true;
    }

    public void TriggerReleased()
    {
        triggeredPress = false;
    }

    public void Reload()
    {
        int bulletsToReload = maxMagazine - currentBullets;
        if (bulletsToReload < totalBullets)
        {
            currentBullets = maxMagazine;
            totalBullets -= bulletsToReload;
        }
        else
        {
            currentBullets += totalBullets;
            totalBullets = 0;
        }
    }

    public string MagazineBullets
    {
        get { return currentBullets.ToString() + "/" + maxMagazine.ToString(); }
    }

    public string TotalBullets
    {
        get { return totalBullets.ToString(); }
    }
}
