using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepCheeseForMouse {
    public class Blade : MonoBehaviour {
        public float rotateSpeed;
        public float movementSpeed;
        public float triggerDelay;
        float dir;
        private void Start() {
            dir = -1;
        }
        void Update() {
            Movement();
        }
        void Movement() {
            transform.position += new Vector3(dir * movementSpeed * Time.deltaTime, 0, 0);
            transform.Rotate(new Vector3(0, 0, rotateSpeed));
        }
        public IEnumerator TriggerActive(float delay) {
            gameObject.GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(delay);
            gameObject.GetComponent<Collider>().enabled = true;

        }
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("HeadOfObject")) {
                dir = -1;
            }
            if (other.CompareTag("EndOfObject")) {
                dir = 1;
            }

            if (other.CompareTag("CheeseObject")) {
                StartCoroutine(TriggerActive(triggerDelay));
            }
        }
    }
}