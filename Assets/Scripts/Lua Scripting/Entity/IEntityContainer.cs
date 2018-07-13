using MoonSharp.Interpreter;
using UnityEngine;

namespace OneGame.Lua {
    /// <summary>
    /// An interface representing a c# wrapper for Lua entities
    /// </summary>
    public interface IEntityContainer {
        /// <summary>
        /// The asset id that this entity is based on
        /// </summary>
        string AssetId { get; }

        /// <summary>
        /// The entity instance of this container
        /// </summary>
        Entity Entity { get; }

        /// <summary>
        /// The root containing gameobject instance
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// The root transform of the entity
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// The active running script on the entity
        /// </summary>
        LuaScript Script { get; set; }

        /// <summary>
        /// A collection of native components on this entity
        /// </summary>
        NativeComponent[] ActiveComponents { get; }

        /// <summary>
        /// The unique instance id of this entity
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// Adds a component to the entity
        /// </summary>
        void AddComponent (Type type, string name);

        /// <summary>
        /// Invokes a Lua method on the entity
        /// </summary>
        /// <param name="methodName">THe name of the method</param>
        void InvokeLuaMethod (string methodName);

        /// <summary>
        /// Invokes a Lua method on the entity, with a given DynValue
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="value">The value of the method</param>
        void InvokeLuaMethod (string methodName, DynValue value);
    }
}