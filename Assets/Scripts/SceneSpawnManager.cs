using UnityEngine;

// Gestionnaire statique pour mémoriser le prochain point d'apparition à l'entrée d'une scène
public static class SceneSpawnManager
{
    // Identifiant du SpawnPoint à utiliser dans la prochaine scène
    public static string NextSpawnId { get; private set; }

    public static void SetNext(string spawnId)
    {
        // Stocke l'ID que la scène suivante utilisera pour positionner le joueur
        NextSpawnId = spawnId;
    }

    public static void Clear()
    {
        // Réinitialise après utilisation pour éviter un respawn involontaire
        NextSpawnId = null;
    }
}
