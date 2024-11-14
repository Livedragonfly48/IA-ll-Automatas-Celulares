using UnityEngine;

public class VidaLuna_Celula : MonoBehaviour
{
    public Space spaceScript;  
    public Universo universeScript;
    private const float probability = 0.48f;

    Color aguaPequeño = new Color(0.4f, 0.4f, 0.4f);
    Color aguaMediano = new Color(0.6f, 0.6f, 0.6f); 
    Color aguaGrande = new Color(0.2f, 0.2f, 0.2f);  

    Color pastoPequeño = new Color(0.7f, 0.7f, 0.7f);  
    Color pastoMediano = new Color(0.5f, 0.5f, 0.5f);  
    Color pastoGrande = new Color(0.3f, 0.3f, 0.3f);  

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

                    // Determinar el color del planeta
                    Color planetaColor = ObtenerColorPlaneta(x, y);

                    if (ContarVecinasDePlaneta(x, y) == 8 && Random.value < probability)
                    {
                        if (planetaColor == new Color(0.33f, 0.33f, 0.33f))  
                            renderer.color = aguaPequeño;
                        else if (planetaColor == new Color(0.5f, 0.5f, 0.5f)) 
                            renderer.color = aguaMediano;
                        else if (planetaColor == new Color(0.75f, 0.75f, 0.75f))  
                            renderer.color = aguaGrande;
                    }
                }
            }

            universeScript.RevisarVidaButton.interactable = true;
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
                        if (aguaNeighbors >= 2) 
                        {
                            renderer.color = ObtenerAguaColorPorPlaneta(x, y); 
                        }
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

                    Color planetaColor = ObtenerColorPlaneta(x, y);

                    if (renderer.color == ObtenerPastoColorPorPlaneta(x, y))  
                    {
                        if (ContarVecinasColor(x, y, ObtenerAguaColorPorPlaneta(x, y)) >= 4)
                        {
                            renderer.color = ObtenerAguaColorPorPlaneta(x, y);  
                        }
                    }

                    if (ContarVecinasColor(x, y, ObtenerAguaColorPorPlaneta(x, y)) >= 1 && Random.value < probability)
                    {
                        renderer.color = ObtenerPastoColorPorPlaneta(x, y); 
                    }
                }
            }
        }
    }

    private Color ObtenerAguaColorPorPlaneta(int x, int y)
    {
        Color planetaColor = ObtenerColorPlaneta(x, y);
        if (planetaColor == new Color(0.33f, 0.33f, 0.33f))  
            return aguaPequeño;
        else if (planetaColor == new Color(0.5f, 0.5f, 0.5f)) 
            return aguaMediano;
        else  
            return aguaGrande;
    }

    private Color ObtenerPastoColorPorPlaneta(int x, int y)
    {
        Color planetaColor = ObtenerColorPlaneta(x, y);
        if (planetaColor == new Color(0.33f, 0.33f, 0.33f))  
            return pastoPequeño;
        else if (planetaColor == new Color(0.5f, 0.5f, 0.5f)) 
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

                        if (neighborRenderer.color == new Color(0.33f, 0.33f, 0.33f) ||
                            neighborRenderer.color == new Color(0.5f, 0.5f, 0.5f) ||
                            neighborRenderer.color == new Color(0.75f, 0.75f, 0.75f))
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