using System.Collections;
using UnityEngine;

public class ShrinkingWall : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 0.1f;
    public float expandSpeed = 0.2f;

    [Header("Sides")]
    public bool fromTop;
    public bool fromBottom;
    public bool fromLeft;
    public bool fromRight;

    [Header("Game Over")]
    public GameOverManager gameOverManager;

    [Header("Crush detection")]
    public float squeezeDistance = 0.5f;
    public LayerMask blockerMask = ~0;
    public float originOffset = 0.02f;
    public bool debugLogs = false;

    Vector3 moveDirection;

    void Start()
    {
        // Détermination de la direction initiale du mur selon le flag activé
        if (fromTop) moveDirection = Vector3.down;
        else if (fromBottom) moveDirection = Vector3.up;
        else if (fromLeft) moveDirection = Vector3.right;
        else if (fromRight) moveDirection = Vector3.left;
        else moveDirection = Vector3.zero;
    }

    void Update()
    {
        // Avance du mur dans sa direction (mouvement linéaire simple)
        transform.position += moveDirection * speed * Time.deltaTime;

        // Agrandissement progressif pour donner une impression de pression
        Vector3 s = transform.localScale;
        if (fromTop || fromBottom) s.y += expandSpeed * Time.deltaTime;
        else if (fromLeft || fromRight) s.x += expandSpeed * Time.deltaTime;
        transform.localScale = s;
    }

    void OnCollisionEnter2D(Collision2D c) => HandlePlayerHit(c.gameObject);
    void OnTriggerEnter2D(Collider2D c) => HandlePlayerHit(c.gameObject);

    void HandlePlayerHit(GameObject obj)
    {
        if (!obj || !obj.CompareTag("Player")) return;
        // On décale à la prochaine frame physique pour des bounds stables (écrasement)
        StartCoroutine(CheckCrush(obj));
    }

    IEnumerator CheckCrush(GameObject player)
    {
        yield return new WaitForFixedUpdate();
        if (!player) yield break;

        // Direction utilisée pour pousser / détecter l'écrasement
        Vector3 pd3 = moveDirection;
        if (pd3 == Vector3.zero)
        {
            if (fromLeft) pd3 = Vector3.right;
            else if (fromRight) pd3 = Vector3.left;
            else if (fromTop) pd3 = Vector3.down;
            else if (fromBottom) pd3 = Vector3.up;
        }

        Vector2 pushDir = new Vector2(pd3.x, pd3.y).normalized;
        if (pushDir.sqrMagnitude < 1e-6f) yield break;

        // player bounds -> base box size
        Collider2D pc = player.GetComponent<Collider2D>();
        Vector2 box = pc ? pc.bounds.size : new Vector2(0.5f, 0.5f);

        float half = Mathf.Max(box.x, box.y) * 0.5f;
        Vector2 origin = (Vector2)player.transform.position + pushDir * (half + originOffset);

        // sweep area center + size
        Vector2 center = origin + pushDir * (squeezeDistance * 0.5f);
        Vector2 extra = new Vector2(Mathf.Abs(pushDir.x) * squeezeDistance, Mathf.Abs(pushDir.y) * squeezeDistance);
        Vector2 size = box * 0.9f + extra;
        float angle = Mathf.Atan2(pushDir.y, pushDir.x) * Mathf.Rad2Deg;

        if (debugLogs) Debug.DrawLine(origin, origin + pushDir * squeezeDistance, Color.yellow, 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle, blockerMask);
        foreach (var col in hits)
        {
            if (!col) continue;
            if (col.isTrigger) continue;
            if (col.gameObject == player) continue;
            if (col.gameObject == gameObject) continue;

            // Si un autre collider "solide" se trouve exactement dans la zone d'écrasement => Game Over
            if (debugLogs) Debug.Log($"ShrinkingWall: blocker {col.gameObject.name}");
            if (gameOverManager != null) gameOverManager.TriggerGameOver(player);
            else if (GameOverManager.Instance != null) GameOverManager.Instance.TriggerGameOver(player);
            else if (debugLogs) Debug.LogWarning("ShrinkingWall: no GameOverManager");
            yield break;
        }

        // Sinon: pas encore écrasé (le joueur peut encore reculer)
        if (debugLogs) Debug.Log("ShrinkingWall: no blocker detected");
    }
}