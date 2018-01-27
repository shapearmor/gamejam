using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Avatar Parameters", fileName =  "New Avatar Parameters Instance", order = 0)]
[System.Serializable]
public class AvatarParameters : ScriptableObject
{
	public float thrustPower = 5.0f;
	public float brakeForce = 15.0f;
	public float rotationSpeed = 5.0f;
	public AvatarDisplacementMode displacementMode = AvatarDisplacementMode.Pivot;

}


