using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamEnum { Neutral, Red, Bleu, }

public class Shape : MonoBehaviour {

    [HideInInspector]
    public TeamEnum team;

    void Start()
    {
        team = TeamEnum.Neutral;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shape"))
        {
            if (team != TeamEnum.Neutral) {
                if(other)
            }
        }
    }
}
