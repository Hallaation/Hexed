using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {
    public float TravelDistance = 10;
    public float TimeToOpen = 2;
    public float TimeStaysOpen = 2;
    public float TimeBetweenMovements = .1f;
    float timer;
    BoxCollider2D Coll;
    Collider2D Coll2d;
    Transform ChildPosition;
    Vector2 OriginalPosition;
    Vector3 EndPosition;
    bool DoorIsOpening;
    // Use this for initialization
    void Start () {
        Coll = GetComponent<BoxCollider2D>();
        Coll2d = GetComponent<Collider2D>();
   
        ChildPosition = transform.GetChild(0).GetComponentInChildren<Transform>();
        OriginalPosition = ChildPosition.transform.position;
        EndPosition = OriginalPosition + new Vector2(0, TravelDistance);
        DoorIsOpening = false;
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if(DoorIsOpening == false)
        {
            if (!Coll.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
            {
                if (timer >= TimeStaysOpen)
                {
                    StartCoroutine(CCloseDoor());
                    timer = 0;
                }
                timer += Time.deltaTime;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D Collider)
    {
        if (Collider.tag == "Player")
        {
            if (DoorIsOpening == false)
            {
                DoorIsOpening = true;
                StartCoroutine(COpenDoor());
            }
        }
    }

   void OpenDoor()
    {
        float ctimer = 0f;
        while (timer < TimeToOpen)
        {
            ChildPosition.position += new Vector3(0, ((TravelDistance / TimeToOpen) * Time.deltaTime));
         
            if(ChildPosition.position.y > OriginalPosition.y + TravelDistance)
            {
                ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y + TravelDistance);
                break;
            }
            ctimer += Time.deltaTime;

        }
        //CloseDoor();
    }
    void CloseDoor()
    {
        float ctimer = 0f;
        while (timer < TimeToOpen)
        {
            ChildPosition.position -= new Vector3(0, ((TravelDistance / TimeToOpen) * Time.deltaTime));
            if (ChildPosition.position.y < OriginalPosition.y)
            {
                ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y);
                break;
            }
            ctimer += Time.deltaTime;

        }
    }

    IEnumerator COpenDoor()
    {
        Vector3 startingPosition = ChildPosition.position;
        float ctimer = 0f;
        while (ctimer < TimeToOpen)
        {
            ChildPosition.position = Vector3.Lerp(startingPosition, EndPosition, (ctimer / TimeToOpen));
            ctimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //CloseDoor();
        DoorIsOpening = false;
    }
    IEnumerator CCloseDoor()
    {
        Vector3 startingPosition = ChildPosition.position;
        float ctimer = 0f;
        while (ctimer < TimeToOpen && DoorIsOpening == false)  
        {
            ChildPosition.position = Vector3.Lerp(startingPosition, OriginalPosition, (ctimer / TimeToOpen));
            ctimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    //    //    IEnumerator COpenDoor()
    //    {

    //        float ctimer = 0f;
    //        while (ctimer <= TimeToOpen)
    //        {
    //            ChildPosition.position += new Vector3(0, ((TravelDistance / TimeToOpen) * TimeBetweenMovements));
    //            if (ChildPosition.position.y > OriginalPosition.y + TravelDistance)
    //            {
    //                ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y + TravelDistance);
    //                break;
    //                timer = 0;
    //            }
    //ctimer += TimeBetweenMovements;
    //            yield return new WaitForEndOfFrame();
    //timer = 0;
    //        }
    //        //CloseDoor();
    //        DoorIsOpening = false;
    //    }
    //    IEnumerator CCloseDoor()
    //{
    //    float ctimer = 0f;
    //    while (ctimer < TimeToOpen && DoorIsOpening == false)
    //    {
    //        ChildPosition.position -= new Vector3(0, ((TravelDistance / TimeToOpen) * TimeBetweenMovements));
    //        if (ChildPosition.position.y < OriginalPosition.y)
    //        {
    //            ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y);
    //            break;
    //        }
    //        ctimer += TimeBetweenMovements;
    //        yield return new WaitForEndOfFrame();

    //    }
}

