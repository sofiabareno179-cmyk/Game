using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ConanPerseguidor : MonoBehaviour
{
    [Header("Persecucion")]
    [SerializeField, Min(0f)] private float velocidadPersecucion = 4f;
    [SerializeField] private KaitoController jugador;

    private Rigidbody2D cuerpo;
    private bool activo;

    public bool Activo => activo;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;
        cuerpo.gravityScale = 0f;
        cuerpo.freezeRotation = true;

        Collider2D colisionador = GetComponent<Collider2D>();
        colisionador.isTrigger = true;

        if (jugador == null)
        {
            jugador = FindObjectOfType<KaitoController>();
        }
    }

    private void Update()
    {
        GameManager manager = GameManager.Instance;
        if (!activo && manager != null &&
            (GameManager.GemaRobada || manager.JugadorDetectado || manager.Alerta >= 100f))
        {
            activo = true;
        }
    }

    private void FixedUpdate()
    {
        if (!activo || jugador == null ||
            (GameManager.Instance != null &&
             GameManager.Instance.EstadoActual != GameManager.EstadoPartida.EnCurso))
        {
            return;
        }

        Vector2 nuevaPosicion = Vector2.MoveTowards(
            cuerpo.position,
            jugador.transform.position,
            velocidadPersecucion * Time.fixedDeltaTime);
        cuerpo.MovePosition(nuevaPosicion);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        RegistrarContacto(otro);
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        RegistrarContacto(colision.collider);
    }

    private void RegistrarContacto(Collider2D otro)
    {
        if (!activo)
        {
            return;
        }

        KaitoController jugadorTocado = otro.GetComponentInParent<KaitoController>();
        if (jugadorTocado != null && GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarCaptura(jugadorTocado);
        }
    }
}
