using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Horda
{
    [Tooltip("Número de enemigos en esta horda")]
    public int cantidadEnemigos = 5;

    [Tooltip("Tiempo de espera antes de que empiece esta horda (segundos)")]
    public float tiempoEsperaAntes = 3f;

    [Tooltip("Intervalo entre cada spawn de enemigo")]
    public float intervaloSpawn = 0.5f;
}

public class SpawnManager : MonoBehaviour
{
    [Header("Prefab del Enemigo")]
    [Tooltip("Arrastra aquí el prefab de tu enemigo")]
    [SerializeField] private GameObject prefabEnemigo;

    [Header("Puntos de Spawn")]
    [Tooltip("Lista de SpawnPoints donde pueden aparecer los enemigos")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Configuración de Hordas")]
    [SerializeField] private List<Horda> hordas = new List<Horda>();

    [Tooltip("¿Las hordas se repiten infinitamente?")]
    [SerializeField] private bool hordas_infinitas = false;

    [Tooltip("Si hordas infinitas, cuántos enemigos extra por horda adicional")]
    [SerializeField] private int incrementoEnemigosExtra = 2;

    [Header("UI - Canvas")]
    [SerializeField] private TextMeshProUGUI textoHordaActual;
    [SerializeField] private TextMeshProUGUI textoTotalHordas;
    [SerializeField] private TextMeshProUGUI textoEstado;
    [SerializeField] private TextMeshProUGUI textoEnemigosVivos;

    [Header("Colores UI")]
    [SerializeField] private Color colorPreparando = Color.yellow;
    [SerializeField] private Color colorEnCombate = Color.red;
    [SerializeField] private Color colorVictoria = Color.green;

    // Estado interno
    private int hordaActualIndex = 0;
    private int hordaInfinitaContador = 0;
    private List<GameObject> enemigosActivos = new List<GameObject>();
    private bool juegoTerminado = false;
    private bool hordaEnProgreso = false;

    // Propiedades públicas
    public int HordaActual => hordaActualIndex + 1;
    public int TotalHordas => hordas.Count;
    public int EnemigosVivos => enemigosActivos.Count;

    void Start()
    {
        ValidarConfiguracion();
        ActualizarUI();
        IniciarSiguienteHorda();
    }

    void Update()
    {
        // Limpiar referencias de enemigos destruidos/desactivados
        enemigosActivos.RemoveAll(e => e == null || !e.activeInHierarchy);
        ActualizarContadorEnemigos();

        // Detectar si la horda terminó
        if (hordaEnProgreso && enemigosActivos.Count == 0)
        {
            hordaEnProgreso = false;
            OnHordaCompletada();
        }
    }

    // ============================================================
    //  FLUJO DE HORDAS
    // ============================================================

    void IniciarSiguienteHorda()
    {
        if (juegoTerminado) return;

        if (hordaActualIndex >= hordas.Count)
        {
            if (hordas_infinitas)
            {
                hordaInfinitaContador++;
                Horda hordaExtra = new Horda
                {
                    cantidadEnemigos = hordas[hordas.Count - 1].cantidadEnemigos + (hordaInfinitaContador * incrementoEnemigosExtra),
                    tiempoEsperaAntes = 3f,
                    intervaloSpawn = 0.4f
                };
                hordas.Add(hordaExtra);
            }
            else
            {
                FinDelJuego();
                return;
            }
        }

        ActualizarUI();
        Horda hordaActual = hordas[hordaActualIndex];
        StartCoroutine(CorrutinaSpawnHorda(hordaActual));
    }

    IEnumerator CorrutinaSpawnHorda(Horda horda)
    {
        // Fase de preparación
        SetTextoEstado($"¡HORDA {HordaActual} PRÓXIMAMENTE!", colorPreparando);

        float cuenta = horda.tiempoEsperaAntes;
        while (cuenta > 0)
        {
            SetTextoEstado($"¡HORDA {HordaActual} EN {Mathf.CeilToInt(cuenta)}!", colorPreparando);
            yield return new WaitForSeconds(1f);
            cuenta -= 1f;
        }

        SetTextoEstado($"¡HORDA {HordaActual} INICIADA!", colorEnCombate);
        hordaEnProgreso = true;

        // Spawnear enemigos uno a uno
        for (int i = 0; i < horda.cantidadEnemigos; i++)
        {
            SpawnEnemigo();
            yield return new WaitForSeconds(horda.intervaloSpawn);
        }
    }

    void OnHordaCompletada()
    {
        Debug.Log($"✅ Horda {HordaActual} completada.");
        SetTextoEstado($"¡HORDA {HordaActual} COMPLETADA!", colorVictoria);
        hordaActualIndex++;
        Invoke(nameof(IniciarSiguienteHorda), 1.5f);
    }

    void FinDelJuego()
    {
        juegoTerminado = true;
        SetTextoEstado("¡TODAS LAS HORDAS COMPLETADAS!", colorVictoria);
        Debug.Log("🏆 ¡Todas las hordas completadas!");
    }

    // ============================================================
    //  SPAWN
    // ============================================================

    void SpawnEnemigo()
    {
        if (prefabEnemigo == null)
        {
            Debug.LogError("❌ No hay prefab de enemigo asignado en SpawnManager.");
            return;
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("❌ No hay SpawnPoints asignados en SpawnManager.");
            return;
        }

        Transform spawnElegido = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject nuevoEnemigo = Instantiate(prefabEnemigo, spawnElegido.position, spawnElegido.rotation);
        enemigosActivos.Add(nuevoEnemigo);

        // 👇 Suscribirse al evento OnDeath del enemigo
        EnemyMultiplayerController hp = nuevoEnemigo.GetComponent<EnemyMultiplayerController>();
        if (hp != null)
        {
            hp.OnDeath += OnEnemigoMuerto;
        }
        else
        {
            Debug.LogWarning($"⚠️ El prefab '{prefabEnemigo.name}' no tiene componente EnemyMultiplayerController.");
        }
    }

    // 👇 Se llama automáticamente cuando el enemigo dispara OnDeath
    void OnEnemigoMuerto(GameObject enemigo)
    {
        enemigosActivos.Remove(enemigo);
        Debug.Log($"💀 Enemigo eliminado. Quedan: {enemigosActivos.Count}");
        ActualizarContadorEnemigos();
    }

    // ============================================================
    //  UI
    // ============================================================

    void ActualizarUI()
    {
        if (textoHordaActual != null)
            textoHordaActual.text = $"HORDA: {HordaActual}";

        if (textoTotalHordas != null)
            textoTotalHordas.text = hordas_infinitas ? $"/ ∞" : $"/ {TotalHordas}";
    }

    void ActualizarContadorEnemigos()
    {
        if (textoEnemigosVivos != null)
            textoEnemigosVivos.text = $"Enemigos: {enemigosActivos.Count}";
    }

    void SetTextoEstado(string mensaje, Color color)
    {
        if (textoEstado != null)
        {
            textoEstado.text = mensaje;
            textoEstado.color = color;
        }
        Debug.Log($"[SpawnManager] {mensaje}");
    }

    // ============================================================
    //  VALIDACIÓN
    // ============================================================

    void ValidarConfiguracion()
    {
        if (hordas.Count == 0)
        {
            Debug.LogWarning("⚠️ No hay hordas configuradas. Agregando una horda por defecto.");
            hordas.Add(new Horda { cantidadEnemigos = 5, tiempoEsperaAntes = 3f, intervaloSpawn = 0.5f });
        }

        if (spawnPoints.Count == 0)
            Debug.LogError("❌ No hay SpawnPoints asignados. Agrégalos en el Inspector.");

        if (prefabEnemigo == null)
            Debug.LogError("❌ No hay prefab de enemigo asignado.");
    }

    // ============================================================
    //  GIZMOS
    // ============================================================

    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.red;
        foreach (Transform sp in spawnPoints)
        {
            if (sp == null) continue;
            Gizmos.DrawWireSphere(sp.position, 0.5f);
            Gizmos.DrawLine(sp.position, sp.position + sp.forward * 1.5f);
        }
    }

    // ============================================================
    //  API PÚBLICA
    // ============================================================

    public void PausarSpawn() => StopAllCoroutines();

    public void ReiniciarJuego()
    {
        StopAllCoroutines();
        foreach (var e in enemigosActivos)
            if (e != null) Destroy(e);

        enemigosActivos.Clear();
        hordaActualIndex = 0;
        hordaInfinitaContador = 0;
        juegoTerminado = false;
        hordaEnProgreso = false;
        ActualizarUI();
        IniciarSiguienteHorda();
    }
}
