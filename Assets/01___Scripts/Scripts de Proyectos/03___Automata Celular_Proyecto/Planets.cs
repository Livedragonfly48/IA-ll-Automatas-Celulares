using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Planets : MonoBehaviour
{
    public TextMeshProUGUI smallPlanetText;
    public TextMeshProUGUI mediumPlanetText;
    public TextMeshProUGUI largePlanetText;

    public int smallPlanetCount;
    public int mediumPlanetCount;
    public int largePlanetCount;

    private bool isPaintingEnabled = false;
    public Space spaceScript;  
    public Universo universeScript;
    public Moons moonsScript;
  
    private int currentPlanetSize = 1;  

    public HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();  
    public bool isGeneratingPlanets = false;  

    public void Initialize(Space spaceScript)
    {
        this.spaceScript = spaceScript;
    }

    private void Start()
    {
        SetTextTransparency(0.2f);
    }

    public void UpdatePlanetCounts(int width, int height)
    {
        smallPlanetCount = 0;
        mediumPlanetCount = 0;
        largePlanetCount = 0;

        if (width < 248 && height < 248)
            smallPlanetCount = 3;
        else if (width < 348 && height < 348)
        {
            smallPlanetCount = 3;
            mediumPlanetCount = 1;
        }
        else if (width < 448 && height < 448)
        {
            smallPlanetCount = 2;
            mediumPlanetCount = 1;
            largePlanetCount = 1;
        }
        else if (width < 481 && height < 481)
        {
            smallPlanetCount = 2;
            mediumPlanetCount = 3;
            largePlanetCount = 1;
        }

        smallPlanetText.text = smallPlanetCount.ToString();
        mediumPlanetText.text = mediumPlanetCount.ToString();
        largePlanetText.text = largePlanetCount.ToString();

    }

    public void EnablePlanetPainting()
    {
        isPaintingEnabled = true;
        SetTextTransparency(1f);
    }

    private void Update()
    {
        if (isPaintingEnabled && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(mousePos.x);
            int y = Mathf.RoundToInt(mousePos.y);

            if (x >= 0 && x < spaceScript.width && y >= 0 && y < spaceScript.height)
            {
                Vector2Int position = new Vector2Int(x, y);

                if (CanPlacePlanet(position, currentPlanetSize))
                {
                    GameObject cell = spaceScript.grid[x, y];
                    if (cell != null)
                    {
                        PaintPlanet(cell);
                        BlockInfluenceArea(position, currentPlanetSize);  
                    }
                }
            }
        }
    }

    private bool CanPlacePlanet(Vector2Int center, int planetSize)
    {
        int expansionRadius = GetExpansionRadius(planetSize);

        for (int dx = -expansionRadius; dx <= expansionRadius; dx++)
        {
            for (int dy = -expansionRadius; dy <= expansionRadius; dy++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);
                if (occupiedCells.Contains(pos))
                {
                    return false;  
                }
            }
        }

        return true;
    }

    private void PaintPlanet(GameObject cell)
    {
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

        if (currentPlanetSize == 1 && smallPlanetCount > 0)
        {
            renderer.color = new Color(0.82f, 0.41f, 0.12f);  
            smallPlanetCount--;
            smallPlanetText.text = smallPlanetCount.ToString();
            if (smallPlanetCount == 0) currentPlanetSize = 2;  
        }
        else if (currentPlanetSize == 2 && mediumPlanetCount > 0)
        {
            renderer.color = new Color(0.64f, 0.32f, 0.17f);  
            mediumPlanetCount--;
            mediumPlanetText.text = mediumPlanetCount.ToString();
            if (mediumPlanetCount == 0) currentPlanetSize = 3;  
        }
        else if (currentPlanetSize == 3 && largePlanetCount > 0)
        {
            renderer.color = new Color(0.36f, 0.25f, 0.20f);  
            largePlanetCount--;
            largePlanetText.text = largePlanetCount.ToString();
        }

        if (smallPlanetCount == 0 && mediumPlanetCount == 0 && largePlanetCount == 0)
        {
            isPaintingEnabled = false;
            universeScript.createPlanetsButton.interactable = true;
            SetTextTransparency(0.2f);
        }
    }

    private void BlockInfluenceArea(Vector2Int center, int planetSize)
    {
        int expansionRadius = GetExpansionRadius(planetSize);

        for (int dx = -expansionRadius; dx <= expansionRadius; dx++)
        {
            for (int dy = -expansionRadius; dy <= expansionRadius; dy++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);

                if (pos.x >= 0 && pos.x < spaceScript.width && pos.y >= 0 && pos.y < spaceScript.height)
                {
                    occupiedCells.Add(pos);  
                }
            }
        }
    }

    private int GetExpansionRadius(int planetSize)
    {
        if (planetSize == 1) return 20;
        else if (planetSize == 2) return 40;
        else return 60;
    }

    private void SetTextTransparency(float alphaValue)
    {
        smallPlanetText.color = new Color(smallPlanetText.color.r, smallPlanetText.color.g, smallPlanetText.color.b, alphaValue);
        mediumPlanetText.color = new Color(mediumPlanetText.color.r, mediumPlanetText.color.g, mediumPlanetText.color.b, alphaValue);
        largePlanetText.color = new Color(largePlanetText.color.r, largePlanetText.color.g, largePlanetText.color.b, alphaValue);
    }

    public void StartPlanetExpansion()
    {
        isGeneratingPlanets = true; 
        StartCoroutine(ExpandPlanets());
    }

    private IEnumerator ExpandPlanets()
    {
        List<Vector2Int> planetPositions = new List<Vector2Int>();
        for (int x = 0; x < spaceScript.width; x++)
        {
            for (int y = 0; y < spaceScript.height; y++)
            {
                GameObject cell = spaceScript.grid[x, y];
                if (cell != null)
                {
                    SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

                    if (renderer.color == new Color(0.82f, 0.41f, 0.12f) || renderer.color == new Color(0.64f, 0.32f, 0.17f) || renderer.color == new Color(0.36f, 0.25f, 0.20f))
                    {
                        planetPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        foreach (var position in planetPositions)
        {
            GameObject planetCell = spaceScript.grid[position.x, position.y];
            if (planetCell != null)
            {
                SpriteRenderer planetRenderer = planetCell.GetComponent<SpriteRenderer>();        
                StartCoroutine(ExpandPlanet(position.x, position.y, planetRenderer));
            }
        }

        yield return new WaitForSeconds(0.1f);
        isGeneratingPlanets = false;  
   
    }

    private IEnumerator ExpandPlanet(int x, int y, SpriteRenderer renderer)
    {
        int expansionDistance = 0;
        int chunkSize = 20; 

        Color planetColor = renderer.color;

        if (planetColor == new Color(0.82f, 0.41f, 0.12f))
        {
            expansionDistance = 15; 
            chunkSize = 20;          
        }
        else if (planetColor == new Color(0.64f, 0.32f, 0.17f))
        {
            expansionDistance = 25;  
            chunkSize = 70;         
        }
        else if (planetColor == new Color(0.36f, 0.25f, 0.20f))
        {
            expansionDistance = 48; 
            chunkSize = 120;       
        }

        List<Vector2Int> expandedCells = new List<Vector2Int>();  
        for (int dx = -expansionDistance; dx <= expansionDistance; dx++)
        {
            for (int dy = -expansionDistance; dy <= expansionDistance; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < spaceScript.width && ny >= 0 && ny < spaceScript.height)
                {
                    GameObject neighborCell = spaceScript.grid[nx, ny];
                    if (neighborCell != null)
                    {
                        SpriteRenderer neighborRenderer = neighborCell.GetComponent<SpriteRenderer>();

                        float distance = Mathf.Sqrt(dx * dx + dy * dy);

                        if (distance <= expansionDistance)
                        {
                            if (neighborRenderer.color == Color.black || neighborRenderer.color == Color.white)
                            {
                                expandedCells.Add(new Vector2Int(nx, ny)); 
                            }

                            if (neighborRenderer.color == Color.yellow)
                            {
                                neighborRenderer.color = Color.yellow; 
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < expandedCells.Count; i += chunkSize)
        {
            for (int j = i; j < Mathf.Min(i + chunkSize, expandedCells.Count); j++)
            {
                Vector2Int expandedCell = expandedCells[j];
                GameObject neighborCell = spaceScript.grid[expandedCell.x, expandedCell.y];
                if (neighborCell != null)
                {
                    SpriteRenderer neighborRenderer = neighborCell.GetComponent<SpriteRenderer>();

                    neighborRenderer.color = renderer.color;
                }
            }

            yield return new WaitForSeconds(0.01f);
            moonsScript.EnableMoonPainting();
        }
    }
}
