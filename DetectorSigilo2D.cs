using UnityEngine;

public class DetectorSigilo2D : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private KaitoController jugador;
    [SerializeField] private Transform puntoVision;
    [SerializeField, Min(0f)] private float distanciaVision = 8f;
    [SerializeField, Range(0f, 360f)] private float anguloVision = 90f;
    [SerializeField] private LayerMask capasObstaculo;

    private Vector2 direccionVision = Vector2.right;
    private bool visionActiva = true;

    public bool JugadorDetectado { get; private set; }

    private void Awake()
    {
        if (puntoVision == null)
        {
            puntoVision = transform;
        }

        if (jugador == null)
        {
            jugador = FindObjectOfType<KaitoController>();
        }
    }

    private void Update()
    {
        if (!visionActiva || jugador == null)
        {
            JugadorDetectado = false;
            return;
        }

        KaitoGadgets gadgets = jugador.GetComponent<KaitoGadgets>();
        if (gadgets != null && gadgets.EstaOculto)
        {
            JugadorDetectado = false;
            return;
        }

        Vector2 origen = puntoVision.position;
        Vector2 haciaJugador = (Vector2)jugador.transform.position - origen;
        float distancia = haciaJugador.magnitude;

        if (distancia > distanciaVision || distancia == 0f)
        {
            JugadorDetectado = false;
            return;
        }

        Vector2 direccionAlJugador = haciaJugador / distancia;
        float angulo = Vector2.Angle(direccionVision, direccionAlJugador);
        if (angulo > anguloVision * 0.5f)
        {
            JugadorDetectado = false;
            return;
        }

        RaycastHit2D impacto = Physics2D.Raycast(origen, direccionAlJugador, distancia, capasObstaculo);
        JugadorDetectado = impacto.collider == null;
    }

    public void EstablecerDireccion(Vector2 direccion)
    {
        if (direccion.sqrMagnitude > 0f)
        {
            direccionVision = direccion.normalized;
        }
    }

    public void EstablecerVisionActiva(bool activa)
    {
        visionActiva = activa;
        if (!activa)
        {
            JugadorDetectado = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform origen = puntoVision != null ? puntoVision : transform;
        Vector2 direccion = direccionVision.sqrMagnitude > 0f ? direccionVision : Vector2.right;
        float mitadAngulo = anguloVision * 0.5f;

        Gizmos.color = JugadorDetectado ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(origen.position, distanciaVision);
        Gizmos.DrawLine(origen.position, origen.position + (Vector3)(Quaternion.Euler(0f, 0f, mitadAngulo) * direccion * distanciaVision));
        Gizmos.DrawLine(origen.position, origen.position + (Vector3)(Quaternion.Euler(0f, 0f, -mitadAngulo) * direccion * distanciaVision));
    }
}
