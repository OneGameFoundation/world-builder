using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneGame
{
    public class LobbyManager : MonoBehaviour
    {
		public GameObject[] lobbyList;
		public GameObject[] worldSelectionList;

		void Awake()
		{
			ShowLobby ();
		}

		public void LoadScene(string sceneName)
        {
            var current = SceneManager.GetActiveScene();
			SceneManager.LoadSceneAsync(sceneName);
            SceneManager.UnloadSceneAsync(current);
        }

		public void ShowWorldSelection()
		{
			foreach (GameObject go in lobbyList)
			{
				go.SetActive (false);
			}
			foreach (GameObject go in worldSelectionList)
			{
				go.SetActive (true);
			}
			GetComponent<WorldSelection> ().Reset ();
		}

		public void ShowLobby()
		{
			foreach (GameObject go in lobbyList)
			{
				go.SetActive (true);
			}
			foreach (GameObject go in worldSelectionList)
			{
				go.SetActive (false);
			}
		}
    }
}
