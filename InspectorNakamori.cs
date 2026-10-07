using UnityEngine;

[RequireComponent(typeof(DetectorSigilo2D))]
public class InspectorNakamori : MonoBehaviour
{
    [Header("Patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField, Min(0f)] private float velocidadPatrulla = 2f;
    [SerializeField, Min(0f)] private float toleranciaPunto = 0.05f;

    private DetectorSigilo2D detector;
    private Transform destinoActual;
    private float aturdidoHasta;

    private void Awake()
    {
        detector = GetComponent<DetectorSigilo2D>();

        if (puntoA == null || puntoB == null)
        {
            Debug.LogError("Asigna los puntos A y B de patrulla en el Inspector.", this);
            enabled = false;
            return;
        }

        destinoActual = puntoB;
    }

    private void Update()
    {
        if (Time.time < aturdidoHasta)
        {
            return;
        }

        detector.EstablecerVisionActiva(true);

        Vector2 desplazamiento = (Vector2)destinoActual.position - (Vector2)transform.position;
        if (desplazamiento.sqrMagnitude > 0f)
        {
            detector.EstablecerDireccion(new Vector2(Mathf.Sign(desplazamiento.x), 0f));
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            destinoActual.position,
            velocidadPatrulla * Time.deltaTime);

        if (Vector2.Distance(transform.position, destinoActual.position) <= toleranciaPunto)
        {
            destinoActual = destinoActual == puntoA ? puntoB : puntoA;
        }
    }

    public void Aturdir(float duracion)
    {
        if (duracion <= 0f)
        {
            return;
        }

        aturdidoHasta = Mathf.Max(aturdidoHasta, Time.time + duracion);
        detector.EstablecerVisionActiva(false);
    }
}
