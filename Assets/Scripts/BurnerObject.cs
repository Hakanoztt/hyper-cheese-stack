using System.Collections;
using UnityEngine;

namespace KeepCheeseForMouse {
    public class BurnerObject : MonoBehaviour {
        Player player;
        public float triggerDelay;
        public float makeDeathDelay;
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("CheeseObject")) {
                gameObject.GetComponent<Collider>().enabled = false;
                Invoke("EnableCollider", triggerDelay);
            }
            if (Player.TryGetCharacter(other, out Player player)) {
                this.player = player;
                Invoke("MakeDeath", makeDeathDelay);
            }
        }
        void EnableCollider() {
            gameObject.GetComponent<Collider>().enabled = true;
        }
        void MakeDeath() {
            player.state = Player.States.Burned;
            player.deathManager.MakeDeath();
        }
    }
}