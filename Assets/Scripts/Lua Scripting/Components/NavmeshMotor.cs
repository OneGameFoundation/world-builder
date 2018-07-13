using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.AI;

namespace OneGame.Lua {
	/// <summary>
	/// A c# lua interface for moving characters in a navmesh field
	/// </summary>
	[RequireComponent (typeof (NavMeshAgent))]
	[MoonSharpUserData]
	public class NavmeshMotor : NativeComponent {

		/// <summary>
		/// The motor's current speed
		/// </summary>
		/// <returns></returns>
		public float speed {
			get { return agent.speed; }
			set { agent.speed = value; }
		}

		/// <summary>
		/// The remaining distance of the motor
		/// </summary>
		public float remainingDistance { get { return agent.remainingDistance; } }

		/// <summary>
		/// How far away from the destination should the agent start to brake?
		/// </summary>
		public float stoppingDistance {
			get { return agent.stoppingDistance; }
			set { agent.stoppingDistance = value; }
		}

		private NavMeshAgent agent;

		protected override void Awake () {
			base.Awake ();
			
			agent = GetComponent<NavMeshAgent> ();
		}

		/// <summary>
		/// Moves the agent to a destination
		/// </summary>
		/// <param name="position">The position to move to</param>
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