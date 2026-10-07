using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CartaProyectil : MonoBehaviour
{
    [SerializeField] private float velocidad = 12f;
    [SerializeField] private float tiempoDeVida = 3f;
    [SerializeField, Min(0f)] private float duracionAturdimiento = 2f;
    [SerializeField] private LayerMask capasImpacto;

    private Rigidbody2D cuerpo;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        cuerpo.gravityScale = 0f;
        cuerpo.freezeRotation = true;

        Collider2D colisionador = GetComponent<Collider2D>();
        if (!colisionador.isTrigger)
        {
            Debug.LogError("El Collider2D de la carta debe tener Is Trigger activado.", this);
            enabled = false;
            return;
        }

        Destroy(gameObject, tiempoDeVida);
    }

    public void Inicializar(Vector2 direccion)
    {
        if (direccion.sqrMagnitude == 0f)
        {
            Debug.LogError("La direccion de la carta no puede ser cero.", this);
            enabled = false;
            return;
        }

        cuerpo.velocity = direccion.normalized * velocidad;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        int mascaraCapaImpactada = 1 << otro.gameObject.layer;
        if ((capasImpacto.value & mascaraCapaImpactada) != 0)
        {
            InspectorNakamori inspector = otro.GetComponentInParent<InspectorNakamori>();
            if (inspector != null)
            {
                inspector.Aturdir(duracionAturdimiento);
            }

            Destroy(gameObject);
        }
    }
}
