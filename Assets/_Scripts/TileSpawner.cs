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
    private const int TileStartCount = 7;

    [SerializeField]
    private const int MinimumStraightTile = 10;

    [SerializeField]
    private const int MaximumStraightTile = 20;
    [SerializeField]
    private int MinimumObstacleDistance = 0;
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

    private int currentObstacleDistance = 2;
    


    private void Start()
    {
        _currentObstacles = new List<GameObject>();
        _currentTiles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < TileStartCount; i++)
        {
            SpawnTile(_startingTile.GetComponent<Tile>());
        }

        var turnTile = SelectRandomGameObject(_turnTiles).GetComponent<Tile>();

        SpawnTile(turnTile);
        SpawnFakeAheadPath(turnTile);

    }

    private void SpawnTile(Tile tile, bool spawnObstacle = false)
    {
        Quaternion newTileRotation = tile.gameObject.transform.rotation 
                                     * Quaternion.LookRotation(_currentTileDirection, Vector3.up);

        _prevTile = Instantiate(tile.gameObject, _currentTileLocation, newTileRotation);
        _currentTiles.Add(_prevTile);

        if(spawnObstacle && currentObstacleDistance >MinimumObstacleDistance)
        {
            currentObstacleDistance = 0;
            SpawnObstacle();
        }
        else if (spawnObstacle)
        {
            currentObstacleDistance++;
        }

        if(tile.type == TileType.STRAIGHT)
            _currentTileLocation += Vector3.Scale(_prevTile.GetComponent<Renderer>().bounds.size,_currentTileDirection);
    }

    private void SpawnObstacle()
    {
        if (Random.value > 0.5f) return;

        GameObject obstaclePrefab = SelectRandomGameObject(_obstacles);
        Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation
                                       * Quaternion.LookRotation(_currentTileDirection, Vector3.up);
        GameObject obstacle = Instantiate(obstaclePrefab, _currentTileLocation, newObjectRotation);
        _currentObstacles.Add(obstacle);
    }

    public void AddNewDirection(Vector3 direction)
    {
        _currentTileDirection = direction;
        DeletePreviousTile();

        Vector3 tilePlacementScale;
        if (_prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
        {
            tilePlacementScale = Vector3.Scale(
                _prevTile.GetComponent<Renderer>().bounds.size/2
                + Vector3.one * _startingTile.GetComponent<BoxCollider>().size.z/2
                ,_currentTileDirection);
        }
        else
        {
            tilePlacementScale = Vector3.Scale(
                (_prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2))
                + (Vector3.one * _startingTile.GetComponent<BoxCollider>().size.z / 2)
                , _currentTileDirection
                );
        }

        _currentTileLocation += tilePlacementScale;

        int currentPathLenght = Random.Range(MinimumStraightTile, MaximumStraightTile);
        for(int i = 0; i < currentPathLenght; i++)
        {
            SpawnTile(_startingTile.GetComponent<Tile>(), (i != 0));
        }

        var turnTile = SelectRandomGameObject(_turnTiles).GetComponent<Tile>();
        SpawnTile(turnTile);
        SpawnFakeAheadPath(turnTile);
    }

    private GameObject SelectRandomGameObject(List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0) return null;
        return gameObjects[Random.Range(0, gameObjects.Count)];
    }

    private void DeletePreviousTile()
    {
        while(_currentTiles.Count != 1)
        {
            GameObject tile = _currentTiles[0];
            _currentTiles.RemoveAt(0);
            Destroy(tile);
        }

        while (_currentObstacles.Count != 0)
        {
            GameObject obstacle = _currentObstacles[0];
            _currentObstacles.RemoveAt(0);
            Destroy(obstacle);
        }
    }

    private void SpawnFakeAheadPath(Tile cornerTile)
    {
        //Vector3 fakeTileDirection = Vector3.forward;
        //if (_currentTileDirection == Vector3.forward || _currentTileDirection == Vector3.back)
        //    fakeTileDirection = Vector3.right;
        //if(_currentTileDirection == Vector3.right || _currentTileDirection == Vector3.left)
        //    fakeTileDirection = Vector3.forward;


        //if (cornerTile.type is TileType.LEFT or TileType.SIDEWAYS)
        //{
        //    for(int i=0;i<10;i++)
        //    {
        //        Vector3 tileSize = _prevTile.GetComponent<Renderer>().bounds.size; // Get the size of the tile
        //        Vector3 offset = new Vector3(-tileSize.x*0.5f*i, 0, 0); // Offset in the upward (y) direction
        //        Vector3 fakeTileLocation =
        //            _prevTile.transform.position + offset; // Add the offset to the previous tile's position

        //        Quaternion fakeTileRotation = Quaternion.LookRotation(fakeTileDirection, Vector3.up);
        //        GameObject fakeTile = Instantiate(_startingTile.gameObject, fakeTileLocation, fakeTileRotation);
        //        _currentTiles.Insert(0, fakeTile);
        //    }
        //}
        //if (cornerTile.type is TileType.RIGHT or TileType.SIDEWAYS)
        //{
        //    for(int i=0;i<10;i++)
        //    {
        //        Vector3 tileSize = _prevTile.GetComponent<Renderer>().bounds.size; // Get the size of the tile
        //        Vector3 offset = new Vector3(tileSize.x*i*.5f, 0, 0); // Offset in the upward (y) direction
        //        Vector3 fakeTileLocation =
        //            _prevTile.transform.position + offset; // Add the offset to the previous tile's position


        //        Quaternion fakeTileRotation = Quaternion.LookRotation(fakeTileDirection, Vector3.up);
        //        GameObject fakeTile = Instantiate(_startingTile.gameObject, fakeTileLocation, fakeTileRotation);
        //        _currentTiles.Insert(0, fakeTile);
        //    }
        //}
    }
}


