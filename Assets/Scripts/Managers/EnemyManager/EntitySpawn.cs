using UnityEngine;

public class EntitySpawn : MonoBehaviour {

        #region Variables

    [Header("Spawn Information")]
    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private GameObject gameObjectToSpawn = null;

    [Header("Respawn Information")]
    [SerializeField] private bool respawnOnDie = false;
    [SerializeField] private float respawnTime = 1.0f;

    // Script side
    GameObject myEntity = null;
    float timer = 0.0f;
    Vector2Int myChunkPos = Vector2Int.zero;

    // Public variables
    public bool myEntityAlive { get { return myEntity == null; } }
    public bool SpawnOnStart { get { return spawnOnStart; } }

        #endregion

    void Start() {

        myChunkPos = GameManager.Entity.GetChunkPos(this.transform.position);

    }

    public void Spawn() {
        myEntity = Instantiate(gameObjectToSpawn, this.transform.position, this.transform.rotation);

        if(respawnOnDie)
            myEntity.GetComponent<Alive>().onDie += OnDieEntity;

        if(myEntity.TryGetComponent<IChunkEntity>(out IChunkEntity ice))
            ice.OutChunk();
    }

    void ChunkCheck(Vector2Int playerPos) {
        if(GameManager.Entity.OnPlayerRange(this.myChunkPos)) return;
        GameManager.Entity.onPlayerChangeChunk -= ChunkCheck;
        Spawn();
    }

    void OnDieEntity() {
        Debug.Log("Teste");
        timer = respawnTime;
    }

    void FixedUpdate() {

        if(timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            if(timer <= 0)
            {
                if(!GameManager.Entity.OnPlayerRange(this.myChunkPos))
                {
                    Spawn();
                }
                else
                {
                    GameManager.Entity.onPlayerChangeChunk += ChunkCheck;
                }
            }
        }

    }

}