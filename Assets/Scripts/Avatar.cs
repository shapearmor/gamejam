using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : Shape
{
    public AvatarParameters param;
    public string playerType = "P1";

    private Rigidbody rigid;
    private float baseDrag;

    protected override void Start()
    {
        base.Start();

        rigid = GetComponent<Rigidbody>();
        baseDrag = rigid.drag;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis(playerType + "_Horizontal"), Input.GetAxis(playerType + "_Vertical"));
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
        Debug.Log("Rotation : " +  transform.rotation.eulerAngles);
    }

    private void Thrust(float input)
    {
        if (input > 0.0f)
        {
            rigid.AddForce(transform.forward * param.thrustPower * Time.deltaTime);
            // rigid.AddForceAtPosition(transform.forward * param.thrustPower * Time.deltaTime, transform.position, ForceMode.Force);
        }
        else
        {
            rigid.drag = param.brakeForce;
        }
    }

}
