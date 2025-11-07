using UnityEngine;

[DisallowMultipleComponent]
public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Identifiant unique de ce point d'apparition. Doit correspondre à l'ID envoyé par la porte précédente.")]
    private string spawnId;

    [SerializeField]
    [Tooltip("Conserver la composante Z actuelle du joueur en 2D")]
    private bool preserveZ = true;

    private void Start()
    {
        // Si un point d'apparition est demandé et que l'ID correspond, placer le joueur ici
        if (!string.IsNullOrEmpty(SceneSpawnManager.NextSpawnId) && SceneSpawnManager.NextSpawnId == spawnId)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var rb = player.GetComponent<Rigidbody2D>();
                Vector3 target = transform.position;
                if (preserveZ)
                {
                    target.z = player.transform.position.z;
                }

                if (rb != null)
                {
                    // Déplacement physique du joueur directement à la position du spawn (2D)
                    rb.position = new Vector2(target.x, target.y);
                }
                else
                {
                    // Fallback si pas de Rigidbody2D
                    player.transform.position = target;
                }
            }

            // Nettoyer l'ID pour éviter de respawn à nouveau
            SceneSpawnManager.Clear();
        }
    }
}
