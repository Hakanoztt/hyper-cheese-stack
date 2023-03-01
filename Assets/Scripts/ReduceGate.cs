using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReduceGate : MonoBehaviour {
    public int ReducedAmount;
    public TextMeshPro reducedText;
    public ParticleSystem effect;

    void Start() {
        reducedText.text = ReducedAmount.ToString();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("CheeseObject") || other.CompareTag("Player")) {
            Instantiate(effect, transform.position, transform.rotation);
        }
    }
}
