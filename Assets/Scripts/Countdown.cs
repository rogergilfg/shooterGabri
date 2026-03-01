using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Configuración del Temporizador")]
    [SerializeField] private float tiempoTotal = 600f; // 10 minutos en segundos

    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoTiempo;

    [Header("Colores de alerta")]
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorAdvertencia = Color.yellow; // Menos de 1 min
    [SerializeField] private Color colorPeligro = Color.red;        // Menos de 30 seg

    private float tiempoRestante;
    private bool corriendo = true;

    void Start()
    {
        tiempoRestante = tiempoTotal;
        ActualizarTexto();
    }

    void Update()
    {
        if (!corriendo) return;

        if (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            tiempoRestante = Mathf.Max(tiempoRestante, 0f);
            ActualizarTexto();
            ActualizarColor();
        }
        else
        {
            corriendo = false;
            AlTerminar();
        }
    }

    void ActualizarTexto()
    {
        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
        textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    void ActualizarColor()
    {
        if (tiempoRestante <= 30f)
            textoTiempo.color = colorPeligro;
        else if (tiempoRestante <= 60f)
            textoTiempo.color = colorAdvertencia;
        else
            textoTiempo.color = colorNormal;
    }

    void AlTerminar()
    {
        textoTiempo.text = "00:00";
        textoTiempo.color = colorPeligro;
        Debug.Log("¡Tiempo terminado!");
        // Aquí puedes agregar tu lógica: mostrar pantalla de fin, detener juego, etc.
    }

    // === Métodos públicos para controlar el temporizador desde otros scripts ===

    public void Pausar() => corriendo = false;

    public void Reanudar() => corriendo = true;

    public void Reiniciar()
    {
        tiempoRestante = tiempoTotal;
        corriendo = true;
        ActualizarTexto();
        ActualizarColor();
    }

    public void SetTiempo(float nuevoTiempoEnSegundos)
    {
        tiempoTotal = nuevoTiempoEnSegundos;
        Reiniciar();
    }

    public float GetTiempoRestante() => tiempoRestante;
    public bool EstaCorreindo() => corriendo;
}
