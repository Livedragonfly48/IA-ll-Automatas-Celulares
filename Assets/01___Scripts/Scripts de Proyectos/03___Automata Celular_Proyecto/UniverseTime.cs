using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UniverseTime : MonoBehaviour
{
    public Button universeTimeButton; 
    public Stars starsScript;
    public Meteorites meteoritesScript;

    private void Start()
    {
       
    }

    public IEnumerator ExecuteUniverseTimeCoroutine()
    {
        universeTimeButton.interactable = false;
        starsScript.MaintainStars();

        yield return new WaitForSeconds(0.5f); 


        meteoritesScript.MaintainMeteorites();
        universeTimeButton.interactable = true;
    }
}
