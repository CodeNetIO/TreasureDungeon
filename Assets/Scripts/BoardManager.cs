using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		public int Minimum { get; set; }
		public int Maximum { get; set; }

		public Count (int min, int max)
		{
			Minimum = min;
			Maximum = max;
		}
	}

	#region "Public Properties"
	public int Columns { get; set; }
	public int Rows { get; set; }
	public Count WallCount { get; set; }
	public Count FoodCount { get; set; }
	public GameObject Exit { get; set; }
	public GameObject[] FloorTiles { get; set; }
	public GameObject[] WallTiles { get; set; }
	public GameObject[] FoodTiles { get; set; }
	public GameObject[] EnemyTiles { get; set; }
	public GameObject[] OuterWallTiles { get; set; }
	#endregion

	#region "Fields"
	private Transform _boardHolder;
	private IList<Vector3> _gridPositions;
	#endregion

	#region "Constructors"
	public BoardManager ()
	{
		Columns = 8;
		Rows = 8;
		WallCount = new Count (5, 9);
		FoodCount = new Count (1, 5);
		_gridPositions = new List<Vector3>();
	}
	#endregion

	private void InitializeList()
	{
	    _gridPositions.Clear();
        // 1,1 -> Columns-1,Rows-1 (leave 1-square buffer around the sides)
	    for (var x = 1; x < Columns - 1; x ++)
	    {
	        for (var y = 1; y < Rows - 1; y++)
	        {
	            _gridPositions.Add(new Vector3(x,y,0f));
	        }
	    }
	}

    private void BoardSetup()
    {
        _boardHolder = new GameObject("Board").transform;
        for (var x = -1; x < Columns + 1; x++)
        {
            for (var y = -1; y < Rows + 1; y++)
            {
                GameObject tilePrefab;
                if (x == -1 || x == Columns || y == -1 || y == Rows)
                {
                    // outer wall
                    tilePrefab = OuterWallTiles[Random.Range(0, OuterWallTiles.Length)];
                }
                else
                {
                    // inner floor
                    tilePrefab = FloorTiles[Random.Range(0, FloorTiles.Length)];
                }

                var tile = Instantiate(tilePrefab, new Vector3(x,y,0f), Quaternion.identity) as GameObject;
                if (tile != null)
                {
                    tile.transform.SetParent(_boardHolder);
                }
                else
                {
                    // log error
                }
                {
                    
                }

            }
        }
    }

    private Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, _gridPositions.Count);
        var randomPosition = _gridPositions[randomIndex];
        _gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (var i = 0; i < objectCount; i++)
        {
            var randomPosition = RandomPosition();
            var tilePrefab = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tilePrefab, randomPosition, Quaternion.identity);
        }
    }
	// Use this for initialization
	private void Start () {

	}
	
	// Update is called once per frame
	private void Update () {
	
	}

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(WallTiles, WallCount.Minimum, WallCount.Maximum);
        LayoutObjectAtRandom(FoodTiles, FoodCount.Minimum, FoodCount.Maximum);
        var enemyCount = (int) Mathf.Log(level, 2f);
        LayoutObjectAtRandom(EnemyTiles, enemyCount, enemyCount);
        Instantiate(Exit, new Vector3(Columns-1, Rows-1, 0f), Quaternion.identity);
    }
}
