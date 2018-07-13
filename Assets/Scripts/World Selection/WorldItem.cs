using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneGame
{
    public class WorldItem : MonoBehaviour
    {
        public Text nameLabel;
        public RawImage icon;

        private WorldInfo thisInfo;

        public Color selectedColor;
        public Color unSelectedColor;

        public Image selectionIndicator;

        private WorldSelection manager;
        public WorldInfo Info { get { return thisInfo; } set { value = thisInfo; } }

        private Texture2D tmpPic;
        public void UpdateInfo(WorldInfo info, WorldSelection managerThis)
        {
            thisInfo = info;
            nameLabel.text = info.name;
            manager = managerThis;
            var iconUrl = info.thumbNailPath;
			StartCoroutine(getPic("http://localhost:8000" + iconUrl));

        }

        public void SelectedThis()
        {
            manager.SelectedWorld = thisInfo;
            //Debug.Log(manager.SelectedWorld);
            UnHighlightAll();
            Select();
            manager.UpdatePreview();
            //StartCoroutine(getDetailPic(thisInfo.imagePath));
        }

        public void Unselect()
        {
            selectionIndicator.color = unSelectedColor;
        }

        public void Select()
        {
            selectionIndicator.color = selectedColor;
        }

        private void UnHighlightAll()
        {
            for (var i = 0; i < manager.GeneratedItems.Count; i++)
            {
                manager.GeneratedItems[i].Unselect();
            }

        }

        private IEnumerator getPic(string url)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                icon.texture = www.texture;
                //Debug.Log(www.texture);
            }
        }




    }
}
