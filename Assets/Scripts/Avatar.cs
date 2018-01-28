using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class Avatar : Shape
{
    public AvatarParameters param;
    public string playerType = "P1";
    public bool enabled = false;

    private Rigidbody rigid;
    private float baseDrag;

    public float pitchLevel = 1.4f;

    [Header("Animation Parameters")]
    public AnimationCurve speedEvolution;
    public float maxVelocity = 15.0f;
    private Animator animator;

    protected override void Start()
    {
        // base.Start();
        // SwitchState(team);

        rigid = GetComponent<Rigidbody>();
        baseDrag = rigid.drag;
        audioSource = gameObject.GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        rigid.ResetCenterOfMass();

        if (!enabled) return;

        if (transform.parent != null)
        {
            FreeChild(this.transform);
            Destroy(this.gameObject);
        }

        // Vector2 input = new Vector2(Input.GetAxis(playerType + "_Horizontal"), Input.GetAxis(playerType + "_Vertical"));
        Vector2 input = GetInput();
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

        AnimationUpdate();
    }

    private void AnimationUpdate()
    {
        float actualVelocity = rigid.velocity.magnitude;
        // Debug.Log(actualVelocity);
        float t = Mathf.Lerp(0.0f, 1.0f, actualVelocity / maxVelocity);
        float speed = speedEvolution.Evaluate(t);
        animator.SetFloat("speed", speed);
    }

    private Vector2 GetInput()
    {
        if (IsGamePadAvailable())
        {
            GamePad.Index index = (GamePad.Index)((int)(team + 1));
            // Debug.Log("Gamepad ! " + index);
            Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, index);
            bool thrust = GamePad.GetButton(GamePad.Button.A, index);
            bool brake = GamePad.GetButton(GamePad.Button.B, index);
            float y = 0.0f;
            if (brake) y = -1.0f;
            else if (thrust) y = 1.0f;
            return new Vector2(leftStick.x, y);
        }
        else
        {
            int inputIndex = Mathf.Abs((int)team - Input.GetJoystickNames().Length) + 1;
            // Debug.Log("Keyboard ! " + inputIndex);            
            float x = Input.GetAxis("Horizontal_" + inputIndex);
            float y = Input.GetAxis("Vertical_" + inputIndex);
            return new Vector2(x, y);
        }
    }

    private bool IsGamePadAvailable()
    {
        int numberOfGamepad = Input.GetJoystickNames().Length;
        string[] joysticks = Input.GetJoystickNames();
        // foreach(string str in joysticks)
        // {
        //     Debug.Log(str);
        // }
        if (numberOfGamepad > (int)team)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void SwitchState(TeamEnum newState)
    {
        switch (newState)
        {
            case TeamEnum.Blue:
                spriteRenderer.sprite = skin[1];
                break;

            case TeamEnum.Red:
                spriteRenderer.sprite = skin[0];
                break;

            case TeamEnum.Yellow:
                spriteRenderer.sprite = skin[2];
                break;

            case TeamEnum.Green:
                spriteRenderer.sprite = skin[3];
                break;
            default:
                break;
        }
        team = newState;
        GetComponentInChildren<Animator>().SetInteger("team", (int)team);
    }

    private void Rotate(float input)
    {
        // Quaternion rotation = transform.rotation;
        // Vector3 euler = rotation.eulerAngles;
        // euler.y += param.rotationSpeed * input * Time.deltaTime;
        // transform.rotation = Quaternion.Euler(euler);
        // Debug.Log("Rotation : " + transform.rotation.eulerAngles);

        // rigid.angularVelocity = new Vector3(0.0f, param.rotationSpeed * input * Time.deltaTime, 0.0f);
        transform.Rotate(new Vector3(0.0f, param.rotationSpeed * input * Time.deltaTime, 0.0f));
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

    public void Setup(TeamEnum setup)
    {
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        SwitchState(setup);
        playerType = "P" + ((int)setup + 1).ToString();
        gameObject.name = setup.ToString();
    }
}

public enum AvatarDisplacementMode
{
    Local, Pivot
}


