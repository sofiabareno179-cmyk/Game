using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GemaObjetivo : MonoBehaviour
{
    private void Awake()
    {
        Collider2D colisionador = GetComponent<Collider2D>();
        if (!colisionador.isTrigger)
        {
            Debug.LogError("El Collider2D de la gema debe tener Is Trigger activado.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        KaitoController jugador = otro.GetComponentInParent<KaitoController>();
        if (jugador == null)
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("No se puede recoger la gema sin un GameManager activo.", this);
            return;
        }

        GameManager.Instance.RobarGema();
        if (GameManager.GemaRobada)
        {
            gameObject.SetActive(false);
        }
    }
}
