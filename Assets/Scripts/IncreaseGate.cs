using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IncreaseGate : MonoBehaviour {
    public int increasedAmount;
    public TextMeshPro increasedText;
    public ParticleSystem effect;

    void Start() {
        increasedText.text = increasedAmount.ToString();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("CheeseObject") || other.CompareTag("Player")) {
            Instantiate(effect, transform.position, transform.rotation);
        }
    }
}
