using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // Nouveau Input System

[DisallowMultipleComponent]
public class StartMenu : MonoBehaviour
{
    [Tooltip("Index de la première scène de jeu dans Build Settings (souvent 1 si le menu est en 0)")]
    public int firstGameSceneIndex = 1;

    [Tooltip("Activer l'appui sur Entrée/Espace pour lancer le jeu")]
    public bool allowKeyboardStart = true;

    [Tooltip("Masquer le curseur au lancement du jeu")]
    public bool hideCursorOnStart = true;

    private bool _starting = false;

    // Appelé par le bouton UI (OnClick)
    public void OnPlayClicked()
    {
        // Appelé par le bouton Play (UI) pour démarrer la partie
        StartGame();
    }

    // Appelé par un bouton Quitter (facultatif)
    public void OnQuitClicked()
    {
#if UNITY_EDITOR
    // En éditeur, stoppe le Play Mode (Application.Quit ne fonctionne pas ici)
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Update()
    {
        if (!_starting && allowKeyboardStart)
        {
            var kb = Keyboard.current;
            if (kb != null)
            {
                // Entrée (Enter ou Numpad Enter) ou Espace
                if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
                {
                    // Démarrage via raccourci clavier
                    StartGame();
                }
                // Échap pour quitter (optionnel)
                if (kb.escapeKey.wasPressedThisFrame)
                {
                    OnQuitClicked();
                }
            }
        }
    }

    private void StartGame()
    {
        if (_starting) return;
        _starting = true;

        if (hideCursorOnStart)
        {
            Cursor.visible = false;
        }

        // Remettre le temps normal si jamais il avait été mis en pause
        if (Time.timeScale == 0f) Time.timeScale = 1f;

        // Réinitialiser l'état de victoire éventuel
        if (WinManager.Instance != null)
        {
            // Remise à zéro du compteur de portes pour une nouvelle session
            WinManager.Instance.ResetDoors();
        }

        // Charger la première scène de jeu
        if (firstGameSceneIndex < 0 || firstGameSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"StartMenu: firstGameSceneIndex={firstGameSceneIndex} invalide. Vérifiez les Build Settings.", this);
            _starting = false;
            return;
        }

        // Chargement effectif de la première scène de jeu
        SceneManager.LoadScene(firstGameSceneIndex);
    }
}
