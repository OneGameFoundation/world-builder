using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame.UI {
	public class ShopWindow : MonoBehaviour {
		[SerializeField]
		private GameEventTable eventTable;

		private CanvasGroup group;

		private void Awake () {
			group = GetComponent<CanvasGroup> ();
			Close ();
		}

		private void OnEnable () {
			eventTable.Register ("OnShopButtonClicked", Open);
		}

		private void OnDisable () {
			eventTable.Unregister ("OnShopButtonClicked", Open);
		}

		public void Close () {
			group.alpha = 0;
			group.interactable = false;
			group.blocksRaycasts = false;
		}

		public void Open () {
			group.alpha = 1;
			group.interactable = true;
			group.blocksRaycasts = true;
		}
	}
}