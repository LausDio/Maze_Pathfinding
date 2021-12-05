using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinGame : MonoBehaviour
{
    [SerializeField]
    public GameObject winMess;


    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.CompareTag("Finish");
        winMess.SetActive(true);
    }
   
}
