using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepCheeseForMouse {
    public class CheeseObject : MonoBehaviour {
        public Rigidbody rb;
        Player player;
        public Action<Collider> OnTriggerEntered;  // playerde ontriggerenter kontrolü.
        public bool removed = false;
        private void Update() {
            ReduceScale();
        }
        void ReduceScale() {
            if (removed) {
                if (transform.localScale.y > 0) {
                    transform.localScale -= new Vector3(0.01f, 0.01f, 0);
                } else {
                    gameObject.SetActive(false);
                }
            }
        }
        private void OnTriggerEnter(Collider other) {
            OnTriggerEntered?.Invoke(other); 
            if (Player.TryGetCharacter(other, out Player p)) {
                player = p;
                player.stackManager.Add(this);
            }
            if (other.CompareTag("FinishLine")) {
                gameObject.SetActive(false);
            }
        }
    }
}