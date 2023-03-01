using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepCheeseForMouse {
    public class Cam : MonoBehaviour {
        public Transform targetObject;
        private Vector3 cameraPos;
        public Player player;

        void Start() {
            cameraPos = transform.position - targetObject.position;
        }
        void LateUpdate() {
            CameraMovement();
        }
        void CameraMovement() {
            if (!player.deathManager.canDeath) {
                transform.position = targetObject.position + cameraPos;
            }
        }
    }
}