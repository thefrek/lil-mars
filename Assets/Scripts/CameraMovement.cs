using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 origin;
    private Vector3 difference;

    private bool drag = false;

    int zoom = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if(drag == false)
            {
                drag = true;
                origin = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            Camera.main.transform.position = origin - difference;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && zoom < 3) // forward
        {
            zoom++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && zoom > -3) // backwards
        {
            zoom--;
        }

        Camera.main.transform.localScale = new Vector3(Mathf.Pow(2, zoom), Mathf.Pow(2, zoom), 1) ;
    }
}
