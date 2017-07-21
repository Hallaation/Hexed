using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    //shitty fidget spinner class I have for some reason

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Rigidbody2D>().AddTorque(100);
        if (Input.GetKey(KeyCode.Space))
        {
            Ray2D ray = new Ray2D(new Vector2(this.transform.position.x , this.transform.position.y) , -Vector2.right);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin , ray.direction , 5);
            if (hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<Rigidbody2D>().AddForce(-this.transform.right * 20 , ForceMode2D.Impulse);
            }
            Debug.DrawRay(ray.origin , ray.direction , Color.red , 2);
        }
    }
}
