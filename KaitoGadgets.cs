using UnityEngine;

public class KaitoGadgets : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private KaitoController controlador;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private CartaProyectil prefabCarta;
    [SerializeField] private GameObject prefabHumo;

    [Header("Pistola de cartas")]
    [SerializeField] private float tiempoRecargaCarta = 0.5f;

    [Header("Bomba de humo")]
    [SerializeField] private float duracionHumo = 3f;

    private float proximoDisparo;
    private float ocultoHasta;

    // Los enemigos pueden consultar esta propiedad para omitir la deteccion de Kaito.
    public bool EstaOculto => Time.time < ocultoHasta;

    private void Awake()
    {
        if (controlador == null)
        {
            controlador = GetComponent<KaitoController>();
        }

        if (controlador == null)
        {
            Debug.LogError("Asigna un KaitoController o adjunta ambos scripts al mismo objeto.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            DispararCarta();
        }

        if (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Fire2"))
        {
            LanzarBombaDeHumo();
        }
    }

    private void DispararCarta()
    {
        if (Time.time < proximoDisparo)
        {
            return;
        }

        if (prefabCarta == null || puntoDisparo == null)
        {
            Debug.LogError("Asigna el prefab de carta y el PuntoDisparo en el Inspector.", this);
            return;
        }

        CartaProyectil carta = Instantiate(prefabCarta, puntoDisparo.position, puntoDisparo.rotation);
        carta.Inicializar(controlador.DireccionMirada);
        proximoDisparo = Time.time + tiempoRecargaCarta;
    }

    private void LanzarBombaDeHumo()
    {
        if (prefabHumo == null)
        {
            Debug.LogError("Asigna el prefab del efecto de humo en el Inspector.", this);
            return;
        }

        GameObject efectoHumo = Instantiate(prefabHumo, transform.position, Quaternion.identity);
        ocultoHasta = Time.time + duracionHumo;
        Destroy(efectoHumo, duracionHumo);
    }
}
