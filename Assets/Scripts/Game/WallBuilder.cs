using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class WallPassingEventArgs : AchievePropertyEventArgs
{
    public Vector3 FigurePrevPosition { get; set; }
    public WallPassingEventArgs(Vector3 figurePosition, AchievementPropName name, bool completed) : base(name, completed)
    {
        FigurePrevPosition = figurePosition;
    }
}

public class LevelPassingEventArgs : System.EventArgs
{
    public int NextWallSize { get; set; }
    public LevelPassingEventArgs(int wallSize)
    {
        NextWallSize = wallSize;
    }
}

public class WallBuilder : MonoBehaviour
{
    public float wallDistance;
    public float[] possibleRotations;
    public Vector2 holePosition;
    public RotatingDirection rotatingDirection;
    public Object cubePrefab;
    private Texture cubeTexture;

    public Object coinPrefab;
    public Object[] figurePrefabs;

    public event System.EventHandler<WallPassingEventArgs> WallPassed;
    public event System.EventHandler<LevelPassingEventArgs> LevelPassed;

    private Transform wallTransform = null;
    private GameObject figure = null;

    private int totalWallsPassed = 0;
    private int walls = 0;

    public int wallSize;
    public int maxWallSize;

    private float cubeSize = .5f;

    // X,Y positions of cubes in wall
    private List<Vector2> matrix;

    private void Awake()
    {
        BuildWall();
    }

    private void OnDisable()
    {
        wallTransform?.GetComponent<Wall>().WallDestroyed.RemoveListener(PassWall);
    }

    void SpawnWallObject()
    {
        var gObj = new GameObject("Wall");
        gObj.AddComponent<Wall>().WallDestroyed.AddListener(PassWall);
        gObj.transform.position = new Vector3(-(wallSize - 1) * .5f * cubeSize, (wallSize - 1) * .5f * cubeSize, wallDistance);
        wallTransform = gObj.transform;
    }

    void InitMatrix()
    {
        matrix = new List<Vector2>();
        for (float i = 0; i < wallSize; i++)
            for (float j = 0; j < wallSize; j++)
                matrix.Add(new Vector2(
                    wallTransform.position.x + i * cubeSize,
                    wallTransform.position.y - j * cubeSize
                    ));

        //var wallPositionX = wallTransform.position.x;
        //var wallPositionY = wallTransform.position.y;

        //Parallel.For(0, wallSize, (i)=>
        //{
        //    for (float j = 0; j < wallSize; j++)
        //        matrix.Add(new Vector2(
        //            wallPositionX + i * cubeSize,
        //            wallPositionY - j * cubeSize
        //            ));
        //});
    }

    void SpawnFigureWithRotation(Vector3 rotation)
    {
        figure = Instantiate(
            figurePrefabs[Random.Range(0, figurePrefabs.Length)],
            Vector3.zero,
            Quaternion.Euler(rotation)
            ) as GameObject;
    }

    void PassWall()
    {
        totalWallsPassed++;
        walls++;
        //if (walls % 3 == 0 && GameManager.LEVEL < GameManager.MAX_LEVEL)
        //{
        //    wallSize += (wallSize < maxWallSize ? 2 : 0);
        //    LevelPassed?.Invoke(this, new LevelPassingEventArgs(wallSize));
        //}
        //if (GameManager.LEVEL < GameManager.MAX_LEVEL)
        //    LevelPassed?.Invoke(this, new LevelPassingEventArgs(wallSize));

        if (walls == GameManager.LEVEL && GameManager.LEVEL < GameManager.MAX_LEVEL)
        {
            LevelPassed?.Invoke(this, new LevelPassingEventArgs(wallSize));
            walls = 0;
        }

        var figurePos = figure.GetComponent<FigureMotion>().startPos;
        Destroy(figure);
        BuildWall();
        WallPassed?.Invoke(this, new WallPassingEventArgs(figurePos, AchievementPropName.FirstWallPassedOnBoost, totalWallsPassed < 2 && GameManager.BOOST));
    }

    public void BuildWall()
    {
        SpawnWallObject();
        InitMatrix();

        SpawnFigureWithRotation(
            rotatingDirection == RotatingDirection.Random
            ? new Vector3(
                possibleRotations[Random.Range(0, possibleRotations.Length)],
                possibleRotations[Random.Range(0, possibleRotations.Length)],
                possibleRotations[Random.Range(0, possibleRotations.Length)]
                )
            : Rotations.eulers[rotatingDirection]
            );

        cubeTexture = Resources.Load($"Textures/Squares/{GameManager.LEVEL}lvl/4") as Texture;
        List<Vector2> figureCubesPos = GetCubesPositions();
        figure.transform.eulerAngles = Vector3.zero;

        //Set the hole position
        var locker = new object();
        var success = false;

        while (!success)
        {
            var holePositions = new List<Vector2>();
            Vector2 randPos = matrix[Random.Range(0, matrix.Count)];

            if (holePosition != -Vector2.one)
            {
                try
                {
                    var pos = matrix[(int)(holePosition.x + holePosition.y * wallSize)];
                    randPos = pos;
                }
                catch (System.IndexOutOfRangeException)
                {
                    throw;
                }
            }

            Parallel.ForEach(figureCubesPos, cubePos =>
            {
                Parallel.For(0, matrix.Count, (i, state) =>
                {
                    lock (locker)
                    {
                        if (cubePos + randPos == matrix[i])
                        {
                            holePositions.Add(matrix[i]);
                            state.Break();
                        }
                    }
                });
            });

            //Parallel.ForEach(figureCubesPos, cubePos => {
            //    for (int i = 0; i < matrix.Count; i++)
            //    {
            //        if (cubePos + randPos == matrix[i])
            //        {
            //            holePositions.Add(matrix[i]);
            //            break;
            //        }
            //    }
            //});

            if (holePositions.Count == figureCubesPos.Count)
            {
                success = true;
                foreach (var hole in holePositions)
                    matrix.Remove(hole);
                foreach (var coinPos in figureCubesPos)
                    (Instantiate(coinPrefab, new Vector3(coinPos.x + randPos.x, coinPos.y + randPos.y, wallDistance), Quaternion.identity, wallTransform) as GameObject)
                        .SetActive(true);
                foreach (var pos in matrix)
                    (Instantiate(cubePrefab, new Vector3(pos.x, pos.y, wallDistance), Quaternion.identity, wallTransform) as GameObject)
                        .GetComponent<Renderer>().material.mainTexture = cubeTexture;
            }
        }
    }

    List<Vector2> GetCubesPositions()
    {
        //Get (x,y) of each cube in figure
        //var temp = figure.GetComponentsInChildren<FigureElement>().ToList();
        List<Transform> temp = new List<Transform>();
        var childCount = figure.transform.childCount;
        var figureTransform = figure.transform;
        for (int i = 0; i < childCount; i++)
            temp.Add(figureTransform.GetChild(i));

        List<Vector2> cubesXY = new List<Vector2>();
        //foreach (var item in temp)
        //    item.transform.eulerAngles = Vector3.zero;

        //temp.ForEach(cube => cubesXY.Add(
        //       new Vector2(cube.transform.position.x, cube.transform.position.y)
        //       ));

        foreach (var cube in temp)
            cube.eulerAngles = Vector3.zero;

        temp.ForEach(cube => cubesXY.Add(
               new Vector2(cube.position.x, cube.position.y)
               ));

        return DistinctListV2(cubesXY);
    }

    #region Vector2 methods

    float Vector2DistanceRound(Vector2 v1, Vector2 v2, int multiplier = 100)
        => Mathf.Round(multiplier * Vector2.Distance(v1, v2));

    List<Vector2> DistinctListV2(List<Vector2> vector2s)
    {
        var result = new List<Vector2> { vector2s.First() };

        for (int i = 0; i < vector2s.Count; i++)
            for (int j = 0; j < result.Count; j++)
            {
                if (Vector2DistanceRound(vector2s[i], result[j]) == 0)
                    break;
                if (j + 1 == result.Count)
                    result.Add(vector2s[i]);
            }

        return result;
    }
    #endregion
}
