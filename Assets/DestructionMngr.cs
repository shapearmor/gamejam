using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionMngr : MonoBehaviour
{
	public static DestructionMngr instance;

	public List<GameObject> toDelete = new List<GameObject>();

	void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(this);
		}
	}

	void LateUpdate()
	{
		if (toDelete.Count > 0)
		{
			foreach(GameObject go in toDelete)
			{
				if (go.transform.childCount > 0)
				{
					foreach(Transform child in go.transform)
					{
						// child.GetComponent<Shape>()
					}
				}
			}
		}
	}

	public void AddDeletion(GameObject target)
	{
		toDelete.Add(target);
	}
}
