using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using OneGame.Lua;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame.UI {
    public class EntityScriptSubMenu : MonoBehaviour, IEntityWindowComponent {
        public IEntityContainer Entity { get { return entity; } set { entity = value; } }
        public EntityWindowComponentType Type { get { return EntityWindowComponentType.Script; } }

        [SerializeField]
        private Dropdown dropdownMenu;
        [SerializeField]
        private ScriptCatalog scriptDatabase;
        [SerializeField]
        private GameEventTable eventTable;

        private IEntityContainer entity;
        private CanvasGroup group;

        private void Awake () {
            group = GetComponent<CanvasGroup> ();
        }

        private IEnumerator Start () {
            while (!scriptDatabase.IsReady) {
                yield return null;
            }

            PrepareScriptTemplates ();
        }

        private void OnEnable () {
            eventTable.Register<IEntityContainer> ("OnScriptApply", RefreshPropertyWindow);
        }
        private void OnDisable () {
            eventTable.Unregister<IEntityContainer> ("OnScriptApply", RefreshPropertyWindow);
        }

        /// <summary>
        /// Clear script in script editor
        /// </summary>
        public void ClearScript () {
            var script = entity.Script;
            script.code = string.Empty;
            script.script.DoString (string.Empty);
            entity.Script = script;

            eventTable?.Invoke<Tuple<ScriptProperty, Action<string>>[]> (
                "OnPropertiesInspect", new Tuple<ScriptProperty, Action<string>>[0]);
        }

        /// <summary>
        /// Close sub menu
        /// </summary>
        public void Close () {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        /// <summary>
        /// pass the entity and script in an event, when user is editing script
        /// </summary>
        public void InvokeEditScriptEvent () {
            eventTable?.Invoke<IEntityContainer, LuaScript> ("OnScriptEditorOpen", entity, entity.Script);
        }

        /// <summary>
        /// open the menu and invoke an event to pass selected script properties
        /// </summary>
        public void Open () {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;

            eventTable?.Invoke<Tuple<ScriptProperty, Action<string>>[]> (
                "OnPropertiesInspect", CreateScriptProperties (entity.Script.properties, entity.Script.script));
        }

        /// <summary>
        /// refresh the property window by invoking the event
        /// </summary>
        /// <param name="entity">the entity that is selected</param>
        public void RefreshPropertyWindow (IEntityContainer entity) {
            eventTable?.Invoke<Tuple<ScriptProperty, Action<string>>[]> (
                "OnPropertiesInspect", CreateScriptProperties (entity.Script.properties, entity.Script.script));
        }

        /// <summary>
        /// create property gameobject on the ui from the script's variables
        /// </summary>
        /// <param name="extract"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        private Tuple<ScriptProperty, Action<string>>[] CreateScriptProperties (ScriptProperty[] extract, Script script) {
            if (extract == null) {
                extract = ScriptUtility.ExtractProperties (script);
            }

            var properties = new Tuple<ScriptProperty, Action<string>>[extract.Length];

            for (var i = 0; i < properties.Length; ++i) {
                var index = i;
                var prop = extract[index];
                Action<string> callback = null;

                switch (prop.type) {
                    case PropertyType.Boolean:
                        callback = s => {
                            prop.value = s;
                            extract[index] = prop;
                            ScriptUtility.ApplyProperties (script, extract);
                        };
                        break;

                    case PropertyType.Number:
                        callback = s => {
                            var num = default (double);
                            if (double.TryParse (s, out num)) {
                                prop.value = s.ToString ();
                                extract[index] = prop;
                                ScriptUtility.ApplyProperties (script, extract);
                            }
                        };
                        break;

                    case PropertyType.String:
                        callback = s => {
                            prop.value = s;
                            extract[index] = prop;
                            ScriptUtility.ApplyProperties (script, extract);
                        };
                        break;
                }

                properties[i] = new Tuple<ScriptProperty, Action<string>> (prop, callback);
            }

            return properties;
        }

        private Property[] GetProperties (Script script) {
            var table = script.Globals;
            var propList = new List<Property> ();

            foreach (var tableKey in table.Keys) {
                var key = tableKey;
                var value = table.Get (key);
                var p = new Property {
                    name = key.String.Replace ("\"", "")
                };

                switch (value.Type) {
                    case DataType.Boolean:
                        p.type = PropertyType.Boolean;
                        p.value = value.Boolean.ToString ();
                        p.onPropertyEdit = s => { table.Set (key, DynValue.NewBoolean (s == "true")); };
                        propList.Add (p);
                        break;

                    case DataType.Number:
                        p.type = PropertyType.Number;
                        p.value = value.Number.ToString ();
                        p.onPropertyEdit = s => {
                            var number = default (double);
                            if (double.TryParse (s, out number)) {
                                table.Set (key, DynValue.NewNumber (number));
                            }
                        };
                        propList.Add (p);
                        break;

                    case DataType.String:
                        p.type = PropertyType.String;
                        p.value = value.String;
                        p.onPropertyEdit = s => table.Set (key, DynValue.NewString (s));
                        propList.Add (p);
                        break;
                }
            }

            return propList.ToArray ();
        }

        private void PrepareScriptTemplates () {
            var scripts = scriptDatabase.Scripts;

            var options = new List<Dropdown.OptionData> ();
            for (var i = 0; i < scripts.Length; ++i) {
                options.Add (new Dropdown.OptionData {
                    text = scripts[i].name
                });
            }

            dropdownMenu.ClearOptions ();
            dropdownMenu.AddOptions (options);

            dropdownMenu.onValueChanged.AddListener (i => {
                var overwrite = scriptDatabase.Scripts[i];
                var script = entity.Script;
                var isValidated = true;
                try {
                    script.script.DoString (overwrite.script);
                } catch {
                    Debug.LogWarning ("Error on code!");
                    isValidated = false;
                }

                // If the code passes the Moonsharp text, finalize the script values
                if (isValidated) {
                    script.code = overwrite.script;
                    script.properties = ScriptUtility.ExtractProperties (script.script);
                    entity.Script = script;

                    eventTable?.Invoke<Tuple<ScriptProperty, Action<string>>[]> ("OnPropertiesInspect", CreateScriptProperties (script.properties, script.script));
                    eventTable?.Invoke<IEntityContainer, LuaScript> ("OnScriptEditorOpen", entity, entity.Script);
                }
            });
        }
    }
}