using System;

namespace OneGame.UI {
    
    /// <summary>
    /// An editable property
    /// </summary>
    public class Property {
        public string name;
        public string value;
        public PropertyType type;

        public Action<string> onPropertyEdit;
    }
}