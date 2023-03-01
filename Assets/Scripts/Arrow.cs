
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepCheeseForMouse {
    public class Arrow : MonoBehaviour {
        public float speed;
        public float downSpeed;
        public Transform targetPos;
        public int reduceCheese;

        void Update() {
            Move();
        }
        public void Move() {
          transform.position += new Vector3(speed * Time.deltaTime, -1*downSpeed*Time.deltaTime, -speed * Time.deltaTime);
        }
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Plane")) {
                Destroy(gameObject);
            }
        }
    }
}