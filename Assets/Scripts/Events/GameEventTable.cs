using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame {
	/// <summary>
	/// A collection of firable events
	/// </summary>
	[CreateAssetMenu (menuName = "Events/Event Table")]
	public class GameEventTable : ScriptableObject {
		[SerializeField, TextArea (10, 50)]
		private string comments;

		private Dictionary<string, Delegate> eventTable;

		private void OnEnable () {
			eventTable = new Dictionary<string, Delegate> ();
		}

		public void Invoke (string eventName) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent?.DynamicInvoke ();
			}
		}

		public void Invoke<T> (string eventName, T value) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent.DynamicInvoke (value);
			}
		}

		public void Invoke<T, U> (string eventName, T value, U value2) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent.DynamicInvoke (value, value2);
			}
		}

		public void Invoke<T, U, V> (string eventName, T value, U value2, V value3) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent.DynamicInvoke (value, value2, value3);
			}
		}

		public void Register (string eventName, Action action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Combine (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			} else {
				eventTable.Add (eventName, action);
			}
		}

		public void Register<T> (string eventName, Action<T> action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Combine (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			} else {
				eventTable.Add (eventName, action);
			}
		}

		public void Register<T, U> (string eventName, Action<T, U> action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Combine (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			} else {
				eventTable.Add (eventName, action);
			}
		}

		public void Register<T, U, V> (string eventName, Action<T, U, V> action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Combine (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			} else {
				eventTable.Add (eventName, action);
			}
		}

		public void Unregister (string eventName, Action action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Remove (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			}
		}

		public void Unregister<T> (string eventName, Action<T> action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Remove (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			}
		}

		public void Unregister<T, U> (string eventName, Action<T, U> action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Remove (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			}
		}

		public void Unregister<T, U, V> (string eventName, Action<T, U, V> action) {
			Delegate delegateEvent;

			if (eventTable.TryGetValue (eventName, out delegateEvent)) {
				delegateEvent = Delegate.Remove (delegateEvent, action);
				eventTable[eventName] = delegateEvent;
			} else {
				eventTable.Add (eventName, action);
			}
		}

	}
}