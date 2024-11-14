using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stars : MonoBehaviour
{
    public Space spaceScript;
    public Universo universeScript;
    private bool isGeneratingStars = false;

    public void CreateStars()
    {
        if (isGeneratingStars) return; 

        isGeneratingStars = true;
        StartCoroutine(GenerateStars());
    }

    public void MaintainStars()
    {
        int width = spaceScript.width;
        int height = spaceScript.height;
        GameObject[,] grid = spaceScript.grid;

        // Recorrer todas las celdas
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsStar(x, y, grid)) 
                {
                    int starNeighbors = CountStarNeighbors(x, y, grid);

                    if (starNeighbors < 3)
                    {
                        GameObject star = grid[x, y];
                        SetCellColor(star, Color.black);
                    }
                }
            }
        }

        CreateStars();
    }

    private bool IsStar(int x, int y, GameObject[,] grid)
    {
        SpriteRenderer renderer = grid[x, y].GetComponent<SpriteRenderer>();
        return renderer.color == Color.white;
    }

    private int CountStarNeighbors(int x, int y, GameObject[,] grid)
    {
        int starCount = 0;
        int[] neighborOffsets = { -1, 0, 1 };

        foreach (int i in neighborOffsets)
        {
            foreach (int j in neighborOffsets)
            {
                if (i == 0 && j == 0) continue;  

                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX < 0 || neighborX >= grid.GetLength(0) || neighborY < 0 || neighborY >= grid.GetLength(1))
                    continue;

                if (IsStar(neighborX, neighborY, grid))
                {
                    starCount++;
                }
            }
        }

        return starCount; 
    }



    private IEnumerator GenerateStars()
    {
        List<Vector2Int> cellsToChange = new List<Vector2Int>(); 

        int width = spaceScript.width;
        int height = spaceScript.height;
        GameObject[,] grid = spaceScript.grid;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsCellDead(x, y, grid) && AreNeighborsDead(x, y, grid))
                {
                    if (Random.value < 0.0048f)
                    {
                        cellsToChange.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        int chunkSize = 50;
        int chunksPerRow = Mathf.CeilToInt((float)width / chunkSize);
        int totalChunks = chunksPerRow * Mathf.CeilToInt((float)height / chunkSize);

        for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            int chunkX = (chunkIndex % chunksPerRow) * chunkSize;
            int chunkY = (chunkIndex / chunksPerRow) * chunkSize;

            for (int x = chunkX; x < Mathf.Min(chunkX + chunkSize, width); x++)
            {
                for (int y = chunkY; y < Mathf.Min(chunkY + chunkSize, height); y++)
                {
                    if (cellsToChange.Contains(new Vector2Int(x, y)))
                    {
                        GameObject cell = grid[x, y];
                        SetCellColor(cell, Color.white); 
                    }
                }
            }

            yield return null;
        }

        isGeneratingStars = false;
        universeScript.createMeteoritesButton.interactable = true;
    }

    private bool IsCellDead(int x, int y, GameObject[,] grid)
    {
        SpriteRenderer renderer = grid[x, y].GetComponent<SpriteRenderer>();
        return renderer.color == Color.black; 
    }

    private bool AreNeighborsDead(int x, int y, GameObject[,] grid)
    {
        int[] neighborOffsets = { -1, 0, 1 };
        foreach (int i in neighborOffsets)
        {
            foreach (int j in neighborOffsets)
            {
                if (i == 0 && j == 0) continue; 

                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX < 0 || neighborX >= grid.GetLength(0) || neighborY < 0 || neighborY >= grid.GetLength(1))
                    continue;

                if (!IsCellDead(neighborX, neighborY, grid))
                    return false;
            }
        }
        return true;  
    }

    private void SetCellColor(GameObject cell, Color color)
    {
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
        renderer.color = color;
    }
}
