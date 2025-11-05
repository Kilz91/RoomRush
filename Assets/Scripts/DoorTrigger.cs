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

        Debug.Log($"DoorTrigger: Player entered trigger on '{gameObject.name}'", this);

        var player = other.gameObject;
        if (WinManager.Instance != null)
        {
            WinManager.Instance.DoorPassed(player);
        }
        else
        {
            Debug.LogWarning("DoorTrigger: WinManager.Instance is null", this);
        }

        if (singleUse)
        {
            used = true;
            Debug.Log($"DoorTrigger: marked '{gameObject.name}' used (singleUse)", this);
        }
    }
}
