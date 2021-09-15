using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axie
{
    public class MiniCamUIView : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private RectTransform miniMapRect;
        [SerializeField] private RectTransform miniMapCurrentViewRect;

        public float mapHeight;

        public void UpdateView()
        {
            var mainCamlocalPosition = mainCam.transform.localPosition;
            var ratioY = mainCamlocalPosition.y / (mapHeight);
            var posY = ratioY * miniMapRect.sizeDelta.y;

            var totalWidth = mapHeight * Screen.width / Screen.height;
        
            var ratioX = mainCamlocalPosition.x / (totalWidth);
            var posX = ratioX * miniMapRect.sizeDelta.x;
            miniMapCurrentViewRect.anchoredPosition = new Vector2(posX, posY) * 0.5f;
        
            //update rect size
            var sizeY = mainCam.orthographicSize / mapHeight * miniMapRect.sizeDelta.y;
            var sizeX = sizeY / Screen.height * Screen.width;
            miniMapCurrentViewRect.sizeDelta = new Vector2(sizeX, sizeY);
        }
        
    }
}