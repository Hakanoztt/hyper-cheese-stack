using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace KeepCheeseForMouse {
    public class UIManager : MonoBehaviour {

        public Player player;
        public SliderManager sliderManager;
        public TapToPlayModule tapToPlayModule;
        public RestartModule restartModule;
        public WinModule winModule;

        void Start() {
            sliderManager.Init(this);
            tapToPlayModule.Init(this);
            restartModule.Init(this);
            winModule.Init(this);
        }
        void Update() {
            sliderManager.Update();
            tapToPlayModule.Update();
            restartModule.Update();
            winModule.Update();
        }
        public void Restart() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    [Serializable]
    public class SliderManager {
        public UIManager UIManager;
        public Slider slider;
        public float lerpSpeed;
        public void Init(UIManager u) {
            UIManager = u;
            slider.maxValue = UIManager.player.gameController.requiedCheese;
            slider.minValue = 0;
        }
        public void Update() {
            var targetValue = UIManager.player.stackManager.cheeseList.Count;
            slider.value = Mathf.Lerp(slider.value, targetValue, lerpSpeed);
        }
    }
    [Serializable]
    public class TapToPlayModule {
        public GameObject tapToPlayPanel;
        UIManager UIManager;
        public void SetActiveFalse() {
            tapToPlayPanel.SetActive(false);
        }
        public void Init(UIManager u) {
            UIManager = u;
        }
        public void Update() {
            if (UIManager.player.state == Player.States.Running) {
                tapToPlayPanel.SetActive(false);
            } 
        }
    }
    [Serializable]
    public class RestartModule {
        public GameObject restartPanel;
        UIManager UIManager;
        public float restartDelay;
        public void Init(UIManager u) {
            UIManager = u;
        }
        public IEnumerator Delay(float delay) {
            yield return new WaitForSeconds(delay);
                restartPanel.SetActive(true);
        }
        public void Update() {
            if (UIManager.player.state==Player.States.FailFinish || UIManager.player.state == Player.States.Burned) {
                UIManager.StartCoroutine(Delay(restartDelay));
            }
        }

    }
    [Serializable]
    public class WinModule {
        public GameObject winPanel;
        UIManager UIManager;
        public float winDelay;
        public void Init(UIManager u) {
            UIManager = u;
        }
        public IEnumerator Delay(float delay) {
            yield return new WaitForSeconds(delay);
            winPanel.SetActive(true);
        }
        public void Update() {
            if (UIManager.player.state == Player.States.WellFinish) {
                UIManager.StartCoroutine(Delay(winDelay));
            }
        }
    }
}