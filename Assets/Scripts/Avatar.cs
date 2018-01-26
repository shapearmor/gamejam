﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : Shape
{
    public AvatarParameters param;

    private Rigidbody rigid;
	private float baseDrag;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
		baseDrag = rigid.drag;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input.x != 0.0f)
        {
            Rotate(input.x);
        }

        if (input.y != 0.0f)
        {
            Thrust(input.y);
        }
		else
		{
			rigid.drag = baseDrag;
		}
    }

    private void Rotate(float input)
    {
        Quaternion rotation = transform.rotation;
        Vector3 euler = rotation.eulerAngles;
        euler.y += param.rotationSpeed * input * Time.deltaTime;
        transform.rotation = Quaternion.Euler(euler);
    }

    private void Thrust(float input)
    {
        if (input > 0.0f)
        {
            rigid.AddForce(transform.forward * param.thrustPower * Time.deltaTime);
        }
		else
		{
			rigid.drag = param.brakeForce;
		}
    }

}
