﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : Shape
{

    // Use this for initialization
    protected override void Start()
    {
        // base.Start();
        SwitchState(team);
    }
}
