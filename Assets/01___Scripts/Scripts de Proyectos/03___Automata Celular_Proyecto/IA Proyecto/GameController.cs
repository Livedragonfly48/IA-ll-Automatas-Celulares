using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;       
    public GameObject pathMarkerPrefab;  
    public TMP_Text gameOverText;    
    public TMP_Text winText;         

    private Vector2Int currentPos;  
    private bool canMove = true;  
    private bool start;
    private Pathfinding pathfindingScript;  

    private void Start()
    {
        pathfindingScript = FindFirstObjectByType<Pathfinding>();
        winText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        winText.text = "";
        gameOverText.text = "";

    }

    public void startgame()
    {
        if (pathfindingScript != null)
        {
            Vector2Int startPos = pathfindingScript.startPos;
            Vector2Int endPos = pathfindingScript.endPos;

            currentPos = startPos;

            player = Instantiate(playerPrefab, new Vector3(startPos.x, startPos.y, 0), Quaternion.identity);
            canMove = true;
            start = true;

            player.transform.SetParent(transform); 

            winText.text = "";
            gameOverText.text = "";
        }
        else
        {
            Debug.LogError("No se encontró el script Pathfinding en la escena.");
        }
    }

    private void Update()
    {

        if (canMove && start)
        {
            if (Input.GetKeyDown(KeyCode.Y)) MovePlayer(Vector2Int.up);
            if (Input.GetKeyDown(KeyCode.H)) MovePlayer(Vector2Int.down);
            if (Input.GetKeyDown(KeyCode.G)) MovePlayer(Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.J)) MovePlayer(Vector2Int.right);
            if (Input.GetKeyDown(KeyCode.M)) MovePlayer(new Vector2Int(1, -1));
            if (Input.GetKeyDown(KeyCode.V)) MovePlayer(new Vector2Int(-1, -1));
            if (Input.GetKeyDown(KeyCode.U)) MovePlayer(new Vector2Int(1, 1));
            if (Input.GetKeyDown(KeyCode.T)) MovePlayer(new Vector2Int(-1, 1));
        }
            if (start)
        {
            Ganar(); 
        }
        

    }

    private void MovePlayer(Vector2Int direction)
    {
        Vector2Int nuevaPos = currentPos + direction;

        if (EsPosicionValida(nuevaPos))
        {
            currentPos = nuevaPos;
            player.transform.position = new Vector3(currentPos.x, currentPos.y, 0);
        }
        else
        {
            gameOverText.text = "¡Perdiste! Te saliste del camino.";
            gameOverText.gameObject.SetActive(true);
            Debug.Log("¡Perdiste! Te saliste del camino.");
        }
    }


    public void Ganar()
    {
        if (currentPos == pathfindingScript.endPos)
        {
            winText.text = "¡Ganaste! Has llegado al punto final.";
            winText.gameObject.SetActive(true);
            canMove = false;

        }
    }

private bool EsPosicionValida(Vector2Int pos)
    {
        Vector3 worldPos = new Vector3(pos.x, pos.y, 0);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.gameObject.CompareTag("CellPath")) 
        {
            return true;
        }

        return false;
    }
}
