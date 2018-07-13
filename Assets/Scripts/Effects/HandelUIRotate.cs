using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneGame
{
    public class HandelUIRotate : MonoBehaviour
    {
        public Vector3 goToRot;
        public float moveDuration = 0.2f;

        public float rotateAmount = 180f;


        // Use this for initialization

        private RectTransform myTrans;
        private Vector3 originRot;

        private bool atOriginPos = true;
        void Start()
        {
            myTrans = GetComponent<RectTransform>();

        }
        public void ToggleState()
        {
            if (atOriginPos)
            {
                Rotate();
            }
            else
            {
                RotateBack();
            }
        }
        public void RotateBack()
        {

            LeanTween.rotateAroundLocal(myTrans, Vector3.forward, -rotateAmount, moveDuration).setEaseOutBounce();
            atOriginPos = false;
        }

        public void Rotate()
        {

            atOriginPos = true;
            LeanTween.rotateAroundLocal(myTrans, Vector3.forward, rotateAmount, moveDuration).setEaseOutBounce();
        }
    }
}
