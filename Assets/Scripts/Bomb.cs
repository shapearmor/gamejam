using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Shape
{
    public GameObject prefab;
    public float boomRadius = 2.5f;
    protected override void Start()
    {
        // base.Start();
        SwitchState(team);
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    protected override void PopEffect()
    {
        Instantiate(prefab, this.transform.position, this.transform.rotation);
    }
}
