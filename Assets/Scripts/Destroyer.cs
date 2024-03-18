using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    GameObject cosa;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cosa = GameObject.Find("Martillo");
        Destroy(cosa, 0.3f);
        Destroy(this.gameObject, 1f);
    }
}
