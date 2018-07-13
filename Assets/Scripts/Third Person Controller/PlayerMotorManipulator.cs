using OneGame.Lua;
using OneGame.TPC;
using UnityEngine;

namespace OneGame {
	public class PlayerMotorManipulator : MonoBehaviour {
		[SerializeField]
		private string deathTriggerName = "Dead";

		private Animator animator;
		private Health health;
		private PlayerMotor motor;

		private void Awake () {
			animator = GetComponent<Animator> ();
			health = GetComponent<Health> ();
			motor = GetComponent<PlayerMotor> ();
		}

		private void OnEnable () {
			if (health != null) {
				health.OnDeath += HandleDeath;
				health.OnDamage += HandleDamage;
			}
		}

		private void OnDisable () {
			if (health != null) {
				health.OnDeath -= HandleDeath;
				health.OnDamage -= HandleDamage;
			}
		}

		private void HandleDeath () {
			Player.Instance.isMoving = false;
			motor.CanMove = false;

			if (animator != null) {
				animator.SetTrigger (deathTriggerName);
			}
		}

		private void HandleDamage (float amt) {
			animator?.SetTrigger ("Stagger");
		}
	}
}