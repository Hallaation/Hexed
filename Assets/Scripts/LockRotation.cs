using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour {

  Quaternion rotation;
  void Awake()
  {
       transform.parent.rotation = transform.rotation;
  }
  void Update()
  {
        transform.parent.rotation = rotation;
  }
}
