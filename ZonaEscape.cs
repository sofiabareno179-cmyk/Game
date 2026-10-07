using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZonaEscape : MonoBehaviour
{
    private void Awake()
    {
        Collider2D colisionador = GetComponent<Collider2D>();
        if (!colisionador.isTrigger)
        {
            Debug.LogError("El Collider2D de la zona de escape debe tener Is Trigger activado.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        KaitoController jugador = otro.GetComponentInParent<KaitoController>();
        if (jugador != null && GameManager.Instance != null)
        {
            GameManager.Instance.IntentarEscapar(jugador);
        }
    }
}
