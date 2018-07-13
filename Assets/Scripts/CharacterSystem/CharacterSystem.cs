//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]

public class CharacterSystem : MonoBehaviour
{
	[HideInInspector]
	public string CharacterKey = "";
	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public CharacterController controller;
	[HideInInspector]
	public CharacterMotor Motor;
	[HideInInspector]
	public bool Sprint;
	public float MoveSpeed = 0.7f;
	public float MoveSpeedMax = 5;
	public float TurnSpeed = 5;
	public float PrimaryWeaponDistance = 1;
	public int PrimaryItemType;
	public int AttackType = 0;

	[HideInInspector]
	public float spdMovAtkMult = 1;
	
	
	void Awake ()
	{
		SetupAwake ();
	}
	
	public void SetupAwake ()
	{
		
		Motor = this.GetComponent<CharacterMotor> ();
		controller = this.GetComponent<CharacterController> ();
		animator = this.GetComponent<Animator> ();
		spdMovAtkMult = 1;
	}

	void Update ()
	{
	}

	public void Move (Vector3 directionVector)
	{
		Motor.inputMoveDirection = directionVector;
	}

	public void MoveTo (Quaternion rotation, Vector3 direction)
	{
		Vector3 rotatedDirection = rotation * direction;

		float speed = MoveSpeed;
		if (Sprint)
			speed = MoveSpeedMax;

		Move (rotatedDirection * speed * spdMovAtkMult);

		MoveAnimation (direction);
	}

	private int currentAnimationStatus = 0;

	public void MoveAnimation (Vector3 direction)
	{
		int animationStatus = 0;

		if (direction.magnitude > 0.4f) {
			if (Mathf.Abs (direction.x) < 0.01f) {
				if (direction.z > 0) {
					animationStatus = 1;
				} else {
					animationStatus = 2;
				}
			} else {
				if (direction.x > 0) {
					animationStatus = 4;
				} else {
					animationStatus = 3;
				}
			}
		} else {
			animationStatus = 0;
		}

		if (animationStatus == this.currentAnimationStatus) {
			return;
		}

		this.currentAnimationStatus = animationStatus;

		switch (animationStatus) {
		case 0:
			animator.SetTrigger ("stop");
			break;
		case 1:
			animator.SetTrigger ("forward");
			break;
		case 2:
			animator.SetTrigger ("backward");
			break;
		case 3:
			animator.SetTrigger ("left");
			break;
		case 4:
			animator.SetTrigger ("right");
			break;
		}
	}
}
