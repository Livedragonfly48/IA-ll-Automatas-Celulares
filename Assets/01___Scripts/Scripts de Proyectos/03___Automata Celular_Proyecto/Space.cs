using UnityEngine;
using System.Collections;

public class Space : MonoBehaviour
{
    public GameObject cellSpacePrefab; 
    public Universo universeScript;
    public int width; 
    public int height; 

    public GameObject[,] grid;
    private bool isGenerating = false; 

    public void CreateSpace(int width, int height)
    {
        this.width = width;
        this.height = height;

        if (grid != null)
        {
            foreach (var cell in grid)
            {
                Destroy(cell);
            }
        }

        grid = new GameObject[width, height];
        StartCoroutine(GenerateSpaceInChunks());
    }

    private IEnumerator GenerateSpaceInChunks()
    {
        isGenerating = true; 

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
                    GameObject cell = Instantiate(cellSpacePrefab, new Vector3(x, y, 0), Quaternion.identity);
                    cell.transform.SetParent(transform);
                    grid[x, y] = cell; 

                    SetCellColor(cell, 0); 
                }
            }

            yield return null;
        }

        isGenerating = false; 

        universeScript.createStarsButton.interactable = true;
        FindFirstObjectByType<Planets>().UpdatePlanetCounts(width, height);
        FindFirstObjectByType<Moons>().UpdateMoonCounts(width, height);
    }

    // Cambiar el color de la celda
    private void SetCellColor(GameObject cell, int value)
    {
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
        renderer.color = value == 1 ? Color.white : Color.black;
    }
}
