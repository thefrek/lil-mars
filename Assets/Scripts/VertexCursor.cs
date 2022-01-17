using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class VertexCursor : MonoBehaviour
{
    public Grid grid;

    public Vector3 cursorOffset = new Vector3(0, -0.1875f, 0);
    float zOffsetFactor = 0.25f;
    Vector3 zOffset;

    public GameObject mapManager;
    MapGeneration mapGeneration;

    public float moveSpeed = 0.1f;

    public int[] vertexSelected;
    List<Vector3> diagonals = new List<Vector3>();

    void Start()
    {
        UnityEngine.Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = DisplayVertexCursor();
    }

    Vector3 DisplayVertexCursor()
    {
        mapGeneration = mapManager.GetComponent<MapGeneration>();
        Vector3Int gridPosition = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if ((gridPosition.x + (mapGeneration.height / 2) >= 0) && (gridPosition.y + (mapGeneration.width / 2) >= 0) && (gridPosition.x + (mapGeneration.height / 2) <= mapGeneration.height) && (gridPosition.y + (mapGeneration.width / 2) <= mapGeneration.width))
        {
            zOffset = DetermineZOffset(mapGeneration.depthMap, gridPosition);
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        return Vector3.Lerp(transform.position, GetMousePositionOnGrid(), moveSpeed);
    }

    Vector3 DetermineZOffset(int[,] depthMap, Vector3Int _gridPosition)
    {
        int gridX = _gridPosition.x += (mapGeneration.height / 2);
        int gridY = _gridPosition.y += (mapGeneration.width / 2);

        float zOffset = depthMap[gridX, gridY] * zOffsetFactor;

        return new Vector3(0, zOffset, 0);
    }

    Vector3 GetMousePositionOnGrid()
    {
        mapGeneration = mapManager.GetComponent<MapGeneration>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = grid.WorldToCell(mousePos);
        Vector3Int trueGridPosition = new Vector3Int(gridPosition.x + mapGeneration.height / 2, gridPosition.y + mapGeneration.width / 2, 0);

        Vector3 nearestDiagonal = new Vector3(1000, 1000, 1000);

        diagonals.Clear();

        if (trueGridPosition.x > trueGridPosition.y)
        {
            for (int i = 0; i <= mapGeneration.width - (trueGridPosition.x - trueGridPosition.y); i++)
            {
                Vector3Int pos;

                int x = i + trueGridPosition.x - trueGridPosition.y;
                pos = new Vector3Int(x - mapGeneration.height / 2, i - mapGeneration.width / 2, mapGeneration.depthMap[x, i]);
                diagonals.Add(grid.GetCellCenterWorld(pos));
            }
        }
        else
        {
            for (int i = 0; i <= mapGeneration.height - (trueGridPosition.y - trueGridPosition.x); i++)
            {
                Vector3Int pos;

                int y = i + trueGridPosition.y - trueGridPosition.x;
                pos = new Vector3Int(i - mapGeneration.height / 2, y - mapGeneration.width / 2, mapGeneration.depthMap[i, y]);
                diagonals.Add(grid.GetCellCenterWorld(pos));
            }
        }

        foreach (Vector3 diagonal in diagonals)
        {
            if (Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(nearestDiagonal)) > Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(diagonal)))
            {
                nearestDiagonal = diagonal;
            }
        }

        return nearestDiagonal + cursorOffset;
    }
}