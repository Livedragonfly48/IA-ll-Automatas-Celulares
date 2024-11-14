using UnityEngine;
using System.Collections;

public class Vida_Celula : MonoBehaviour
{
    public Space spaceScript; 
    public Universo universeScript;
    private const float probability = 0.048f;  

    Color aguaPequeño = new Color(0.6f, 0.2f, 0.1f);  
    Color aguaMediano = new Color(0.2f, 0.6f, 0.8f);  
    Color aguaGrande = new Color(0.2f, 0.4f, 0.6f);  

    Color pastoPequeño = new Color(0.8f, 0.3f, 0.2f); 
    Color pastoMediano = new Color(0.5f, 0.7f, 0.9f);  
    Color pastoGrande = new Color(0.1f, 0.3f, 0.1f); 

    public void IniciarRevisarVida()
    {
        RevisarVidaCoroutine();
    }

    private void RevisarVidaCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            RevisarVidaAgua();  
            RevisarVidaPasto();  
            GenerarAgua();
            GenerarPasto();

        }

        universeScript.RevisarVidaButton.interactable = true;
    }


    public void GenerarAgua()
    {
        for (int x = 0; x < spaceScript.width; x++)
        {
            for (int y = 0; y < spaceScript.height; y++)
            {
                GameObject cell = spaceScript.grid[x, y];
                if (cell != null)
                {
                    SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

                    Color planetaColor = ObtenerColorPlaneta(x, y);

                    if (ContarVecinasDePlaneta(x, y) == 8 && Random.value < probability)
                    {
                        if (planetaColor == new Color(0.82f, 0.41f, 0.12f))  
                            renderer.color = aguaPequeño;
                        else if (planetaColor == new Color(0.64f, 0.32f, 0.17f))  
                            renderer.color = aguaMediano;
                        else if (planetaColor == new Color(0.36f, 0.25f, 0.20f)) 
                            renderer.color = aguaGrande;
                    }
                }
            }

            universeScript.RevisarVidaButton.interactable = true;
        }
    }


    public void GenerarPasto()
    {
        for (int x = 0; x < spaceScript.width; x++)
        {
            for (int y = 0; y < spaceScript.height; y++)
            {
                GameObject cell = spaceScript.grid[x, y];
                if (cell != null)
                {
                    SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

                    Color planetaColor = ObtenerColorPlaneta(x, y);

                    if (ContarVecinasColor(x, y, ObtenerAguaColorPorPlaneta(x, y)) >= 1 && renderer.color != ObtenerPastoColorPorPlaneta(x, y))
                    {
                        if (Random.value < probability)  
                        {
                            if (planetaColor == new Color(0.82f, 0.41f, 0.12f)) 
                                renderer.color = pastoPequeño;
                            else if (planetaColor == new Color(0.64f, 0.32f, 0.17f)) 
                                renderer.color = pastoMediano;
                            else if (planetaColor == new Color(0.36f, 0.25f, 0.20f)) 
                                renderer.color = pastoGrande;
                        }
                    }
                }
            }
        }
    }

    public void RevisarVidaAgua()
    {
        for (int x = 0; x < spaceScript.width; x++)
        {
            for (int y = 0; y < spaceScript.height; y++)
            {
                GameObject cell = spaceScript.grid[x, y];
                if (cell != null)
                {
                    SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

                    int aguaNeighbors = ContarVecinasColor(x, y, ObtenerAguaColorPorPlaneta(x, y));

                    if (renderer.color == ObtenerAguaColorPorPlaneta(x, y))  
                    {
                        if (aguaNeighbors >= 5)  
                        {
                            renderer.color = ObtenerAguaColorPorPlaneta(x, y); 
                        }
                        else
                        {
                            renderer.color = ObtenerPastoColorPorPlaneta(x, y); 
                        }
                    }
                    else if (aguaNeighbors == 4)  
                    {
                        renderer.color = ObtenerPastoColorPorPlaneta(x, y);  
                    }
                }
            }
        }
    }

    public void RevisarVidaPasto()
    {
        for (int x = 0; x < spaceScript.width; x++)
        {
            for (int y = 0; y < spaceScript.height; y++)
            {
                GameObject cell = spaceScript.grid[x, y];
                if (cell != null)
                {
                    SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

                    if (renderer.color == ObtenerPastoColorPorPlaneta(x, y))
                    {
                        int pastoNeighbors = ContarVecinasColor(x, y, ObtenerPastoColorPorPlaneta(x, y));
                        Debug.Log($"Pasto en ({x},{y}) tiene {pastoNeighbors} vecinos de pasto.");

                        if (pastoNeighbors >= 4)
                        {
                            Debug.Log($"Convertir celda en ({x},{y}) de pasto a agua.");
                            renderer.color = ObtenerAguaColorPorPlaneta(x, y); 
                        }

                        else if (ContarVecinasColor(x, y, ObtenerAguaColorPorPlaneta(x, y)) >= 1 && Random.value < probability)
                        {
                            renderer.color = ObtenerPastoColorPorPlaneta(x, y);  
                        }
                    }
                }
            }
        }
    }

    private Color ObtenerAguaColorPorPlaneta(int x, int y)
    {
        Color planetaColor = ObtenerColorPlaneta(x, y);
        if (planetaColor == new Color(0.82f, 0.41f, 0.12f)) 
            return aguaPequeño;
        else if (planetaColor == new Color(0.64f, 0.32f, 0.17f)) 
            return aguaMediano;
        else  
            return aguaGrande;
    }

    private Color ObtenerPastoColorPorPlaneta(int x, int y)
    {
        Color planetaColor = ObtenerColorPlaneta(x, y);
        if (planetaColor == new Color(0.82f, 0.41f, 0.12f))  
            return pastoPequeño;
        else if (planetaColor == new Color(0.64f, 0.32f, 0.17f))  
            return pastoMediano;
        else  
            return pastoGrande;
    }

    private int ContarVecinasColor(int x, int y, Color color)
    {
        int cuenta = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < spaceScript.width && ny >= 0 && ny < spaceScript.height)
                {
                    GameObject neighborCell = spaceScript.grid[nx, ny];
                    if (neighborCell != null)
                    {
                        SpriteRenderer neighborRenderer = neighborCell.GetComponent<SpriteRenderer>();

                        if (neighborRenderer.color == color)
                        {
                            cuenta++;
                        }
                    }
                }
            }
        }

        return cuenta;
    }

    private Color ObtenerColorPlaneta(int x, int y)
    {
        GameObject cell = spaceScript.grid[x, y];
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();

        return renderer.color;
    }

    private int ContarVecinasDePlaneta(int x, int y)
    {
        int cuenta = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < spaceScript.width && ny >= 0 && ny < spaceScript.height)
                {
                    GameObject neighborCell = spaceScript.grid[nx, ny];
                    if (neighborCell != null)
                    {
                        SpriteRenderer neighborRenderer = neighborCell.GetComponent<SpriteRenderer>();

                        if (neighborRenderer.color == new Color(0.82f, 0.41f, 0.12f) ||
                            neighborRenderer.color == new Color(0.64f, 0.32f, 0.17f) ||
                            neighborRenderer.color == new Color(0.36f, 0.25f, 0.20f))
                        {
                            cuenta++;
                        }
                    }
                }
            }
        }

        return cuenta;
    }
}