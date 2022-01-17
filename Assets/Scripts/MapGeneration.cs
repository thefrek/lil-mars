using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class MapGeneration : MonoBehaviour
{
    public int width;
    public int height;

    public bool debugMode; //Prevents map being put in center of screen.

    [Range(0,100)]
    public int roughness;

    public bool removePeaks;
    public int smoothingPasses;

    public Tilemap terrain;

    public GameObject cursorManager;
    public VertexCursor vertexCursor;

    //Definintions for which tile to place based on relative height of its four corners 

    public Tile nullTile;

    public Tile emptyTile;
    static int[] empty = {0, 0, 0, 0};


    public Tile upNorthTile;
    static int[] upNorth = {1, 0, 0, 0};

    public Tile upEastTile;
    static int[] upEast = {0, 1, 0, 0};
    
    public Tile upWestTile;
    static int[] upWest = { 0, 0, 1, 0 };

    public Tile upSouthTile;
    static int[] upSouth = { 0, 0, 0, 1 };


    public Tile upNETile;
    static int[] upNE = { 1, 1, 0, 0 };

    public Tile upNWTile;
    static int[] upNW = { 1, 0, 1, 0 };

    public Tile upSETile;
    static int[] upSE = { 0, 1, 0, 1 };

    public Tile upSWTile;
    static int[] upSW = { 0, 0, 1, 1 };


    public Tile fullUpNorthTile;
    static int[] fullUpNorth = { 2, 1, 1, 0 };

    public Tile fullUpEastTile;
    static int[] fullUpEast = { 1, 2, 0, 1 };

    public Tile fullUpWestTile;
    static int[] fullUpWest = { 1, 0, 2, 1 };

    public Tile fullUpSouthTile;
    static int[] fullUpSouth = { 0, 1, 1, 2 };


    public Tile downNorthTile;
    static int[] downNorth = { 0, 1, 1, 1 };

    public Tile downEastTile;
    static int[] downEast = { 1, 0, 1, 1 };

    public Tile downWestTile;
    static int[] downWest = { 1, 1, 0, 1 };

    public Tile downSouthTile;
    static int[] downSouth = { 1, 1, 1, 0 };

    int[,] map;
    public int[,] depthMap;

    void GenerateDepthMap()
    {
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                if (y == 0 && x == 0)
                {
                    //Set the height of the first vertex to 0
                    depthMap[x, y] = 0;
                }
                else if (x == 0)
                {
                    depthMap[x, y] = depthMap[x, y - 1] + DetermineDepth(-1, 2, 0); //-1, 0 , 1
                }
                else if (y == 0)
                {
                    depthMap[x, y] = depthMap[x - 1, y] + DetermineDepth(-1, 2, 0); //-1, 0 , 1
                }
                else
                {
                    //Set array "slope" to be the height distance between corner to the upper left and the corners up and right, respectively.
                    int[] slope = { depthMap[x - 1, y] - depthMap[x - 1, y - 1], depthMap[x, y - 1] - depthMap[x - 1, y - 1] };

                    if (slope[0] == 0 && slope[1] == 0)
                    {
                        depthMap[x, y] = depthMap[x - 1, y - 1] + DetermineDepth(-1, 2, 0); //-1, 0 , 1
                    }

                    if ((slope[0] == 1 ^ slope[1] == 1) && (slope[0] == 0 ^ slope[1] == 0))
                    {
                        depthMap[x, y] = depthMap[x - 1, y - 1] + Random.Range(0, 2); //0, 1
                    }

                    if (slope[0] == 1 && slope[1] == 1)
                    {
                        depthMap[x, y] = depthMap[x - 1, y - 1] + DetermineDepth(1, 3, 1); //1, 2
                    }

                    if ((slope[0] == -1 ^ slope[1] == -1) && (slope[0] == 0 ^ slope[1] == 0))
                    {
                        depthMap[x, y] = depthMap[x - 1, y - 1] + Random.Range(-1, 1); //-1, 0
                    }

                    if (slope[0] == -1 && slope[1] == -1)
                    {
                        depthMap[x, y] = depthMap[x - 1, y - 1] + DetermineDepth(-1, -3, -1); //-1, -2
                    }

                    if ((slope[0] == -1 ^ slope[1] == -1) && (slope[0] == 1 ^ slope[1] == 1))
                    {
                        depthMap[x, y] = depthMap[x - 1, y - 1]; //0
                    }
                }
            }
        }
    }

    int DetermineDepth(int lowerBoundInclusive, int upperBoundExclusive, int flat)
    {
        if (Random.Range(0, 99) < roughness)
        {
            return Random.Range(lowerBoundInclusive, upperBoundExclusive);
        }
        else
        {
            return flat;
        }
    }

    void RemovePeaks()
    {
        void RingCount(int _x, int _y)
        {
            int ringCount = 0;
            int ringCountSatisfied = 0;
            int positiveOrNegative = 0;

            int xStartValue = -1;
            int yStartValue = -1;
            int xEndValue = 1;
            int yEndValue = 1;

            if(_x == 0 && _y == 0)
            {
                ringCountSatisfied = 2;
            }
            else if(_x == 0 || _y == 0 || _x == width || _y == height)
            {
                ringCountSatisfied = 3;
            }
            else
            {
                ringCountSatisfied = 3;
            }

            if(_x == 0)
            {
                xStartValue = 0;
            }
            if (_y == 0)
            {
                yStartValue = 0;
            }
            if (_x == width)
            {
                xEndValue = 0;
            }
            if (_y == height)
            {
                yEndValue = 0;
            }

            for (int x = xStartValue; x <= xEndValue; x ++)
            {
                for (int y = yStartValue; y <= yEndValue; y ++)
                {
                    if ((x == 0 || y == 0))
                    {
                        if (depthMap[_x + x, _y + y] == depthMap[_x, _y] + 1)
                        {
                            ringCount++;
                            positiveOrNegative = 1;
                        }

                        if (depthMap[_x + x, _y + y] == depthMap[_x, _y] - 1)
                        {
                            ringCount--;
                            positiveOrNegative = -1;
                        }
                    }
                }
            }

            if (Mathf.Abs(ringCount) == ringCountSatisfied || Mathf.Abs(ringCount) == 4)
            {
                depthMap[_x, _y] += positiveOrNegative;
            }

        }

        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                RingCount(x, y);
            }
        }
    }

    void DetermineTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int[] cornerDepths = { depthMap[x + 1, y + 1], depthMap[x + 1, y], depthMap[x, y + 1], depthMap[x, y] };
                int z = Mathf.Min(cornerDepths);

                Tile tileToPlace = emptyTile;

                int[] cornerDepthsNormalized = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    cornerDepthsNormalized[i] = cornerDepths[i] - z;
                }

                //Debug.Log(x + " " + y + " " + "zValue = " + z);
                //Debug.Log(x + " " + y + " " + "Original Depths = " + cornerDepths[0] + "," + cornerDepths[1] + "," + cornerDepths[2] + "," + cornerDepths[3]);
                //Debug.Log(x + " " + y + " " + "Normalized Depths = " + cornerDepthsNormalized[0] + "," + cornerDepthsNormalized[1] + "," + cornerDepthsNormalized[2] + "," + cornerDepthsNormalized[3]);

                //Determine which tile to place based on definitions from earlier and the corner depths of the 4 vertices surrounding each square.
                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, empty))
                {
                    tileToPlace = emptyTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upNorth))
                {
                    tileToPlace = upNorthTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upEast))
                {
                    tileToPlace = upEastTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upWest))
                {
                    tileToPlace = upWestTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upSouth))
                {
                    tileToPlace = upSouthTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upNE))
                {
                    tileToPlace = upNETile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upNW))
                {
                    tileToPlace = upNWTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upSE))
                {
                    tileToPlace = upSETile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, upSW))
                {
                    tileToPlace = upSWTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, fullUpNorth))
                {
                    tileToPlace = fullUpNorthTile;
                    z++;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, fullUpEast))
                {
                    tileToPlace = fullUpEastTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, fullUpWest))
                {
                    tileToPlace = fullUpWestTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, fullUpSouth))
                {
                    tileToPlace = fullUpSouthTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, downNorth))
                {
                    tileToPlace = downNorthTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, downEast))
                {
                    tileToPlace = downEastTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, downWest))
                {
                    tileToPlace = downWestTile;
                }

                if (ArrayUtility.ArrayEquals(cornerDepthsNormalized, downSouth))
                {
                    tileToPlace = downSouthTile;
                }

                //Debug.Log(x + " " + y + " " + "Tile placed: " + tileToPlace);

                if (debugMode)
                {
                    terrain.SetTile(new Vector3Int(x, y, z), tileToPlace);
                }
                else
                {
                    terrain.SetTile(new Vector3Int(x - (height / 2), y - (width / 2), z), tileToPlace);
                }
            }
        }
    }

    void RaiseVertex(int amount)
    {
        vertexCursor = cursorManager.GetComponent<VertexCursor>();
        depthMap[vertexCursor.vertexSelected[0], vertexCursor.vertexSelected[1]] += amount;
    }

    public void ClearMap()
    {
        terrain.ClearAllTiles();
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        depthMap = new int[width+1, height+1];

        GenerateDepthMap();

        if (removePeaks)
        {
            for(int i = 0; i < smoothingPasses; i++)
            {
                RemovePeaks();
            }
        }

        DetermineTiles();      
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            RaiseVertex(1);
            DetermineTiles();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaiseVertex(-1);
            DetermineTiles();
        }*/
    }
}
