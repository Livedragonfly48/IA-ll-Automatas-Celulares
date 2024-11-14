using UnityEngine;

public class CameraControllerAC : MonoBehaviour
{
    public Space spaceScript; 
    public float padding = 1.0f;
    public float zoomSpeed = 10f;  

    private Camera cam;

    void Start()
    {
        cam = Camera.main;    
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            cam.orthographicSize -= zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            cam.orthographicSize += zoomSpeed * Time.deltaTime;
        }   
        AjustarCamara();
    }

    void AjustarCamara()
    {
        float gridWidth = spaceScript.width;
        float gridHeight = spaceScript.height;

        float aspectRatio = cam.aspect; 

        if (aspectRatio > 1)  
        {
            cam.orthographicSize = (gridHeight / 2) + padding;
        }
        else  
        {
            cam.orthographicSize = (gridWidth / 2) / aspectRatio + padding;
        }
        Vector3 center = new Vector3(gridWidth / 2, gridHeight / 2, cam.transform.position.z);
        cam.transform.position = center;
    }
}
