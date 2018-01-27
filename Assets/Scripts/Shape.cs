﻿using System.Collections;
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
        Debug.Log("Hello " + this.gameObject.name + " | " + other.collider.gameObject.name);
        if (other.collider.gameObject.CompareTag("Player"))
        {
            if (team == TeamEnum.Neutral)
            {
                CollisionNeutralToPlayer(other);
            }
            else if (team != other.collider.gameObject.GetComponent<Avatar>().team)
            {
                FreeChild(other.collider.transform);
                Destroy(other.collider.gameObject);
                Destroy(this.gameObject);
            }

        }
        else if (other.collider.gameObject.CompareTag("Shape") && this.gameObject.name != "Avatar")
        {
            Shape otherShape = other.collider.gameObject.GetComponent<Shape>();
            if (team == TeamEnum.Neutral && otherShape.team != TeamEnum.Neutral)
            {
                CollisionNeutralToShape(other);
            }
            else if (team != TeamEnum.Neutral && otherShape.team != team)
            {
                Debug.Log("Collision " + this.gameObject.name + " with " + other.collider.gameObject.name);
                Destroy(other.collider.gameObject);
                Destroy(this.gameObject);
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
                other.GetChild(i).SetParent(null, true);

            }
        }
    }

    void CollisionNeutralToShape(Collision other)
    {
        SwitchState(other.gameObject.GetComponent<Shape>().team);
        this.transform.SetParent(other.collider.gameObject.transform);
        Destroy(this.GetComponent<Rigidbody>());
    }

    void CollisionNeutralToPlayer(Collision other)
    {
        SwitchState(other.gameObject.GetComponent<Avatar>().team);
        this.transform.SetParent(other.collider.gameObject.transform, true);
        Destroy(this.GetComponent<Rigidbody>());
    }

    void DestroyGameObject(GameObject otherObject)
    {
        Destroy(otherObject);
        Destroy(this);
    }
}