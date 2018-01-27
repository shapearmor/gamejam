using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public float initialTimerDuration = 10.0f;
    public float regularTimerDuration = 5.0f;
    public float invalidityZoneRadius = 5.0f;
    public GameObject prefab;

    private float actualTimer = 0.0f;
    private bool canSpawn = false;

    void Start()
    {
		StartCoroutine(InitialTimer());
    }

	private IEnumerator InitialTimer()
	{
		yield return new WaitForSeconds(initialTimerDuration);
		canSpawn = true;
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, invalidityZoneRadius);
    }

    private void Spawn()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }

    void Update()
    {
        if (canSpawn && IsValid())
        {
            actualTimer += Time.deltaTime;
			if (actualTimer >= regularTimerDuration)
			{
				actualTimer = 0.0f;
				Spawn();
			}
        }
    }

    private bool IsValid()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, invalidityZoneRadius);
        foreach (Collider col in cols)
        {
			if (col.GetComponent<Shape>() != null)
				return false;
        }
		return true;
    }

}
