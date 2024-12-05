using System.Collections.Generic;
using System.Numerics;
using TempleRun;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private int _tileStartCount = 10;
    [SerializeField]
    private int _miniumStraightTile = 3;
    [SerializeField]
    private int _maximumStraightTile = 15;

    [SerializeField]
    private GameObject _startingTile;
    [SerializeField] 
    private List<GameObject> _turnTiles;
    [SerializeField] 
    private List<GameObject> _obstacles;

    private Vector3 _currentTileLocation = Vector3.zero;
    private Vector3 _currentTileDirection = Vector3.forward;
    private GameObject _prevTile;

    private List<GameObject> _currentTiles;
    private List<GameObject> _currentObstacles;


    private void Start()
    {
        _currentObstacles = new List<GameObject>();
        _currentTiles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < _tileStartCount; i++)
        {
            SpawnTile(_startingTile.GetComponent<Tile>(),false);
        }

        
    }

    private void SpawnTile(Tile tile, bool spawnObstacle)
    {
        _prevTile = Instantiate(tile.gameObject, _currentTileLocation, Quaternion.identity);
        _currentTiles.Add(_prevTile);
        _currentTileLocation += Vector3.Scale(_prevTile.GetComponent<Renderer>().bounds.size,_currentTileDirection);

    }
}
