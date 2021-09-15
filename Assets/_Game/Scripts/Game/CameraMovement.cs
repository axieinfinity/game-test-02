using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Axie
{

    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private MiniCamUIView miniCamUIView;
        
        [Space, Header("Zoom")]
        [SerializeField] private float zoomSpeed = 20f;
        [SerializeField] private float[] zoomBounds = new float[]{5f, 30f};
        
        [Space, Header("Pan")]
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float[] panBounds = new float[]{-20f, 20f};
        

        private Vector3 lastPanPosition;
        
        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            
            if (Input.GetMouseButtonDown(0)) {
                lastPanPosition = Input.mousePosition;
            } else if (Input.GetMouseButton(0)) {
                PanCamera(Input.mousePosition);
            }
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
                Zoom(scroll);
        }
        
        void PanCamera(Vector3 newPanPosition) {
            // Determine how much to move the camera
            Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
            Vector3 move = new Vector3(offset.x * panSpeed, offset.y * panSpeed) * cam.orthographicSize;
        
            // Perform the movement
            transform.Translate(move, Space.World);  
        
            // Ensure the camera remains within bounds.
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(transform.position.x, panBounds[0], panBounds[1]);
            pos.y = Mathf.Clamp(transform.position.y, panBounds[0], panBounds[1]);
            transform.position = pos;
    
            // Cache the position
            lastPanPosition = newPanPosition;
            miniCamUIView.UpdateView();
        }

        private void Zoom(float scroll)
        {
            // Get MouseWheel-Value and calculate new Orthographic-Size
            // (while using Zoom-Speed-Multiplier)
            float mouseScrollWheel = scroll * zoomSpeed;
            float newZoomLevel = cam.orthographicSize - mouseScrollWheel;

            // Get Position before and after zooming
            Vector3 mouseOnWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            cam.orthographicSize = Mathf.Clamp(newZoomLevel, zoomBounds[0], zoomBounds[1]);
            Vector3 mouseOnWorld1 = cam.ScreenToWorldPoint(Input.mousePosition);

            // Calculate Difference between Positions before and after Zooming
            Vector3 posDiff = mouseOnWorld - mouseOnWorld1;

            // Add Difference to Camera Position
            Vector3 camPos = cam.transform.position;
            Vector3 targetPos = new Vector3(
                camPos.x + posDiff.x,
                camPos.y + posDiff.y,
                camPos.z);

            // Apply Target-Position to Camera
            cam.transform.position = targetPos;
            miniCamUIView.UpdateView();
        }

        public void UpdateBound(float mapHeight)
        {
            zoomBounds[1] = mapHeight;
            panBounds[0] = -mapHeight * 0.5f;
            panBounds[1] = mapHeight * 0.5f;
            
            miniCamUIView.mapHeight = mapHeight;
            miniCamUIView.UpdateView();
        }
    }
}