using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamEnum { Neutral, Red, Blue, }

public class Shape : MonoBehaviour {
    public TeamEnum team = TeamEnum.Neutral;

    void OnCollisionEnter (Collision other)
    {
        Debug.Log("Hello " + this.gameObject.name + " | " + other.collider.gameObject.name);
        if (other.collider.gameObject.CompareTag("Player")){
            if (team == TeamEnum.Neutral)
            {
                CollisionNeutralToPlayer(other);
            } else if ( team != other.collider.gameObject.GetComponent<Avatar>().team)
            {
                FreeChild(other.collider.transform);
                Destroy(other.collider.gameObject);
                Destroy(this.gameObject);
            }
            
        } else if (other.collider.gameObject.CompareTag("Shape") && this.gameObject.name != "Avatar")
        {
            Shape otherShape = other.collider.gameObject.GetComponent<Shape>();
            if(team == TeamEnum.Neutral && otherShape.team != TeamEnum.Neutral)
            {
                CollisionNeutralToShape(other);
            } else if (team != TeamEnum.Neutral && otherShape.team != team)
            {
                Debug.Log("Collision " + this.gameObject.name + " with " + other.collider.gameObject.name);
                Destroy(other.collider.gameObject);
                Destroy(this.gameObject);
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

    void FreeChild(Transform other)
    {
        int children = other.childCount;
        for (int i = 0; i < children; ++i)
        {
            if (other.GetChild(i).gameObject.GetComponent<Shape>() != null)
            {

                FreeChild(other.GetChild(i));
                other.GetChild(i).gameObject.GetComponent<Shape>().team = TeamEnum.Neutral;
                other.GetChild(i).gameObject.AddComponent<Rigidbody>();
                other.GetChild(i).SetParent(null);

            }
        }
    }
    void CollisionNeutralToShape(Collision other)
    {
        team = other.gameObject.GetComponent<Shape>().team;
        this.transform.SetParent(other.collider.gameObject.transform);
        //other.gameObject.transform.parent = this.transform;
        // other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Destroy(this.GetComponent<Rigidbody>());
    }

    void CollisionNeutralToPlayer(Collision other)
    {
        team = other.gameObject.GetComponent<Avatar>().team;
        this.transform.parent = other.collider.gameObject.transform;
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
