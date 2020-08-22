using UnityEngine;
using System.Collections.Generic;
using System;

public class EntityManager : MonoBehaviour { 

        #region Variables

    [Header("Enemy Chunks")]
    [SerializeField] private Vector2Int totalChunks = Vector2Int.one;

    [Range(1, 100)]
    [SerializeField] private int chunkSize = 5;

    [Header("Raycasting")]
    [SerializeField] private LayerMask raycastLayer = 0;


    // Actions
    public event Action<Vector2Int> onPlayerChangeChunk;

    // Script side
    private GameObject[] spawns = new GameObject[] {};
    private Vector2Int playerChunk = Vector2Int.zero;
    private List<GameObject> activeEntitys = new List<GameObject>();

        #endregion

    void Start()
    {
        spawns = GameObject.FindGameObjectsWithTag("MapSpawner");

        // EntitySpawn with spawnOnStart enabled
        foreach(GameObject spawnga in spawns)
        {
            if(spawnga.TryGetComponent<EntitySpawn>(out EntitySpawn spawn))
            {
                if(spawn.SpawnOnStart)
                    spawn.Spawn();
            }
        }

        CheckPlayerChunk();
    }

    void Update()
    {
        CheckPlayerChunk();
    }

    void CheckPlayerChunk() {

        Vector2Int chunk = GetChunkPos(GameManager.Player.transform.position);
        
        if(playerChunk != chunk)
        {
            playerChunk = chunk;
            onPlayerChangeChunk?.Invoke(playerChunk);

            Vector2 size = Vector2.one * 3 * chunkSize;
            Vector2 pos = new Vector2((playerChunk.x - 2) * chunkSize + size.x/2, (playerChunk.y + 1) * chunkSize - size.y/2);
            pos += (Vector2)this.transform.position;

            List<GameObject> ices = new List<GameObject>();
            
            foreach(Collider2D c in Physics2D.OverlapBoxAll(pos, size, 0f, raycastLayer)) {
                if(c.GetComponent<IChunkEntity>() != null)
                {
                    ices.Add(c.gameObject);
                    c.gameObject.GetComponent<IChunkEntity>().OnChunk();
                }
            }
            
            activeEntitys.RemoveAll(ice => ice == null);
            activeEntitys.ForEach(ice => {
                Debug.Log(ice == null); 
                Debug.Log(ice); 
                if(!ices.Contains(ice)) { ice.GetComponent<IChunkEntity>().OutChunk(); } 
            } );
            activeEntitys = ices;
        }

    }

    public void ClearAllEntities() {
        foreach(GameObject go in FindObjectsOfType<GameObject>())
        {
            if(go.GetComponent<IChunkEntity>() != null)
                Destroy(go);
        }
    }

    public Vector2Int GetChunkPos(Vector3 position)
    {
        Vector2 pos = (Vector2)(position - this.transform.position) / chunkSize;
        return new Vector2Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y));
    }

    public bool OnPlayerRange(Vector2Int chunkPos)
    {
        return Vector2Int.Distance(playerChunk, chunkPos) < 2;
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