using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamEnum { Neutral, Red, Blue, }

public class Shape : MonoBehaviour
{
    public TeamEnum team = TeamEnum.Neutral;

    protected virtual void Start()
    {
        SwitchState(team);
    }

    public void SwitchState(TeamEnum newState)
    {
        switch (newState)
        {
            case TeamEnum.Blue:
                GetComponent<Renderer>().material.color = Color.blue;
                break;

            case TeamEnum.Red:
                GetComponent<Renderer>().material.color = Color.red;
                break;

            case TeamEnum.Neutral:
                GetComponent<Renderer>().material.color = Color.white;
                break;
        }
        team = newState;
    }

    void OnCollisionEnter(Collision other)
    {

        ContactPoint contact = other.contacts[0];
        Debug.Log("Hello " + contact.thisCollider + " | " + contact.otherCollider);
        if (contact.thisCollider.gameObject.GetComponent<Shape>() == null || contact.otherCollider.gameObject.GetComponent<Shape>() == null)
        {
            return;
        }
        TeamEnum thisTeam = contact.thisCollider.gameObject.GetComponent<Shape>().team;
        TeamEnum otherTeam = contact.otherCollider.gameObject.GetComponent<Shape>().team;
        if (contact.otherCollider.gameObject.CompareTag("Player")) {
            if (thisTeam == TeamEnum.Neutral)
            {
                Debug.Log("Col NeutralToPlayer");
                CollisionNeutralToPlayer(contact.otherCollider);
            }
            else if (thisTeam != otherTeam && contact.thisCollider.gameObject.tag != "Player")
            {
                Debug.Log("Col EnnemieToPlayer");
                FreeChild(contact.otherCollider.transform);
                Destroy(contact.otherCollider.gameObject);
                FreeChild(contact.thisCollider.transform);
                Destroy(contact.thisCollider.gameObject);
            }
        }
        else if (contact.otherCollider.gameObject.CompareTag("Shape") && contact.thisCollider.gameObject.tag != "Player")
        {
            if (thisTeam == TeamEnum.Neutral && otherTeam != TeamEnum.Neutral)
            {
                Debug.Log("Col NeutralToShape");
                CollisionNeutralToShape(contact.otherCollider);
            }
            else if (thisTeam != TeamEnum.Neutral && otherTeam != thisTeam)
            {
                Debug.Log("Col EnnemieToShape");
                FreeChild(contact.otherCollider.transform);
                Destroy(contact.otherCollider.gameObject);
                FreeChild(contact.thisCollider.transform);
                Destroy(contact.thisCollider.gameObject);
            }
        }
    }

    void FreeChild(Transform other)
    {
        int children = other.childCount;
        for (int i = 0; i < children; ++i)
        {
            if (other.GetChild(i).gameObject.GetComponent<Shape>() != null)
            {
                FreeChild(other.GetChild(i));
                other.GetChild(i).gameObject.GetComponent<Shape>().SwitchState(TeamEnum.Neutral);
                other.GetChild(i).gameObject.AddComponent<Rigidbody>();
                other.GetChild(i).gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                other.GetChild(i).SetParent(null, true);

            }
        }
    }

    void CollisionNeutralToShape(Collider other)
    {
        SwitchState(other.gameObject.GetComponent<Shape>().team);
        this.transform.SetParent(other.gameObject.transform);
        Destroy(this.GetComponent<Rigidbody>());
    }

    void CollisionNeutralToPlayer(Collider other)
    {
        SwitchState(other.gameObject.GetComponent<Avatar>().team);
        this.transform.SetParent(other.gameObject.transform);
        Destroy(this.GetComponent<Rigidbody>());
    }
}