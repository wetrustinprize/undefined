using UnityEngine;
using System.Collections.Generic;

public class EntityManager : MonoBehaviour { 

        #region Variables

    [Header("Enemy Chunks")]
    [SerializeField] private Vector2Int totalChunks = Vector2Int.one;

    [Range(1, 100)]
    [SerializeField] private int chunkSize = 5;

    [Header("Raycasting")]
    [SerializeField] private LayerMask raycastLayer = 0;

    [Header("Spawns")]
    [SerializeField] private List<EntitySpawn> spawns = new List<EntitySpawn>();

    // Script side

    private Vector2Int playerChunk = Vector2Int.zero;
    private List<IChunkEntity> activeEntitys = new List<IChunkEntity>();

        #endregion

    void Start()
    {
        CheckPlayerChunk();
    }

    void FixedUpdate()
    {
        CheckPlayerChunk();
    }

    void CheckPlayerChunk() {

        Vector2Int chunk = GetChunkPos(GameManager.Player.transform.position);
        
        if(playerChunk != chunk)
        {
            playerChunk = chunk;

            Vector2 size = Vector2.one * 3 * chunkSize;
            Vector2 pos = new Vector2((playerChunk.x - 2) * chunkSize + size.x/2, (playerChunk.y + 1) * chunkSize - size.y/2);
            pos += (Vector2)this.transform.position;

            List<IChunkEntity> ices = new List<IChunkEntity>();
            
            foreach(Collider2D c in Physics2D.OverlapBoxAll(pos, size, 0f, raycastLayer))
                if(c.TryGetComponent<IChunkEntity>(out IChunkEntity ice)) ices.Add(ice);

            ices.ForEach(ice => ice.OnChunk());
            Debug.Log(ices);
            
            activeEntitys.ForEach(ice => { if(!ices.Contains(ice)) { ice.OutChunk(); } } );
            activeEntitys = ices;
        }

    }

    public Vector2Int GetChunkPos(Vector3 position)
    {
        Vector2 pos = (Vector2)(position - this.transform.position) / chunkSize;
        return new Vector2Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y));
    }

    public void OnDrawGizmos() {

        Vector3 center = Vector3.zero;
        Gizmos.color = Color.gray;
    
        for(int x = 0; x < totalChunks.x; x++)
        {
            for(int y = 0; y < totalChunks.y; y++)
            {
                center = new Vector3(x * chunkSize + chunkSize/2, y * chunkSize + chunkSize/2, 0);
                center += transform.position;

                Gizmos.DrawWireCube(center, Vector2.one * chunkSize);
            }
        }

        Gizmos.color = Color.yellow;
        for(int x = -1; x < 2; x++)
        {
            for(int y = -1; y < 2; y++)
            {
                if(x == 0 && y == 0) continue;

                center = new Vector3((playerChunk.x - 1) * chunkSize + chunkSize/2, (playerChunk.y - 1) * chunkSize + chunkSize/2, 0);
                center += new Vector3(x * chunkSize, y * chunkSize);
                center += transform.position;

                Gizmos.DrawWireCube(center, Vector2.one * chunkSize);
            }
        }

        Gizmos.color = Color.green;
        center = new Vector3((playerChunk.x - 1) * chunkSize + chunkSize/2, (playerChunk.y - 1) * chunkSize + chunkSize/2, 0);
        center += transform.position;
        Gizmos.DrawWireCube(center, Vector2.one * chunkSize);

    }

}