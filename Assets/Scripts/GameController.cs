using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepCheeseForMouse {
    public class GameController : MonoBehaviour {

        public FinishManager finishManager;
        public Player player;

        public int requiedCheese; // Gerekli peynir sayýsý

        void Start() {
            finishManager.Init(this);
        }
        [Serializable]
        public class FinishManager {
            GameController controller;
            public float finishDelayTime;
            public void Init(GameController c) {
                controller = c;
            }
            public void Finish(bool input) {
                if (input) {
                    controller.player.state = Player.States.WellFinish;
                } else {
                    controller.player.state = Player.States.FailFinish;
                }
                controller.StartCoroutine(StopTheGame(finishDelayTime));
             
            }
            public IEnumerator StopTheGame(float finishDelayTime) {
                yield return new WaitForSeconds(finishDelayTime);
                Time.timeScale = 0.5f;
            }
        }
    }
}