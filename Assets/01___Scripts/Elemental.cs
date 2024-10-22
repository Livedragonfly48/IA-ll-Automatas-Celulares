using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro para UI
using UnityEngine.UI;

public class CellularAutomaton : MonoBehaviour
{
    public TMP_InputField ruleInput;
    public TMP_InputField rowsInput;
    public TMP_InputField cellsPerRowInput;
    public Button generateFirstRowButton;
    public Button startButton;
    public Toggle randomToggle;
    public Toggle steppedToggle;
    public GameObject cellPrefab; 
    public Transform gridParent;  

    private int rule; 
    private int[] ruleBinary = new int[8]; 
    private int rows;
    private int cellsPerRow;
    private List<GameObject[]> grid = new List<GameObject[]>();



    private void Start()
    {
        generateFirstRowButton.onClick.AddListener(GenerateFirstRow);
        startButton.onClick.AddListener(StartSimulation);
        startButton.gameObject.SetActive(false); 
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }


    private void GenerateFirstRow()
    {
   
        if (!int.TryParse(ruleInput.text, out rule) || !int.TryParse(rowsInput.text, out rows) || !int.TryParse(cellsPerRowInput.text, out cellsPerRow) || rule < 0 || rule > 255 || rows < 1 || rows > 148 || cellsPerRow < 1 || cellsPerRow > 148)
        {
            Debug.Log("Valores de entrada no válidos.");
            return;
        }
      
        ConvertRuleToBinary();

        GenerateRow(0);


       
        generateFirstRowButton.gameObject.SetActive(false);
        ruleInput.gameObject.SetActive(false);
        rowsInput.gameObject.SetActive(false);
        cellsPerRowInput.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
      

        if (randomToggle.isOn)
        {
            RandomizeFirstRow();
        }
    }

    private void ConvertRuleToBinary()
    {
        string binaryString = System.Convert.ToString(rule, 2).PadLeft(8, '0');
        for (int i = 0; i < 8; i++)
        {
            ruleBinary[i] = binaryString[i] - '0';
        }
        Debug.Log("Regla en binario: " + string.Join("", ruleBinary)); 
    }


    private void GenerateRow(int rowIndex)
    {
        GameObject[] row = new GameObject[cellsPerRow];
        for (int i = 0; i < cellsPerRow; i++)
        {
            GameObject cell = Instantiate(cellPrefab, gridParent);
            row[i] = cell;

            float xPos = i; 
            float yPos = -rowIndex; 
            cell.transform.position = new Vector3(xPos, yPos, 0); 

            if (rowIndex == 0)
            {
                cell.AddComponent<CellClickHandler>().SetParent(this);
            }
        }
        grid.Add(row);
    }

    public void ToggleCellState(GameObject cell)
    {
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
        if (renderer.color == Color.white)
        {
            renderer.color = Color.black; 
        }
        else
        {
            renderer.color = Color.white; 
        }
    }

    private void RandomizeFirstRow()
    {
        foreach (GameObject cell in grid[0])
        {
            bool isAlive = Random.value > 0.5f;
            SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
            renderer.color = isAlive ? Color.black : Color.white;
        }
    }


    private void StartSimulation()
    {
        startButton.gameObject.SetActive(false); 
        StartCoroutine(GenerateAllRows());
    }

    private IEnumerator GenerateAllRows()
    {
        for (int i = 1; i < rows; i++)
        {
            GenerateRow(i);
            CalculateNextRow(i - 1, i); 
            if (steppedToggle.isOn)
            {
                yield return new WaitForSeconds(0.3f); 
            }
        }
    }



    private void CalculateNextRow(int previousRowIndex, int currentRowIndex)
    {
        for (int i = 0; i < cellsPerRow; i++)
        {
            int left = (i == 0) ? grid[previousRowIndex][cellsPerRow - 1].GetComponent<SpriteRenderer>().color == Color.black ? 1 : 0 : grid[previousRowIndex][i - 1].GetComponent<SpriteRenderer>().color == Color.black ? 1 : 0;
            int center = grid[previousRowIndex][i].GetComponent<SpriteRenderer>().color == Color.black ? 1 : 0;
            int right = (i == cellsPerRow - 1) ? grid[previousRowIndex][0].GetComponent<SpriteRenderer>().color == Color.black ? 1 : 0 : grid[previousRowIndex][i + 1].GetComponent<SpriteRenderer>().color == Color.black ? 1 : 0;

            string pattern = "" + left + center + right; 
            int patternIndex = System.Convert.ToInt32(pattern, 2);

            int ruleIndex = 7 - patternIndex; 

            Debug.Log($"Patrón: {pattern}, Índice: {ruleIndex}, Valor: {ruleBinary[ruleIndex]}");

            grid[currentRowIndex][i].GetComponent<SpriteRenderer>().color = (ruleBinary[ruleIndex] == 1) ? Color.black : Color.white;
        }
    }


    private void ResetGame()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        grid.Clear();
        ruleInput.gameObject.SetActive(true);
        rowsInput.gameObject.SetActive(true);
        cellsPerRowInput.gameObject.SetActive(true);
        generateFirstRowButton.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);
    }
}

public class CellClickHandler : MonoBehaviour
{
    private CellularAutomaton parent;

    public void SetParent(CellularAutomaton parent)
    {
        this.parent = parent;
    }

    private void OnMouseDown()
    {
        parent.ToggleCellState(gameObject);
    }
}
