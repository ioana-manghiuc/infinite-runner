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
            SpawnTile(_startingTile.GetComponent<Tile>());
        }
        //SpawnTile(SelectRandomGameObject(_turnTiles).GetComponent<Tile>());
        SpawnTile(_turnTiles[0].GetComponent<Tile>());
        AddNewDirection(Vector3.left);
    }

    private void SpawnTile(Tile tile, bool spawnObstacle = false)
    {
        Quaternion newTileRotation = tile.gameObject.transform.rotation 
                                     * Quaternion.LookRotation(_currentTileDirection, Vector3.up);

        _prevTile = Instantiate(tile.gameObject, _currentTileLocation, newTileRotation);
        _currentTiles.Add(_prevTile);
        if(tile.type == TileType.STRAIGHT)
            _currentTileLocation += Vector3.Scale(_prevTile.GetComponent<Renderer>().bounds.size,_currentTileDirection);
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

        int currentPathLenght = Random.Range(_miniumStraightTile, _maximumStraightTile);
        for(int i = 0; i < currentPathLenght; i++)
        {
            SpawnTile(_startingTile.GetComponent<Tile>(), (i != 0));
        }

        SpawnTile(SelectRandomGameObject(_turnTiles).GetComponent<Tile>());
    }
    private GameObject SelectRandomGameObject(List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0) return null;
        return gameObjects[Random.Range(0, gameObjects.Count)];
    }
    private void DeletePreviousTile()
    {}
}
