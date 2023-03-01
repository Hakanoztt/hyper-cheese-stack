using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepCheeseForMouse {
    public class Tower : MonoBehaviour {
        public GameObject arrow;
        public Transform arrowStartingPos;
        public float arrowThrowDelay;
        void Start() {
            //StartCoroutine(InstantiateArrow(arrowThrowDelay))
            InstantiateArrow();
        }
        void InstantiateArrow() {
            Instantiate(arrow, arrowStartingPos.position, arrow.transform.rotation);
            Invoke("InstantiateArrow", arrowThrowDelay);
        }
    }
}