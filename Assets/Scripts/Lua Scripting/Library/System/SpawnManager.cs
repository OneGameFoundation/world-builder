using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
	/// <summary>
	/// A C# representation of a spawn manager
	/// </summary>
	[MoonSharpUserData]
	public static class SpawnManager {

		[MoonSharpHidden]
		public static Func<string, Entity> CreateEmptyDelegate;
		[MoonSharpHidden]
		public static event Action<Entity> OnEntityDestroy;
		[MoonSharpHidden]
		public static event SpawnItemDelegate OnIemSpawn;
		[MoonSharpHidden]
		public static Func<string, Entity> FindEntityFunction;

		public delegate Entity SpawnItemDelegate (Item item, Vector3 position, Vector3 scale, Vector3 rotation);

		/// <summary>
		/// Creates an empty entity with a given name
		/// </summary>
		/// <param name="name">The name to give the entity</param>
		public static Entity CreateEmpty (string name) {
			if (CreateEmptyDelegate != null) {
				return CreateEmptyDelegate (name);
			}

			return default (Entity);
		}

		/// <summary>
		/// Destroys an entity from the game
		/// </summary>
		/// <param name="entity">The entity to destroy</param>
		public static void Destroy (Entity entity) {
			OnEntityDestroy?.Invoke (entity);
		}

		/// <summary>
		/// Finds an entity by name or id
		/// </summary>
		/// <param name="name">The name or id to query</param>
		public static Entity Find (string name) {
			if (FindEntityFunction != null) {
				return FindEntityFunction (name);
			}

			return default (Entity);
		}

		/// <summary>
		/// Spawns a new entity with a given item
		/// </summary>
		/// <param name="item">The item to spawn</param>
		/// <param name="position">The position of the new entity</param>
		/// <param name="rotation">The rotation of the new entity</param>
		/// <param name="scale">The scale of the new entity</param>
		public static Entity Spawn (Item item, Vec3 position, Vec3 rotation, Vec3 scale) {
			if (OnIemSpawn != null) {
				return OnIemSpawn (
					item,
					position.ToVector3 (),
					rotation.ToVector3 (),
					scale.ToVector3 ()
				);
			}

			return default (Entity);
		}
	}
}