using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamEnum { Neutral, Red, Bleu, }

public class Shape : MonoBehaviour {
    public TeamEnum team = TeamEnum.Neutral;

    void OnCollisionEnter (Collision other)
    {
        Shape otherShape = other.gameObject.GetComponent<Shape>();
        if (other.gameObject.CompareTag("Shape"))
        {
            if (team != TeamEnum.Neutral) {
                if (otherShape.team == TeamEnum.Neutral)
                {
                    Debug.Log("Collision " + this.gameObject.name);
                    CollisionNeutral(other);
                } else if (otherShape.team != team)
                {
                    Debug.Log("Collision " + this.gameObject.name);
                    otherShape.DestroyGameObject(this.gameObject);
                }
            }
        }
    }

    void CollisionNeutral(Collision other)
    {
        other.gameObject.GetComponent<Shape>().team = team;
        other.collider.gameObject.transform.parent = this.transform;
    }


    void DestroyGameObject(GameObject otherObject)
    {
        Destroy(otherObject);
        Destroy(this);
    }
}
