using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;  
    public Camera mainCamera;      
    public Slider zoomSlider;     

    private void Start()
    {
        if (zoomSlider != null)
        {
            zoomSlider.onValueChanged.AddListener(OnZoomSliderChange);
            zoomSlider.value = mainCamera.orthographicSize; 
        }
    }

    private void Update()
    {
        Vector3 position = transform.position; 

        if (Input.GetKey(KeyCode.A))
        {
            position.x -= moveSpeed * Time.deltaTime; 
        }
        if (Input.GetKey(KeyCode.D))
        {
            position.x += moveSpeed * Time.deltaTime; 
        }

        if (Input.GetKey(KeyCode.W))
        {
            position.y += moveSpeed * Time.deltaTime; 
        }
        if (Input.GetKey(KeyCode.S))
        {
            position.y -= moveSpeed * Time.deltaTime; 
        }

        transform.position = position; 

        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scrollData * 2f; 
        Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 1); 
    }


    private void OnZoomSliderChange(float value)
    {
        mainCamera.orthographicSize = value; 
    }
}
