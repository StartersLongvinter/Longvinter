using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;

public class MapSpawner : MonoBehaviourPun
{
    [SerializeField] List<MapDataTable> mapDataTable = new List<MapDataTable>();
    [SerializeField] private float mapTileSize = 50f;
    List<string> currentMapNames = new List<string>();
    List<string> presentMapNames = new List<string>();
    private Vector2 playerPositionTile;
    private Vector2 prePositionTile;
    private Transform playerPosition;

    private void Start()
    {
        playerPosition = this.gameObject.transform;
        playerPositionTile = new Vector2(0, 0);
        currentMapNames.Clear();
        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                if (((int)playerPositionTile.x + x) < 0 || ((int)playerPositionTile.y + z) < 0) continue;
                if (mapDataTable.Count - 1 < (int)playerPositionTile.y + z || mapDataTable[(int)playerPositionTile.y + z].mapDatas.Count - 1 < (int)playerPositionTile.x + x) continue;
                string sceneName = mapDataTable[(int)playerPositionTile.y + z].mapDatas[(int)playerPositionTile.x + x];
                if (!presentMapNames.Contains(sceneName) || SceneManager.GetSceneByName(sceneName) == null)
                {
                    SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                }
                currentMapNames.Add(sceneName);
            }
        }
    }

    private void Update()
    {
        prePositionTile = playerPositionTile;
        playerPositionTile = new Vector2(Mathf.RoundToInt(playerPosition.position.x / mapTileSize), Mathf.RoundToInt(playerPosition.position.z / mapTileSize));
        if (playerPositionTile != prePositionTile)
        {
            presentMapNames.Clear();
            for (int i = 0; i < currentMapNames.Count; i++)
                presentMapNames.Add(currentMapNames[i]);
            currentMapNames.Clear();
            for (int x = -1; x < 2; x++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if (((int)playerPositionTile.x + x) < 0 || ((int)playerPositionTile.y + z) < 0) continue;
                    if (mapDataTable.Count - 1 < (int)playerPositionTile.y + z || mapDataTable[(int)playerPositionTile.y + z].mapDatas.Count - 1 < (int)playerPositionTile.x + x) continue;
                    string sceneName = mapDataTable[(int)playerPositionTile.y + z].mapDatas[(int)playerPositionTile.x + x];
                    if (!presentMapNames.Contains(sceneName) || SceneManager.GetSceneByName(sceneName) == null)
                    {
                        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    }
                    currentMapNames.Add(sceneName);
                }
            }
            foreach (string _sceneName in presentMapNames)
            {
                if (!currentMapNames.Contains(_sceneName))
                {
                    SceneManager.UnloadSceneAsync(_sceneName);
                }
            }
        }
    }
}

// 0,0 = z0, x0
// 0,1 = z0, x1
// z 0 -> -1, 0, 1
// x 0 -> -1, 0, 1
