using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using OneGame.Lua;
using UnityEngine;

namespace OneGame {
    /// <summary>
    /// A representation of an ingame object in the world builder
    /// </summary>
    public class EntityGameObject : MonoBehaviour, IScriptRunner, IEntityContainer {
        /// <summary>
        /// The element that this entity is based from
        /// </summary>
        public ElementData Element { get { return elementData; } }

        /// <summary>
        /// The Lua interface for referencing this entity
        /// </summary>
        public Entity Entity { get { return entityObj; } }

        public GameObject GameObject { get; private set; }

        public LuaScript Script { get { return luaScript; } set { luaScript = value; } }
        public NativeComponent[] ActiveComponents { get { return activeComponents.ToArray (); } }
        public string AssetId { get { return Element.id.ToHexString (); } }
        public string UniqueId { get { return Id; } }

        /// <summary>
        /// The id of the entity
        /// </summary>
        public string Id { get; private set; }

        public Transform Transform { get; private set; }

        internal ScriptManager ScriptManager {
            get { return scriptManager; }
            set {
                scriptManager = value;
                scriptManager.AddRunner (this);
            }
        }

        private LuaScript luaScript;

        private ElementData elementData;
        private Entity entityObj;
        private ScriptManager scriptManager;
        private List<NativeComponent> activeComponents;

        private void Awake () {
            luaScript = LuaScript.Empty;
            luaScript.id = Id.ToUInt ();
            luaScript.script = new Script ();

            activeComponents = new List<NativeComponent> ();

            GameObject = gameObject;
            Transform = transform;

            entityObj = new Entity (gameObject);
            entityObj.OnScriptAdd += HandleAddScriptEvent;
            entityObj.OnComponentAdd += AddComponent;
            entityObj.OnComponentAddWithData += AddComponent;
            entityObj.OnEntityCall += InvokeLuaMethod;
        }

        private void OnEnable () {
            scriptManager?.AddRunner (this);
        }

        private void OnDisable () {
            scriptManager?.RemoveRunner (this);
        }

        private void OnDestroy () {
            if (entityObj != null) {
                entityObj.OnScriptAdd -= HandleAddScriptEvent;
            }
        }

        public void AddComponent (Type type, string name) {
            var component = gameObject.AddComponent (type.type);
            activeComponents.Add (component as NativeComponent);

            var luaComponent = UserData.Create (component);

            luaScript.script.Globals[name] = luaComponent;
        }

        public void AddComponent (Type type, string name, Item item) {
            var component = gameObject.AddComponent (type.type) as NativeComponent;
            var element = ItemCatalog.Instance.FindElementData (item.id);
            if (element.id != 0) {
                component.ProcessMetadata (element);
            }

            activeComponents.Add (component);

            var luaComponent = UserData.Create (component);

            luaScript.script.Globals[name] = luaComponent;
        }

        /// <summary>
        /// Adds a script to the entity
        /// </summary>
        /// <param name="scriptData">The script data to insert</param>
        /// <param name="invokeStart">Should start() be called after the script is initialized?</param>
        public void AddScript (ScriptData scriptData, bool invokeStart = false) {
            var script = this.luaScript.script;

            script.Globals.Set ("entity", UserData.Create (entityObj));
            script.Globals["anim"] = new Anim (GetComponent<Animator> ());
            script.ApplyDefaultValues ();

            script.DoString (scriptData.script);

            var luaScript = new LuaScript {
                id = (uint)scriptData.id,
                code = scriptData.script,
                script = script,
                properties = ScriptUtility.ExtractProperties (script),
            };

            this.luaScript = luaScript;

            if (invokeStart) {
                script.CallFunction ("start");
            }
        }

        public void AddScript (LuaScript scriptData, bool invokeStart = false) {
            var script = this.luaScript.script;

            script.Globals.Set ("entity", UserData.Create (entityObj));
            script.Globals["anim"] = new Anim (GetComponent<Animator> ());
            script.ApplyDefaultValues ();

            script.DoString (scriptData.code);

            if (scriptData.properties != null) {
                ScriptUtility.ApplyProperties (script, scriptData.properties);
            }

            var luaScript = new LuaScript {
                id = Id.ToUInt (),
                code = scriptData.code,
                script = script,
                properties = ScriptUtility.ExtractProperties (script)
            };

            this.luaScript = luaScript;

            if (invokeStart) {
                script.CallFunction ("start");
            }
        }

        public void InvokeLuaMethod (string methodName) {
            luaScript.script.CallFunction (methodName);
        }

        public void InvokeLuaMethod (string methodName, DynValue value) {
            luaScript.script.CallFunction (methodName, value);
        }

        /// <summary>
        /// Runs an update call on the attached scripts
        /// </summary>
        public void RunUpdate (DynValue deltaTime) {
            luaScript.script.CallFunction ("update", deltaTime);
        }

        /// <summary>
        /// Applies element information and prepares scripts
        /// </summary>
        internal void ApplyElementData (ElementData data) {
            elementData = data;
            gameObject.name = data.name;
            Id = EntityUtilities.GetUniqueId ();
            entityObj.SetId (Id);
            entityObj.SetAssetId (data.id.ToHexString ());
        }

        internal void ApplyElementData (ElementData data, string id) {
            elementData = data;
            gameObject.name = data.name;
            Id = id;
            entityObj.SetId (id);
            entityObj.SetAssetId (data.id.ToHexString ());
        }

        private void HandleAddScriptEvent (string script) {
            var scriptData = new ScriptData {
                name = "Custom Script",
                script = script
            };

            AddScript (scriptData, true);
        }
    }
}