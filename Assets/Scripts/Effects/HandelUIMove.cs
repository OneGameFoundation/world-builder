using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame
{
    public class HandelUIMove : MonoBehaviour
    {
        public Vector3 goToPos;
        public float moveDuration = 0.2f;


        // Use this for initialization

        private RectTransform myTrans;
        private Vector3 originPos;

        private bool atOriginPos = true;
        void Start()
        {
            myTrans = GetComponent<RectTransform>();
            originPos = myTrans.anchoredPosition;
        }
        public void ToggleState()
        {
            if (atOriginPos)
            {
                GoThere();
            }
            else
            {
                GoBack();
            }
        }
        public void GoThere()
        {
            LeanTween.move(myTrans, goToPos, moveDuration).setEaseInCirc();
            atOriginPos = false;
        }

        public void GoBack()
        {
            LeanTween.move(myTrans, originPos, moveDuration).setEaseInCirc();
            atOriginPos = true;
        }
    }
}
