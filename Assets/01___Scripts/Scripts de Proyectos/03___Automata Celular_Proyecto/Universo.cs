using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Universo : MonoBehaviour
{
    public TMP_InputField widthInput; 
    public TMP_InputField heightInput;
    public Button createSpaceButton; 
    public Button createStarsButton; 
    public Button createMeteoritesButton; 
    public Button createPlanetsButton; 
    public Button createMoonsButton;
    public Button createVidaButton;
    public Button RevisarVidaButton;
    public Button universeTimeButton; 

    // Referencias a los scripts
    public Space spaceScript;
    public Stars starsScript;
    public Meteorites meteoritesScript;
    public Planets planetsScript; 
    public Moons moonsScript;
    public Vida_Celula vidaScript;
    public VidaLuna_Celula vidaLunaScript;
    public UniverseTime universeTimeScript; 


    private void Start()
    {
        createSpaceButton.onClick.AddListener(CreateSpace);
        createStarsButton.onClick.AddListener(CreateStars);
        createMeteoritesButton.onClick.AddListener(CreateMeteorites);
        createPlanetsButton.onClick.AddListener(CreatePlanets);
        createMoonsButton.onClick.AddListener(CreateMoons);
        createVidaButton.onClick.AddListener(CreateVida);
        RevisarVidaButton.onClick.AddListener(RevisarVida);
        universeTimeButton.onClick.AddListener(ExecuteUniverseTime);

        createStarsButton.interactable = false;
        createMeteoritesButton.interactable = false;
        createPlanetsButton.interactable = false;
        createMoonsButton.interactable = false;
        createVidaButton.interactable= false;
        RevisarVidaButton.interactable = false;
    }

    private void CreateSpace()
    {
        int width = Mathf.Max(int.Parse(widthInput.text), 148);
        int height = Mathf.Max(int.Parse(heightInput.text), 148);

        spaceScript.CreateSpace(width, height);
        createSpaceButton.interactable = false;

    }

    private void CreateStars()
    {
        starsScript.CreateStars();
        createStarsButton.interactable = false;

    }

    private void CreateMeteorites()
    {
        meteoritesScript.CreateMeteorites();
        createMeteoritesButton.interactable = false;

    }

    private void CreatePlanets()
    {
        planetsScript.StartPlanetExpansion(); 
        createPlanetsButton.interactable = false;
    }

    private void CreateMoons()
    {
        moonsScript.StartMoonExpansion();  
        createMoonsButton.interactable = false;
    }


    private void CreateVida() 
    {
        vidaScript.GenerarAgua();
        vidaScript.GenerarPasto();
        vidaLunaScript.GenerarAgua();
        createVidaButton.interactable = false;
        
    }

    private void RevisarVida()
    {
        vidaScript.IniciarRevisarVida();
        
        vidaLunaScript.IniciarRevisarVida();

    }

    private void ExecuteUniverseTime()
    {
        StartCoroutine(universeTimeScript.ExecuteUniverseTimeCoroutine());
    }
}
