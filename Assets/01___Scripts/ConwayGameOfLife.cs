using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConwayGameOfLife : MonoBehaviour
{
    public GameObject cellPrefab;
    public TextMeshProUGUI pauseText;
    public Button startButton;

    public int width;
    public int height;
    public float updateInterval;

    public Image imageHud; 
    public TMP_InputField inputField; 
    public TMP_InputField widthInputField; 
    public TMP_InputField heightInputField; 
    
    private bool isPaused = false;
    private bool[,] currentState;
    private GameObject[,] cells;

    private void Start()
    {
        startButton.onClick.AddListener(InitializeGrid);
        pauseText.gameObject.SetActive(false); 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (cells == null) return;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            int x = Mathf.FloorToInt(mousePos.x);
            int y = Mathf.FloorToInt(mousePos.y);

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                currentState[x, y] = !currentState[x, y];
                UpdateCellColor(x, y);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;

            pauseText.gameObject.SetActive(isPaused);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Application.Quit();
        }
    }

    private void InitializeGrid()
    {     
        if (float.TryParse(inputField.text, out float interval) && interval > 0 && interval < 11)
        {
            updateInterval = interval;
         
            if (int.TryParse(widthInputField.text, out int newWidth) && newWidth > 0 && newWidth < 49 &&
                int.TryParse(heightInputField.text, out int newHeight) && newHeight > 0 && newHeight < 49)
            {
                width = newWidth;
                height = newHeight;
            
                inputField.gameObject.SetActive(false);
                widthInputField.gameObject.SetActive(false);
                heightInputField.gameObject.SetActive(false);
                imageHud.gameObject.SetActive(false);

                currentState = new bool[width, height];
                cells = new GameObject[width, height];
                CreateGrid();
          
                Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);
                Camera.main.orthographicSize = Mathf.Max(width, height) / 2f; 

                StartCoroutine(UpdateGrid());
                startButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Asegúrate de que Width y Height estén entre 1 y 48.");
            }
        }
        else
        {
            Debug.LogWarning("Asegúrate de que el intervalo de actualización esté entre 1 y 10.");
        }
    }

    private void CreateGrid()
    {
        float cellSize = 1.0f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
                cell.transform.parent = transform;
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1);
                cells[x, y] = cell;

                UpdateCellColor(x, y);
            }
        }
    }

    private IEnumerator UpdateGrid()
    {
        while (true)
        {
            if (!isPaused)
            {
                yield return new WaitForSeconds(updateInterval);
          
                if (isPaused)
                    continue;

                bool[,] nextState = new bool[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        nextState[x, y] = GetNextState(x, y);
                    }
                }
                currentState = nextState;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        UpdateCellColor(x, y);
                    }
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private bool GetNextState(int x, int y)
    {
        int aliveNeighbors = GetAliveNeighbors(x, y);
        if (currentState[x, y])
        {
            return aliveNeighbors == 2 || aliveNeighbors == 3;
        }
        else
        {
            return aliveNeighbors == 3;
        }
    }

    private int GetAliveNeighbors(int x, int y)
    {
        int aliveCount = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    if (currentState[newX, newY]) aliveCount++;
                }
            }
        }
        return aliveCount;
    }

    private void UpdateCellColor(int x, int y)
    {
        Color color = currentState[x, y] ? Color.black : Color.white;
        cells[x, y].GetComponent<SpriteRenderer>().color = color;
    }

}
