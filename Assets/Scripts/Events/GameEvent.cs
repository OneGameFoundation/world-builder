using System;
using UnityEngine;

namespace OneGame
{
    [CreateAssetMenu (menuName = "Events/Game Event")]
	public class GameEvent : ScriptableObject {
		[SerializeField, TextArea]
		private string comments;

		private event Action OnGameEventInvoke;

		public void Invoke () {
			if (OnGameEventInvoke != null) {
				OnGameEventInvoke ();
			}
		}

		public void Register (Action handler) {
			OnGameEventInvoke -= handler;
			OnGameEventInvoke += handler;
		}

		public void Unregister (Action handler) {
			OnGameEventInvoke -= handler;
		}
	}

	public abstract class GameEvent<T> : GameEvent {
		public T Value { get; private set; }

		private event Action<T> ValueChangedEvent;

		public void Invoke (T value) {
			Value = value;

			ValueChangedEvent?.Invoke (value);
			Invoke ();
		}

		public void Register (Action<T> handler) {
			ValueChangedEvent -= handler;
			ValueChangedEvent += handler;
		}

		public void Unregister (Action<T> handler) {
			ValueChangedEvent -= handler;
		}
	}
}