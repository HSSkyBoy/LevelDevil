using UnityEngine;

/// <summary>
/// Declares the fixed authoring anchors for newly-created Level Prefabs.
/// This component is intentionally opt-in so legacy Levels keep their existing behaviour.
/// </summary>
public class LevelTemplateDefinition : MonoBehaviour
{
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Gate gate;
    [SerializeField] private Collider2D cameraBounds;
    [SerializeField] private PlayerCtrl playerPrefab;

    public Transform PlayerSpawn => playerSpawn;
    public Gate Gate => gate;
    public Collider2D CameraBounds => cameraBounds;

    private void Awake()
    {
        if (playerPrefab == null || playerSpawn == null || GetComponentInChildren<PlayerCtrl>(true) != null)
        {
            return;
        }

        Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation, transform);
    }
}
