using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum EstadoPartida
    {
        EnCurso,
        Victoria,
        Derrota
    }

    [Header("Alerta")]
    [SerializeField, Min(0f)] private float aumentoAlertaPorSegundo = 25f;
    [SerializeField, Min(0f)] private float reduccionAlertaPorSegundo = 10f;
    [SerializeField] private Slider barraAlerta;
    [SerializeField] private DetectorSigilo2D[] detectores;

    [Header("Objetivos y resultado")]
    [SerializeField] private GameObject objetivoEscapar;
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private KaitoController jugador;

    public static GameManager Instance { get; private set; }
    public static bool GemaRobada { get; private set; }
    public float Alerta { get; private set; }
    public bool JugadorDetectado { get; private set; }
    public EstadoPartida EstadoActual { get; private set; } = EstadoPartida.EnCurso;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Solo puede haber un GameManager activo en la escena.", this);
            enabled = false;
            return;
        }

        Instance = this;
        GemaRobada = false;
        Alerta = 0f;
        EstadoActual = EstadoPartida.EnCurso;

        if (jugador == null)
        {
            jugador = FindObjectOfType<KaitoController>();
        }

        if (detectores == null || detectores.Length == 0)
        {
            detectores = FindObjectsOfType<DetectorSigilo2D>();
        }

        if (barraAlerta != null)
        {
            barraAlerta.minValue = 0f;
            barraAlerta.maxValue = 100f;
            barraAlerta.value = Alerta;
        }

        if (objetivoEscapar != null)
        {
            objetivoEscapar.SetActive(false);
        }

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(false);
        }

        if (panelDerrota != null)
        {
            panelDerrota.SetActive(false);
        }
    }

    private void Update()
    {
        if (EstadoActual != EstadoPartida.EnCurso)
        {
            return;
        }

        JugadorDetectado = false;
        foreach (DetectorSigilo2D detector in detectores)
        {
            if (detector != null && detector.JugadorDetectado)
            {
                JugadorDetectado = true;
                break;
            }
        }

        float cambioAlerta = JugadorDetectado
            ? aumentoAlertaPorSegundo
            : -reduccionAlertaPorSegundo;
        Alerta = Mathf.Clamp(Alerta + cambioAlerta * Time.deltaTime, 0f, 100f);

        if (barraAlerta != null)
        {
            barraAlerta.value = Alerta;
        }

        if (Alerta >= 100f && JugadorDetectado)
        {
            Finalizar(EstadoPartida.Derrota);
        }
    }

    public void RobarGema()
    {
        if (EstadoActual != EstadoPartida.EnCurso || GemaRobada)
        {
            return;
        }

        GemaRobada = true;
        if (objetivoEscapar != null)
        {
            objetivoEscapar.SetActive(true);
        }
    }

    public void IntentarEscapar(KaitoController quien)
    {
        if (quien == null || quien != jugador || !GemaRobada ||
            EstadoActual != EstadoPartida.EnCurso)
        {
            return;
        }

        Finalizar(EstadoPartida.Victoria);
    }

    public void RegistrarCaptura(KaitoController quien)
    {
        if (quien == jugador && EstadoActual == EstadoPartida.EnCurso)
        {
            Finalizar(EstadoPartida.Derrota);
        }
    }

    private void Finalizar(EstadoPartida resultado)
    {
        EstadoActual = resultado;

        if (jugador != null)
        {
            KaitoGadgets gadgets = jugador.GetComponent<KaitoGadgets>();
            if (gadgets != null)
            {
                gadgets.enabled = false;
            }

            jugador.enabled = false;
        }

        if (resultado == EstadoPartida.Victoria && panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
        else if (resultado == EstadoPartida.Derrota && panelDerrota != null)
        {
            panelDerrota.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
