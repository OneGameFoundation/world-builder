using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame.TPC {
	/// <summary>
	/// Controls the player's movements via root motion
	/// </summary>
	[RequireComponent (typeof (Animator))]
	public class PlayerMotor : MonoBehaviour {
		/// <summary>
		/// Can this motor move?
		/// </summary>
		/// <returns></returns>
		public bool CanMove { get { return canMove; } set { canMove = value; } }

		/// <summary>
		/// The current speed of the motor
		/// </summary>
		public float Speed { get; private set; }
		/// <summary>
		/// The current velocity of the motor
		/// </summary>
		/// <returns></returns>
		public Vector2 Velocity { get; private set; }

		public Vector3 TargetLookPosition { get; set; }

		[SerializeField]
		private bool canMove = true;

		[SerializeField, Header ("Motor")]
		private float accleration = 8f;
		[SerializeField]
		private float angularAcceleration = 8f;
		[SerializeField]
		private float minMovementSpeed = 0.1f;

		[SerializeField, Header ("IK Options"), Range (0f, 1f)]
		private float lookWeight;
		[SerializeField, Range (0f, 1f)]
		private float bodyWeight;
		[SerializeField, Range (0f, 1f)]
		private float headWeight;
		[SerializeField, Range (0f, 1f)]
		private float eyeWeight;
		[SerializeField, Range (0f, 1f)]
		private float clampWeight;

		[SerializeField, Header ("Input")]
		private string horizontalControlName = "Horizontal";
		[SerializeField]
		private string verticalControlName = "Vertical";
		[SerializeField]
		private KeyCode sprintKey = KeyCode.LeftShift;
		[SerializeField]
		private KeyCode jumpKey = KeyCode.Space;

		[SerializeField, Header ("Animations")]
		private string horizontalAnimName = "Horizontal Speed";
		[SerializeField]
		private string verticalAnimName = "Vertical Speed";
		[SerializeField]
		private string speedAnimName = "Speed";
		[SerializeField]
		private string jumpTriggerName = "Jump";
		[SerializeField]
		private float jumpForce = 20f;
		[SerializeField]
		private float gravity = 9.8f;

		private Animator animator;
		private CharacterController controller;
		private Vector3 animMovement;
		private Vector3 verticalMovement;

		private void Awake () {
			animator = GetComponent<Animator> ();
			controller = GetComponent<CharacterController> ();
		}

		private void Update () {
			// Handle jump
			if (canMove && controller.isGrounded && Input.GetKeyDown (jumpKey)) {
				animator.SetTrigger (jumpTriggerName);
				verticalMovement.y = jumpForce;
			}

			// Move the character on the vertical axis with gravity
			verticalMovement -= new Vector3 (0f, gravity, 0f) * Time.deltaTime;
			controller.Move (verticalMovement * Time.deltaTime);

			var input = new Vector2 (
				canMove ? Input.GetAxis (horizontalControlName) : 0f,
				canMove ? Input.GetAxis (verticalControlName) : 0f);
			input *= Input.GetKey (sprintKey) ? 2f : 1f;

			animMovement = Vector3.Lerp (animMovement, input, Time.deltaTime * accleration);

			// Update the animator values
			animator.SetFloat (horizontalAnimName, animMovement.x);
			animator.SetFloat (verticalAnimName, animMovement.y);
			animator.SetFloat (speedAnimName, animMovement.magnitude);

			// Set the speed and velocity
			Speed = animMovement.magnitude;
			Velocity = animMovement;

			// Rotate the motor towards the target look position
			if (canMove && (Speed > minMovementSpeed || Input.GetMouseButton (0))) {
				var diff = (TargetLookPosition - transform.position).normalized;
				diff.y = 0f;

				if (diff.sqrMagnitude > 0f) {
					var look = Quaternion.LookRotation (diff);
					transform.rotation = Quaternion.Slerp (transform.rotation, look, Time.deltaTime * angularAcceleration);
				}
			}
		}

		private void OnAnimatorIK (int layer) {
			animator.SetLookAtWeight (lookWeight, bodyWeight, headWeight, eyeWeight, clampWeight);
			animator.SetLookAtPosition (TargetLookPosition);
		}
	}
}