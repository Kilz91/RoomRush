using UnityEngine;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance { get; private set; }

    [Tooltip("Assign the square GameObject (with SpriteRenderer) to show on Win")]
    public GameObject winSprite;

    [Tooltip("Camera position to use when showing Win (optional)")]
    public Vector3 winCameraPosition = new Vector3(0f, 0f, -10f);

    [Tooltip("Number of doors the player must pass to win")]
    public int doorsToWin = 6;

    // Internal counter for doors passed in the current run
    private int doorsPassed = 0;

    void Awake()
    {
        // If another WinManager already exists, destroy this duplicate quietly.
        if (Instance != null && Instance != this)
        {
            // Do not spam the console; just remove the duplicate.
            Destroy(gameObject);
            return;
        }

        // This is the first/primary instance. Keep it alive across scenes.
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (winSprite != null) winSprite.SetActive(false);
        // store the initial camera position so we can restore it on restart if needed
        if (Camera.main != null) initialCameraPosition = Camera.main.transform.position;
        Debug.Log("WinManager Awake - primary Instance set (DontDestroyOnLoad)", this);
    }

    // Called by door scripts (or triggers) when the player successfully passes a door
    // If the player passes enough doors (doorsToWin) without losing, TriggerWin is called.
    public void DoorPassed(GameObject player)
    {
        // Do nothing if game over or already won
        if ((GameOverManager.Instance != null && GameOverManager.Instance.gameOverSprite != null && GameOverManager.Instance.gameOverSprite.activeSelf)
            || (winSprite != null && winSprite.activeSelf)) return;
        doorsPassed++;
        Debug.Log($"WinManager.DoorPassed called - doorsPassed={doorsPassed}/{doorsToWin}", player);
        if (doorsPassed >= doorsToWin)
        {
            Debug.Log("WinManager: doorsToWin reached, triggering Win", this);
            doorsPassed = doorsToWin; // lock the counter to prevent any further increments
            TriggerWin(player);
        }
    }

    // Called when the player wins (passes doorsToWin doors without losing)
    public void TriggerWin(GameObject player)
    {
        // Prevent showing win if game over is already active
        if (GameOverManager.Instance != null && GameOverManager.Instance.gameOverSprite != null && GameOverManager.Instance.gameOverSprite.activeSelf) return;

        Debug.Log("WinManager.TriggerWin called", this);

        // Stop player and hide its visuals similar to GameOver
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }

            foreach (var r in player.GetComponentsInChildren<SpriteRenderer>(true)) r.enabled = false;
            foreach (var a in player.GetComponentsInChildren<Animator>(true)) a.enabled = false;

            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false;

            var col = player.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        // Activate win sprite (if assigned) and bring camera to the desired position
        if (winSprite != null)
        {
            // Move camera first so the sprite can be positioned relative to view
            if (Camera.main != null)
            {
                Camera.main.transform.position = winCameraPosition;
            }

            var cam = Camera.main;
            if (cam != null)
            {
                winSprite.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
            }

            var sr = winSprite.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 32767;

            winSprite.SetActive(true);
        }
        else
        {
            // Fallback: create a simple UI overlay that displays "WIN"
            CreateFallbackWinUI();
            if (Camera.main != null)
            {
                Camera.main.transform.position = winCameraPosition;
            }
        }

        // pause the game so the Win is visible
        Time.timeScale = 0f;
    }

    // Simple IMGUI fallback (no Unity UI package required)
    private bool fallbackWinActive = false;
    private GUIStyle fallbackStyle;
    private bool showRestartPopup = false;
    private Vector3 initialCameraPosition;

    private void CreateFallbackWinUI()
    {
        fallbackWinActive = true;
        // style will be initialized on first OnGUI call
        fallbackStyle = null;
        // also show a simple restart popup
        showRestartPopup = true;
    }

    private void OnGUI()
    {
        // Draw WIN fallback (background)
        if (fallbackWinActive)
        {
            if (fallbackStyle == null)
            {
                fallbackStyle = new GUIStyle(GUI.skin.label);
                fallbackStyle.alignment = TextAnchor.MiddleCenter;
                // set font size relative to screen
                fallbackStyle.fontSize = Mathf.RoundToInt(Mathf.Min(Screen.width, Screen.height) / 6f);
                fallbackStyle.normal.textColor = Color.white;
            }

            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "WIN", fallbackStyle);
        }

        // simple restart popup
        if (showRestartPopup)
        {
            float w = 320f;
            float h = 120f;
            Rect rect = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
            GUI.Box(rect, "Partie terminée");
            GUI.Label(new Rect(rect.x + 12f, rect.y + 28f, rect.width - 24f, 40f), "Recommencer la partie ?");

            if (GUI.Button(new Rect(rect.x + 20f, rect.y + rect.height - 44f, 120f, 32f), "Oui"))
            {
                // unpause and reset
                Time.timeScale = 1f;
                ResetDoors();
                showRestartPopup = false;
            }

            if (GUI.Button(new Rect(rect.x + rect.width - 140f, rect.y + rect.height - 44f, 120f, 32f), "Non"))
            {
                // keep win visuals but close popup
                showRestartPopup = false;
            }
        }
    }

    // Optional: reset the internal door counter (call when restarting a run/level)
    public void ResetDoors()
    {
        doorsPassed = 0;
    }

    // Public check used by TeleporteController to know if teleport should be skipped
    public bool HasWon()
    {
        if (winSprite != null && winSprite.activeSelf) return true;
        if (fallbackWinActive) return true;
        return false;
    }
}
