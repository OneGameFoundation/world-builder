using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.AI;

namespace OneGame.Lua {
	/// <summary>
	/// A c# bridge for moving characters in a navmesh area, with root motion
	/// </summary>
	[RequireComponent (typeof (NavMeshAgent))]
	[MoonSharpUserData]
	public class RootMotionMotor : NativeComponent {

		public float acceleration {
			get { return agent.acceleration; }
			set { agent.acceleration = value; }
		}

		/// <summary>
		/// The remaining distance of the motor
		/// </summary>
		public float remainingDistance { get { return agent.remainingDistance; } }

		/// <summary>
		/// The motor's current speed
		/// </summary>
		/// <returns></returns>
		public float speed {
			get { return agent.speed; }
			set { agent.speed = value; }
		}

		/// <summary>
		/// How far away from the destination should the agent start to brake?
		/// </summary>
		public float stoppingDistance {
			get { return agent.stoppingDistance; }
			set { agent.stoppingDistance = value; }
		}

		private const string ForwardAnimName = "Forward Speed";
		private const string HorizontalAnimName = "Horizontal Speed";
		private const string SpeedAnimName = "Speed";

		private Animator animator;
		private NavMeshAgent agent;

		Vector3 velocity;

		protected override void Awake () {
			base.Awake ();

			animator = GetComponentInChildren<Animator> ();
			agent = GetComponent<NavMeshAgent> ();

			agent.updateRotation = true;
			agent.updatePosition = false;
		}

		private void Update () {
			velocity = Vector3.Lerp (velocity, agent.desiredVelocity, agent.acceleration * Time.deltaTime);
			var normalizedVelocity = new Vector2 (
				Vector3.Dot (transform.right, velocity),
				Vector3.Dot (transform.forward, velocity)
			);

			// Set the animator's speed
			animator.SetFloat (HorizontalAnimName, normalizedVelocity.x);
			animator.SetFloat (ForwardAnimName, normalizedVelocity.y);
			animator.SetFloat (SpeedAnimName, normalizedVelocity.magnitude);
		}

		private void OnAnimatorMove () {
			var position = animator.rootPosition;
			position.y = agent.nextPosition.y;
			transform.position = position;
			agent.nextPosition = position;
		}

		public void LookAt (Vec3 position, float speed) {
			var diff = position.ToVector3 () - transform.position;
			diff.y = 0;

			if (diff.sqrMagnitude != 0) {
				var lookRotation = Quaternion.LookRotation (diff);
				transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * speed);
			}
		}

		public void MoveTo (Vec3 position) {
			agent.SetDestination (position.ToVector3 ());
		}

		/// <summary>
		/// Resumes the agent
		/// </summary>
		public void Resume () {
			agent.isStopped = false;
		}

		/// <summary>
		/// Stops the agent
		/// </summary>
		public void Stop () {
			agent.isStopped = true;
		}
	}

}