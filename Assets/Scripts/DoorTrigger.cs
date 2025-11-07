using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Tooltip("If true, the door will only count once when the player passes through it")]
    public bool singleUse = true;

    bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        // Le joueur traverse la zone de la porte: informer le WinManager pour incrémenter les portes franchies
        Debug.Log($"DoorTrigger: Player entered trigger on '{gameObject.name}'", this);

        var player = other.gameObject;
        if (WinManager.Instance != null)
        {
            // Incrémentation du compteur de progression (victoire quand threshold atteint)
            WinManager.Instance.DoorPassed(player);
        }
        else
        {
            Debug.LogWarning("DoorTrigger: WinManager.Instance is null", this);
        }

        if (singleUse)
        {
            // Empêche de compter plusieurs fois la même porte si 'singleUse' actif
            used = true;
            Debug.Log($"DoorTrigger: marked '{gameObject.name}' used (singleUse)", this);
        }
    }
}
