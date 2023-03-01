using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour {
    public float rotateSpeed;
    public float triggerDelay;

    void Update() {
        Rotate();
    }
    void Rotate() {
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("CheeseObject")) {
            gameObject.GetComponent<Collider>().enabled = false;
            Invoke("EnableCollider", triggerDelay);
        }
    }
    void EnableCollider() {
        gameObject.GetComponent<Collider>().enabled = true;
    }
}
