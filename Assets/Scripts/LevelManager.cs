using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI bulletMagazine;
    [SerializeField]
    private TextMeshProUGUI totalBullets;
    [SerializeField]
    private Volume volume;
    private Vignette vignette;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume.profile.TryGet<Vignette>(out vignette);
        UpdateBullets();
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

    public void UpdateLife()
    {
        float percentage = 1 - GameManager.instance.GetGameData.CurrentLife/GameManager.instance.GetGameData.MaxLife;

        vignette.intensity.value = percentage;
    }
}
