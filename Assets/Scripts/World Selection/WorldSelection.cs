using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace OneGame
{
    public class WorldSelection : MonoBehaviour
    {
        public string myWorldListUrl;
        public string otherWorldListUrl;

        public Button myWorldBtn;
        public Button worldCenterBtn;
        public RectTransform content;
        public GameObject worldItemPrefab;
        public RawImage previewImage;

        public GameObject[] myWorldGroup;

        public GameObject[] otherWorldGroup;


        public enum WorldSelectionMenuState
        {
            MyWorld,
            WorldCenter
        }


        private List<WorldInfo> myWorlds;
        private List<WorldInfo> otherWorlds;
        private List<WorldItem> worldItemList;


        private WorldSelectionMenuState currentState = WorldSelectionMenuState.WorldCenter;

        private WorldInfo selectedWorld = null;

        public WorldInfo SelectedWorld { get { return selectedWorld; } set { selectedWorld = value; } }
        public List<WorldItem> GeneratedItems { get { return worldItemList; } }

		public void Reset()
        {
            myWorlds = new List<WorldInfo>();
            otherWorlds = new List<WorldInfo>();
            worldItemList = new List<WorldItem>();
            SwapToMyWorld(); //default state
        }

        public void SwapToMyWorld()
        {
            SwapMenuState(WorldSelectionMenuState.MyWorld);
            TogglePreviewBtns(WorldSelectionMenuState.MyWorld);
        }

        public void SwapToWorldCenter()
        {
            SwapMenuState(WorldSelectionMenuState.WorldCenter);
            TogglePreviewBtns(WorldSelectionMenuState.WorldCenter);
        }

        public void SwapMenuState(WorldSelectionMenuState swapTo)
        {
            currentState = swapTo;
            if (currentState == WorldSelectionMenuState.MyWorld)
            {
                myWorldBtn.interactable = false;
                worldCenterBtn.interactable = true;
                ClearAllItems();
                StartCoroutine(LoadWorldListRutine(myWorlds, myWorldListUrl));
            } 
			else if (currentState == WorldSelectionMenuState.WorldCenter)
            {
                myWorldBtn.interactable = true;
                worldCenterBtn.interactable = false;
                ClearAllItems();
                StartCoroutine(LoadWorldListRutine(otherWorlds, otherWorldListUrl));
            }
        }



        private IEnumerator LoadWorldListRutine(List<WorldInfo> list, string url)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                //Debug.Log(www.text);
                processingJson(www.text, list);
            }
        }


        private void processingJson(string jsonString, List<WorldInfo> thisList)
        {
            JsonData data = JsonMapper.ToObject(jsonString);
            thisList.Clear();
            worldItemList.Clear();
            for (var i = 0; i < data.Count; i++)
            {
                WorldInfo thisWorld = new WorldInfo();
                thisWorld.name = data[i]["name"].ToString();
                thisWorld.id = (int)data[i]["id"];
                thisWorld.thumbNailPath = data[i]["thumbnail-path"].ToString();
                thisWorld.imagePath = data[i]["image-path"].ToString();

                thisList.Add(thisWorld);
                var thisWorldObj = Instantiate(worldItemPrefab, content);
                thisWorldObj.GetComponent<WorldItem>().UpdateInfo(thisWorld, this);
                worldItemList.Add(thisWorldObj.GetComponent<WorldItem>());

                if (i == 0)
                {
                    selectedWorld = thisWorld;
                    thisWorldObj.GetComponent<WorldItem>().Select();
                    UpdatePreview();
                }
            }
        }

        public void UpdatePreview()
        {
			StartCoroutine(GetDetailPic("http://localhost:8000" + selectedWorld.imagePath));
        }

        private IEnumerator GetDetailPic(string url)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                previewImage.texture = www.texture;
            }
        }

        private void ClearAllItems()
        {
            foreach (RectTransform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        private void TogglePreviewBtns(WorldSelectionMenuState state)
        {
            if (state == WorldSelectionMenuState.MyWorld)
            {
                foreach (GameObject obj in myWorldGroup)
                {
                    obj.SetActive(true);
                }

                foreach (GameObject obj in otherWorldGroup)
                {
                    obj.SetActive(false);
                }
            }
            else if (state == WorldSelectionMenuState.WorldCenter)
            {
                foreach (GameObject obj in myWorldGroup)
                {
                    obj.SetActive(false);
                }

                foreach (GameObject obj in otherWorldGroup)
                {
                    obj.SetActive(true);
                }
            }
        }

		public void GotoWorldBuilder() {
			SceneManager.LoadScene ("WorldBuilder");
		}
    }
}