using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamEnum { Neutral, Red, Blue, }

public class Shape : MonoBehaviour {
    public TeamEnum team = TeamEnum.Neutral;

    void OnCollisionEnter (Collision other)
    {
        if (other.collider.gameObject.CompareTag("Player")){
            if(team == TeamEnum.Neutral)
            {
                CollisionNeutralToPlayer(other);
            }
        }
        /*if (other.gameObject.CompareTag("Shape"))
        {
            Shape otherShape = other.gameObject.GetComponent<Shape>();
            if (otherShape == null) Debug.Log(other.gameObject.name);
            if (team == TeamEnum.Neutral) {
                if (otherShape.team != TeamEnum.Neutral)
                {
                    Debug.Log("Collision " + this.gameObject.name);
                    CollisionNeutral(other);
                }
            } else if (otherShape.team != team)
            {
                Debug.Log("Collision " + this.gameObject.name);
                otherShape.DestroyGameObject(this.gameObject);
            }
        }*/
    }

    void CollisionNeutralToPlayer(Collision other)
    {
        team = other.gameObject.GetComponent<Avatar>().team;
        this.transform.parent = other.collider.gameObject.transform.parent;
        //other.gameObject.transform.parent = this.transform;
        // other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Destroy(this.GetComponent<Rigidbody>());
    }


    void DestroyGameObject(GameObject otherObject)
    {
        Destroy(otherObject);
        Destroy(this);
    }
}
