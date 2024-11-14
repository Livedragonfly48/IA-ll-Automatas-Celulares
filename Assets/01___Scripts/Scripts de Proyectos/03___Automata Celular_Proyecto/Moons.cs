using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Moons : MonoBehaviour
{
    public TextMeshProUGUI smallMoonText;
    public TextMeshProUGUI mediumMoonText;
    public TextMeshProUGUI largeMoonText;

    public int smallMoonCount;
    public int mediumMoonCount;
    public int largeMoonCount;

    private bool isPaintingEnabled = false;
    public Space spaceScript;
    public Universo universeScript;

    private int currentMoonSize = 1;
    public HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
    public bool isGeneratingMoons = false;

    public void Initialize(Space spaceScript)
    {
        this.spaceScript = spaceScript;
    }

    private void Start()
    {
        SetTextTransparency(0.2f);
    }

    public void UpdateMoonCounts(int width, int height)
    {
        smallMoonCount = 0;
        mediumMoonCount = 0;
        largeMoonCount = 0;

        if (width < 248 && height < 248)
            smallMoonCount = 5;
        else if (width < 348 && height < 348)
        {
            smallMoonCount = 5;
            mediumMoonCount = 2;
        }
        else if (width < 448 && height < 448)
        {
            smallMoonCount = 4;
            mediumMoonCount = 3;
            largeMoonCount = 1;
        }
        else if (width < 481 && height < 481)
        {
            smallMoonCount = 3;
            mediumMoonCount = 5;
            largeMoonCount = 2;
        }

        smallMoonText.text = smallMoonCount.ToString();
        mediumMoonText.text = mediumMoonCount.ToString();
        largeMoonText.text = largeMoonCount.ToString();
    }

    public void EnableMoonPainting()
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

                if (CanPlaceMoon(position, currentMoonSize))
                {
                    GameObject cell = spaceScript.grid[x, y];
                    if (cell != null)
                    {
                        PaintMoon(cell);
                        BlockInfluenceArea(position, currentMoonSize);
                    }
                }
            }
        }
    }

    private bool CanPlaceMoon(Vector2Int center, int moonSize)
    {
        int expansionRadius = GetExpansionRadius(moonSize);

        for (int dx = -expansionRadius; dx <= expansionRadius; dx++)
        {
            for (int dy = -expansionRadius; dy <= expansionRadius; dy++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);

                if (occupiedCells.Contains(pos) || IsCellOccupiedByPlanetOrOtherEntity(pos))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsCellOccupiedByPlanetOrOtherEntity(Vector2Int position)
    {
        GameObject cell = spaceScript.grid[position.x, position.y];
        if (cell != null)
        {
            SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                if (renderer.color == new Color(0.82f, 0.41f, 0.12f) || renderer.color == new Color(0.64f, 0.32f, 0.17f) || renderer.color == new Color(0.36f, 0.25f, 0.20f)) 
                {
                    return true;
                }
                if (renderer.color == new Color(0.33f, 0.33f, 0.33f) || renderer.color == new Color(0.5f, 0.5f, 0.5f) || renderer.color == new Color(0.75f, 0.75f, 0.75f))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void PaintMoon(GameObject cell)
    {
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

        if (currentMoonSize == 1 && smallMoonCount > 0)
        {
            renderer.color = new Color(0.33f, 0.33f, 0.33f); 
            smallMoonCount--;
            smallMoonText.text = smallMoonCount.ToString();
            if (smallMoonCount == 0) currentMoonSize = 2;
        }
        else if (currentMoonSize == 2 && mediumMoonCount > 0)
        {
            renderer.color = new Color(0.5f, 0.5f, 0.5f); 
            mediumMoonCount--;
            mediumMoonText.text = mediumMoonCount.ToString();
            if (mediumMoonCount == 0) currentMoonSize = 3;
        }
        else if (currentMoonSize == 3 && largeMoonCount > 0)
        {
            renderer.color = new Color(0.75f, 0.75f, 0.75f);
            largeMoonCount--;
            largeMoonText.text = largeMoonCount.ToString();
        }

        if (smallMoonCount == 0 && mediumMoonCount == 0 && largeMoonCount == 0)
        {
            isPaintingEnabled = false;
            universeScript.createMoonsButton.interactable = true;
            SetTextTransparency(0.2f);
        }
    }

    private void BlockInfluenceArea(Vector2Int center, int moonSize)
    {
        int expansionRadius = GetExpansionRadius(moonSize);

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

    private int GetExpansionRadius(int moonSize)
    {
        if (moonSize == 1) return 3;
        else if (moonSize == 2) return 5;
        else return 8;
    }

    private void SetTextTransparency(float alphaValue)
    {
        smallMoonText.color = new Color(smallMoonText.color.r, smallMoonText.color.g, smallMoonText.color.b, alphaValue);
        mediumMoonText.color = new Color(mediumMoonText.color.r, mediumMoonText.color.g, mediumMoonText.color.b, alphaValue);
        largeMoonText.color = new Color(largeMoonText.color.r, largeMoonText.color.g, largeMoonText.color.b, alphaValue);
    }

    public void StartMoonExpansion()
    {
        isGeneratingMoons = true; 
        StartCoroutine(ExpandMoons());
    }

    private IEnumerator ExpandMoons()
    {
        List<Vector2Int> moonPositions = new List<Vector2Int>();
        for (int x = 0; x < spaceScript.width; x++)
        {
            for (int y = 0; y < spaceScript.height; y++)
            {
                GameObject cell = spaceScript.grid[x, y];
                if (cell != null)
                {
                    SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

                    if (renderer.color == new Color(0.33f, 0.33f, 0.33f) || renderer.color == new Color(0.5f, 0.5f, 0.5f) || renderer.color == new Color(0.75f, 0.75f, 0.75f))
                    {
                        moonPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        foreach (var position in moonPositions)
        {
            GameObject moonCell = spaceScript.grid[position.x, position.y];
            if (moonCell != null)
            {
                SpriteRenderer moonRenderer = moonCell.GetComponent<SpriteRenderer>();
                StartCoroutine(ExpandMoon(position.x, position.y, moonRenderer));
            }
        }

        yield return new WaitForSeconds(0.1f);
        isGeneratingMoons = false;  
        universeScript.createVidaButton.interactable = true;
    }

    private IEnumerator ExpandMoon(int x, int y, SpriteRenderer renderer)
    {
        int expansionDistance = 0;
        int chunkSize = 20; 

        Color moonColor = renderer.color;

        if (moonColor == new Color(0.33f, 0.33f, 0.33f))
        {
            expansionDistance = 3; 
            chunkSize = 1;          
        }
        else if (moonColor == new Color(0.5f, 0.5f, 0.5f))
        {
            expansionDistance = 5;  
            chunkSize = 2;         
        }
        else if (moonColor == new Color(0.75f, 0.75f, 0.75f))  
        {
            expansionDistance = 8;  
            chunkSize = 3;        
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

                            if (neighborRenderer.color == new Color(0.82f, 0.41f, 0.12f) || neighborRenderer.color == new Color(0.64f, 0.32f, 0.17f) || neighborRenderer.color == new Color(0.36f, 0.25f, 0.20f))
                            {
                                continue;
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
            
        }
    }
}