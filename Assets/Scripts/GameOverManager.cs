using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Tooltip("Assign the square GameObject (with SpriteRenderer) to show on Game Over")]
    public GameObject gameOverSprite;

    [Tooltip("Camera position to use when showing Game Over (optional)")]
    public Vector3 gameOverCameraPosition = new Vector3(0f, 0f, -10f);

    void Awake()
    {
        // Mise en place du singleton GameOverManager (un seul actif)
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (gameOverSprite != null) gameOverSprite.SetActive(false); // cacher l'overlay de Game Over au démarrage
    }

    // Called when the player dies (touch the moving wall)
    public void TriggerGameOver(GameObject player)
    {
        // Fonction centrale appelée quand le joueur perd (mur, piège, etc.)
        // Stop player and hide its visuals
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Remplacer linearVelocity par velocity (API publique Unity)
                rb.linearVelocity = Vector2.zero;
                rb.isKinematic = true;
            }

            // On masque le rendu et on désactive l'animator pour figer visuellement le joueur
            foreach (var r in player.GetComponentsInChildren<SpriteRenderer>(true)) r.enabled = false;
            foreach (var a in player.GetComponentsInChildren<Animator>(true)) a.enabled = false;

            // On désactive le contrôle joueur et sa collision
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false;

            var col = player.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        // Activate sprite (if assigned) and bring camera to the desired position
        if (gameOverSprite != null)
        {
            // Si un sprite d'overlay est assigné, on l'affiche au centre de la caméra
            var cam = Camera.main;
            if (cam != null)
            {
                gameOverSprite.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
            }

            var sr = gameOverSprite.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 32767;

            gameOverSprite.SetActive(true);
        }

        if (Camera.main != null)
        {
            // Repositionne la caméra (optionnel) pour mettre en avant l'écran de défaite
            Camera.main.transform.position = gameOverCameraPosition;
        }
    }
}

