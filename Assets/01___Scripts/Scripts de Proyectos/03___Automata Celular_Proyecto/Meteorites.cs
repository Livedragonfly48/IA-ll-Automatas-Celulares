using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Meteorites : MonoBehaviour
{
    
    public Space spaceScript;
    public Planets planetsScript;
    private bool isGeneratingMeteorites = false;  

    public void CreateMeteorites()
    {
        if (isGeneratingMeteorites) return;  

        isGeneratingMeteorites = true;
        StartCoroutine(GenerateMeteorites()); 
    }

    // Mantener los meteoritos en el espacio
    public void MaintainMeteorites()
    {
        int width = spaceScript.width;
        int height = spaceScript.height;
        GameObject[,] grid = spaceScript.grid;

        List<Vector2Int> meteoritesToChange = new List<Vector2Int>(); 

        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsMeteorite(x, y, grid) && !HasStarNeighbor(x, y, grid))
                {
                    meteoritesToChange.Add(new Vector2Int(x, y)); 
                }
            }
        }

        foreach (var meteoritePos in meteoritesToChange)
        {
            GameObject meteorite = grid[meteoritePos.x, meteoritePos.y];
            SetCellColor(meteorite, Color.black); 
        }
      
        CreateMeteorites();       
    }

    private bool IsMeteorite(int x, int y, GameObject[,] grid)
    {
        SpriteRenderer renderer = grid[x, y].GetComponent<SpriteRenderer>();
        return renderer.color == new Color(0.545f, 0.271f, 0.074f);  
    }

    private IEnumerator GenerateMeteorites()
    {
        List<Vector2Int> cellsToChange = new List<Vector2Int>();  

        int width = spaceScript.width;
        int height = spaceScript.height;
        GameObject[,] grid = spaceScript.grid;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsCellDead(x, y, grid) && HasStarNeighbor(x, y, grid))
                {
                    if (Random.value < 0.048f)
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
                        SetCellColor(cell, new Color(0.545f, 0.271f, 0.074f)); 
                    }
                }
            }

            yield return null;
        }

        isGeneratingMeteorites = false;  
        planetsScript.EnablePlanetPainting();
    }

    private bool IsCellDead(int x, int y, GameObject[,] grid)
    {
        SpriteRenderer renderer = grid[x, y].GetComponent<SpriteRenderer>();
        return renderer.color == Color.black;
    }

    private bool HasStarNeighbor(int x, int y, GameObject[,] grid)
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

                SpriteRenderer neighborRenderer = grid[neighborX, neighborY].GetComponent<SpriteRenderer>();
                if (neighborRenderer.color == Color.white)
                {
                    return true;
                }
            }
        }

        return false; 
    }

    private void SetCellColor(GameObject cell, Color color)
    {
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
        renderer.color = color;
    }
}
