using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class TeleporteController : MonoBehaviour
{

    [Serializable]
    public struct CibleScene
    {
        [Tooltip("Nom de la scène à charger (dans Build Settings)")]
        public string nomScene;

        [Tooltip("ID du SpawnPoint dans la scène cible")]
        public string spawnId;

        [Tooltip("Poids pour le tirage aléatoire (>= 0). 0 = ignoré")]
        public float poids;
    }

    [Header("Scenes aléatoires uniquement")]
    [Tooltip("Liste des destinations possibles. Une sera choisie au hasard (pondéré par 'poids').")]
    [SerializeField] private CibleScene[] destinationsAleatoires;

    [SerializeField]
    [Tooltip("Exclure la scène courante lors du tirage aléatoire")]
    private bool exclureSceneCourante = true;

    private void Reset()
    {
        // S'assurer que le collider est configuré en "Is Trigger" pour une zone de téléportation 2D
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        ChargerSceneAleatoire();
    }

    private void ChargerSceneAleatoire()
    {
        if (destinationsAleatoires == null || destinationsAleatoires.Length == 0)
        {
            Debug.LogWarning($"{nameof(TeleporteController)} sur {name}: aucune destination aléatoire configurée.", this);
            return;
        }

        string sceneCourante = SceneManager.GetActiveScene().name;

        // Filtrer les cibles valides et accumuler le poids total
        float sommePoids = 0f;
        for (int i = 0; i < destinationsAleatoires.Length; i++)
        {
            var c = destinationsAleatoires[i];
            if (string.IsNullOrWhiteSpace(c.nomScene)) continue;
            if (exclureSceneCourante && c.nomScene == sceneCourante) continue;
            if (c.poids <= 0f) continue;
            sommePoids += c.poids;
        }

        if (sommePoids <= 0f)
        {
            Debug.LogWarning($"{nameof(TeleporteController)} sur {name}: aucune cible valide pour le tirage.", this);
            return;
        }

        // Tirage au sort pondéré
        float tirage = UnityEngine.Random.Range(0f, sommePoids);
        float cumul = 0f;
        CibleScene cibleChoisie = default;
        bool trouve = false;
        for (int i = 0; i < destinationsAleatoires.Length; i++)
        {
            var c = destinationsAleatoires[i];
            if (string.IsNullOrWhiteSpace(c.nomScene)) continue;
            if (exclureSceneCourante && c.nomScene == sceneCourante) continue;
            if (c.poids <= 0f) continue;
            cumul += c.poids;
            if (tirage <= cumul)
            {
                cibleChoisie = c;
                trouve = true;
                break;
            }
        }

        if (!trouve)
        {
            Debug.LogWarning($"{nameof(TeleporteController)} sur {name}: tirage invalide, fallback première cible valide.", this);
            // Fallback simple: première cible valide
            for (int i = 0; i < destinationsAleatoires.Length; i++)
            {
                var c = destinationsAleatoires[i];
                if (!string.IsNullOrWhiteSpace(c.nomScene) && (!exclureSceneCourante || c.nomScene != sceneCourante) && c.poids > 0f)
                {
                    cibleChoisie = c;
                    trouve = true;
                    break;
                }
            }
            if (!trouve) return;
        }

        SceneSpawnManager.SetNext(cibleChoisie.spawnId);
        SceneManager.LoadScene(cibleChoisie.nomScene);
    }
}
