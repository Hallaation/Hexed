using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float zOffSet = 10;
    public float maxMouseDistance = 10;
    public GameObject crosshair;
    public GameObject player;
    public Camera _camera;
    // Use this for initialization
    void Start()
    {
        _camera = Camera.main;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            Debug.Log(ray.origin);
            if (Physics.Raycast(ray , out hit))
            {
            }
            Debug.DrawRay(ray.origin , ray.direction , Color.magenta , 5.0f);
        }
        Vector3 midpoint = Vector3.zero;

        midpoint = crosshair.transform.position + player.transform.position;
        midpoint /= 2.0f;

        Vector3 newCamPosition = midpoint + new Vector3(0 , 0 , -10);
        this.transform.position = Vector3.Lerp(transform.position , newCamPosition , zOffSet);

        Debug.Log((midpoint - player.transform.position).magnitude);
    }
    void FixedUpdate()
    {

        //get the mouse position
        Vector3 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        //get the players position in screen space
        Vector3 temp = Camera.main.WorldToScreenPoint(player.transform.position);
        //do it for local camera (doesn't matter in this case but keeping it anyway)
        Vector3 playerPoint = _camera.ScreenToWorldPoint(temp);

        Vector3 target = mousePoint; //set my target to the monuse position
        Vector3 diff = mousePoint - playerPoint; //the difference between 2 vectors
        float mouseDist = diff.magnitude; //distance between mouse and player

        //if the mouse distance is greater than the maximum distance.
        if (mouseDist > maxMouseDistance)
        {
            
            target = playerPoint + (diff.normalized * maxMouseDistance);
        }

        target.z = crosshair.transform.position.z;
        crosshair.transform.position = target;
    }

}
