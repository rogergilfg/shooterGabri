using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI bulletMagazine;
    [SerializeField]
    private TextMeshProUGUI totalBullets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBullets()
    {
        bulletMagazine.text = GameManager.instance.GetGameData.Weapons[GameManager.instance.GetGameData.WeaponIndex].MagazineBullets;
        totalBullets.text = GameManager.instance.GetGameData.Weapons[GameManager.instance.GetGameData.WeaponIndex].TotalBullets;
    }
}
