using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pathfinding : MonoBehaviour
{
    public Space spaceScript;
    public Universo universeScript;
    public Button pathfindingButton;
    public Button generatePathButton;
    public TextMeshProUGUI costoTotalText; 
    public GameObject pathMarkerPrefab; 
    public GameController gameController;


    public Vector2Int startPos;
    public Vector2Int endPos;
    private List<Vector2Int> path = new List<Vector2Int>();
    private float totalPathCost = 0f;

    private void Start()
    {
        pathfindingButton.onClick.AddListener(IniciarPathfinding);
        generatePathButton.onClick.AddListener(GenerarCamino);
        generatePathButton.interactable = false;
    }



    private void IniciarPathfinding()
    {
        universeScript.RevisarVidaButton.interactable = false;
        startPos = ObtenerPosicionAleatoria();
        endPos = ObtenerPosicionAleatoria();
        MarcarCelda(startPos, Color.blue);
        MarcarCelda(endPos, Color.red);
        generatePathButton.interactable = true;
        costoTotalText.text = "";
    }

    private void GenerarCamino()
    {
        
        path = AStar(startPos, endPos);

        if (path.Count > 0)
        {
            DibujarCamino(path);
            gameController.startgame();
            
        }
        else
        {
            costoTotalText.text = "No hay camino disponible";
        }
    }


    private List<Vector2Int> AStar(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> openList = new List<Vector2Int>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        Dictionary<Vector2Int, float> gScores = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> fScores = new Dictionary<Vector2Int, float>();

        openList.Add(start);
        gScores[start] = 0;
        fScores[start] = Heuristica(start, end);

        while (openList.Count > 0)
        {
            Vector2Int current = ObtenerCeldaConMenorF(openList, fScores);

            if (current == end)
            {
                float costoFinal = gScores[end];

                Debug.Log("Costo Final: " + costoFinal);



                return ReconstruirCamino(cameFrom, current);
                
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Vector2Int vecino in ObtenerVecinos(current))
            {
                if (closedList.Contains(vecino)) continue;

                float tentativeGScore = gScores[current] + ObtenerCosto(vecino);

                if (!openList.Contains(vecino) || tentativeGScore < gScores[vecino])
                {
                    cameFrom[vecino] = current;
                    gScores[vecino] = tentativeGScore;
                    fScores[vecino] = tentativeGScore + Heuristica(vecino, end);

                    if (!openList.Contains(vecino))
                        openList.Add(vecino);
                }
            }
        }

        return new List<Vector2Int>(); 
    }

    private float Heuristica(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> ObtenerVecinos(Vector2Int current)
    {
        List<Vector2Int> vecinos = new List<Vector2Int>();

        Vector2Int[] direcciones = {
        new Vector2Int(0, 1), 
        new Vector2Int(1, 0),  
        new Vector2Int(0, -1), 
        new Vector2Int(-1, 0), 
        new Vector2Int(1, 1),  
        new Vector2Int(1, -1), 
        new Vector2Int(-1, 1), 
        new Vector2Int(-1, -1) 
    };

        foreach (Vector2Int dir in direcciones)
        {
            Vector2Int vecino = current + dir;
            if (EsValida(vecino))
            {
                vecinos.Add(vecino);
            }
        }

        return vecinos;
    }

    private bool EsValida(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < spaceScript.width && cell.y >= 0 && cell.y < spaceScript.height;
    }

    private float ObtenerCosto(Vector2Int cell)
    {
        GameObject c = spaceScript.grid[cell.x, cell.y];
        if (c != null)
        {
            SpriteRenderer renderer = c.GetComponent<SpriteRenderer>();
            Color color = renderer.color;

            float costo = 0;

            if (color == Color.black || color == Color.white)
                costo = 1;
            else if (color == new Color(0.545f, 0.271f, 0.074f))
                costo = 3;
            else if (EsColorPlaneta(color))
                costo = 4;
            else if (EsColorLuna(color)) 
                costo = 3;
            else if (color == new Color(0.82f, 0.41f, 0.12f)) 
                costo = 4;
            else if (color == new Color(0.8f, 0.3f, 0.2f)) 
                costo = 4;
            else if (color == new Color(0.64f, 0.32f, 0.17f)) 
                costo = 4;
            else if (color == new Color(0.2f, 0.6f, 0.8f))
                costo = 4;
            else if (color == new Color(0.5f, 0.7f, 0.9f)) 
                costo = 4;
            else if (color == new Color(0.2f, 0.4f, 0.6f)) 
                costo = 4;
            else if (color == new Color(0.1f, 0.3f, 0.1f)) 
                costo = 4;
            else if (color == new Color(0.36f, 0.25f, 0.20f)) 
                costo = 4;

            else if (color == new Color(0.4f, 0.4f, 0.4f))
                costo = 3;
            else if (color == new Color(0.7f, 0.7f, 0.7f)) 
                costo = 3;
            else if (color == new Color(0.33f, 0.33f, 0.33f))
                costo = 3;
            else if (color == new Color(0.6f, 0.6f, 0.6f)) 
                costo = 3;
            else if (color == new Color(0.5f, 0.5f, 0.5f))
                costo = 3;
            else if (color == new Color(0.75f, 0.75f, 0.75f)) 
                costo = 3;

            else
                costo = 0;

            Debug.Log($"Color de la celda {cell}: {color}, Costo: {costo}");

            return costo;
        }
        return 1;
    }

    public bool EsColorPlaneta(Color color)
    {
        return color == new Color(0.6f, 0.2f, 0.1f) || color == new Color(0.8f, 0.3f, 0.2f) || color == new Color(0.82f, 0.41f, 0.12f) ||
               color == new Color(0.2f, 0.6f, 0.8f) || color == new Color(0.5f, 0.7f, 0.9f) || color == new Color(0.64f, 0.32f, 0.17f) ||
               color == new Color(0.2f, 0.4f, 0.6f) || color == new Color(0.1f, 0.3f, 0.1f) || color == new Color(0.36f, 0.25f, 0.20f);
    }

    public bool EsColorLuna(Color color)
    {
        return color == new Color(0.4f, 0.4f, 0.4f) || color == new Color(0.7f, 0.7f, 0.7f) || color == new Color(0.33f, 0.33f, 0.33f) ||
               color == new Color(0.6f, 0.6f, 0.6f) || color == new Color(0.5f, 0.5f, 0.5f) ||
               color == new Color(0.2f, 0.2f, 0.2f) || color == new Color(0.3f, 0.3f, 0.3f) || color == new Color(0.75f, 0.75f, 0.75f);
    }

    private List<Vector2Int> ReconstruirCamino(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> totalPath = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        return totalPath;
    }

    private Vector2Int ObtenerCeldaConMenorF(List<Vector2Int> openList, Dictionary<Vector2Int, float> fScores)
    {
        Vector2Int minCell = openList[0];
        foreach (Vector2Int cell in openList)
        {
            if (fScores[cell] < fScores[minCell])
            {
                minCell = cell;
            }
        }
        return minCell;
    }

    private Vector2Int ObtenerPosicionAleatoria()
    {
        int x = Random.Range(0, spaceScript.width);
        int y = Random.Range(0, spaceScript.height);
        return new Vector2Int(x, y);
    }

    private void MarcarCelda(Vector2Int posicion, Color color)
    {
        GameObject celda = spaceScript.grid[posicion.x, posicion.y];
        if (celda != null)
        {
            celda.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void DibujarCamino(List<Vector2Int> camino)
    {

        foreach (Vector2Int cell in camino)
        {
            GameObject pathMarker = Instantiate(pathMarkerPrefab, new Vector3(cell.x, cell.y, 0), Quaternion.identity);
            pathMarker.transform.SetParent(transform); 
            costoTotalText.text = "Costo: " + totalPathCost.ToString("F2");
        }

    }

}