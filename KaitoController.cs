using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KaitoController : MonoBehaviour
{
    public enum Estado
    {
        EnSuelo,
        Saltando,
        Planeando
    }

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 10f;
    [SerializeField] private float velocidadPlaneo = 8f;

    [Header("Deteccion del suelo")]
    [SerializeField] private Transform puntoSuelo;
    [SerializeField] private float radioSuelo = 0.15f;
    [SerializeField] private LayerMask capasSuelo;

    [Header("Ala delta")]
    [SerializeField, Range(0f, 1f)] private float multiplicadorGravedadPlaneo = 0.2f;

    private Rigidbody2D cuerpo;
    private float gravedadBase;
    private float entradaHorizontal;
    private bool saltoSolicitado;
    private bool saltoPresionado;
    private bool enSuelo;

    public Estado EstadoActual { get; private set; } = Estado.EnSuelo;
    public Vector2 DireccionMirada { get; private set; } = Vector2.right;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        gravedadBase = cuerpo.gravityScale;

        if (puntoSuelo == null)
        {
            Debug.LogError("Asigna el Punto Suelo en el Inspector.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // Lee el input aquí y aplica las fuerzas en FixedUpdate.
        entradaHorizontal = Input.GetAxisRaw("Horizontal");
        if (entradaHorizontal != 0f)
        {
            DireccionMirada = entradaHorizontal > 0f ? Vector2.right : Vector2.left;
        }

        saltoPresionado = Input.GetButton("Jump");

        if (Input.GetButtonDown("Jump"))
        {
            saltoSolicitado = true;
        }
    }

    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioSuelo, capasSuelo) != null;

        if (enSuelo && saltoSolicitado)
        {
            cuerpo.velocity = new Vector2(cuerpo.velocity.x, fuerzaSalto);
            enSuelo = false;
        }

        saltoSolicitado = false;

        // El ala delta solo se activa al caer y mientras se mantiene el salto.
        bool planeando = !enSuelo && cuerpo.velocity.y < 0f && saltoPresionado;
        cuerpo.gravityScale = planeando
            ? gravedadBase * multiplicadorGravedadPlaneo
            : gravedadBase;

        float velocidadActual = planeando ? velocidadPlaneo : velocidad;
        cuerpo.velocity = new Vector2(entradaHorizontal * velocidadActual, cuerpo.velocity.y);

        EstadoActual = enSuelo
            ? Estado.EnSuelo
            : planeando ? Estado.Planeando : Estado.Saltando;
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoSuelo == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(puntoSuelo.position, radioSuelo);
    }
}
