using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepCheeseForMouse {
    public class Player : MonoBehaviour {
        private const string _playerTag = "Player";
        public bool burned = false;

        public MovementModule movementModule;
        public StackManager stackManager;
        public CheeseObject cheeseObject;
        public ListTriggerControl triggerControl;
        public StateControl stateControl;
        public GameController gameController;

        public HealthManager deathManager;

        public GameObject endMouse;
        public Animator animator;
        public ParticleSystem finishEffect;
        public enum States {
            Idle,
            Running,
            Holding,
            Burned,
            WellFinish,
            FailFinish
        }
        public States state;
        void Start() {
            movementModule.Init(this);
            stackManager.Init(this);
            triggerControl.Init(this);
            deathManager.Init(this);
            stateControl.Init(this);
        }
        void Update() {
            movementModule.Update();
            deathManager.Update();
            stateControl.Update();
            stackManager.Update();
        }
        public static bool TryGetCharacter(Collider other, out Player player) {
            if (other.CompareTag(_playerTag)) {
                if (other.TryGetComponent(out Player p)) {
                    player = p;
                    return true;
                }
            }
            player = null;
            return false;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("FinishLine")) {
                if (stackManager.cheeseList.Count >= gameController.requiedCheese) {
                    gameController.finishManager.Finish(true);
                    Instantiate(finishEffect, transform.position, transform.rotation);
                    Instantiate(finishEffect, transform.position, transform.rotation);
                } else {
                    gameController.finishManager.Finish(false);
                }
            }
            if (other.CompareTag("ReduceGate")) {
                other.gameObject.transform.parent.gameObject.SetActive(false);
                gameController.finishManager.Finish(false);
            }
            if (other.CompareTag("IncreaseGate")) {
                var increaseGate = other.GetComponent<IncreaseGate>();
                for (int i = 0; i < increaseGate.increasedAmount; i++) {
                    stackManager.GetCheeseObject();
                }
                other.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public class MovementModule {
            Player player;

            public float moveSpeed;
            public float sideSpeed;

            public float minX;
            public float maxX;

            float lastFrameFingerPositionX;
            float moveFactorX;
            public void Init(Player p) {
                player = p;
                Time.timeScale = 1;
            }
            public void Update() {
                Move();
            }
            public void Move() {
                if (Input.GetMouseButtonDown(0) && player.state == States.Idle) {
                    player.state = States.Running;
                    player.animator.SetBool("IsPressed", true);
                }
                if (player.state == States.Running || player.state == States.Holding) {
                    player.transform.Translate(new Vector3(moveFactorX * Time.deltaTime * sideSpeed, 0, moveSpeed * Time.deltaTime));
                    if (Input.GetMouseButtonDown(0)) {
                        lastFrameFingerPositionX = Input.mousePosition.x;
                    } else if (Input.GetMouseButton(0)) {
                        moveFactorX = Input.mousePosition.x - lastFrameFingerPositionX;
                        lastFrameFingerPositionX = Input.mousePosition.x;
                    }
                    if (Input.GetMouseButtonUp(0)) {
                        moveFactorX = 0;
                    }
                    // moveFactorX = Mathf.Clamp(moveFactorX,  minX, maxX);
                }
            }
        }
        [Serializable]
        public class StackManager {
            Player player;
            public List<CheeseObject> cheeseList;
            public CheeseObject cheesePrefab;
            public Transform stackPosition;

            public float smoothLerpX;
            public float smoothLerpXxxxx;

            public float smoothLerpZ;

            public int arrowIndex;
            public void Init(Player p) {
                player = p;
                cheeseList = new List<CheeseObject>();
            }
            public void Add(CheeseObject cheeseObject) {
                if (player.state == States.Running || player.state == States.Holding) {
                    if (!cheeseList.Contains(cheeseObject)) {
                        player.state = States.Holding;
                        player.animator.SetBool("IsHolding", true);
                        player.animator.SetBool("Running", false);
                        cheeseList.Add(cheeseObject);
                        cheeseObject.OnTriggerEntered += player.triggerControl.OnCheeseTriggerEnter;
                        // yeni eleman eklendikçe sondan baþlayýp hepsini yukarý aþaðý yap.?
                    }
                }
            }
            public void Remove(CheeseObject cheeseObject) {
                cheeseList.Remove(cheeseObject);
                cheeseObject.GetComponent<Collider>().enabled = false;
                if (cheeseList.Count == 0) {
                    player.animator.SetBool("Running", true);
                    player.animator.SetBool("IsHolding", false);
                }
                cheeseObject.removed = true;
            }
            public CheeseObject GetCheeseObject() {
                CheeseObject cheeseObject = Instantiate(cheesePrefab, stackPosition.position, Quaternion.identity);
                Add(cheeseObject);
                return cheeseObject;
            }
            public void Update() {
                StackMovement();
            }
            public void StackMovement() {
                for (int i = 0; i < cheeseList.Count; i++) {
                    var targetPos = stackPosition.position + (Vector3.forward) * (i + 1) * 1.4f;
                    if (i != 0) {
                        targetPos.x = Mathf.Lerp(cheeseList[i].transform.position.x, targetPos.x, smoothLerpX * Time.deltaTime / (i + 1));
                        targetPos = Vector3.MoveTowards(cheeseList[i].transform.position, targetPos, smoothLerpZ * Time.deltaTime);
                    }
                    cheeseList[i].transform.position = targetPos;
                }
            }
        }
        [Serializable]
        public class ListTriggerControl {
            Player player;
            public void Init(Player player) {
                this.player = player;
            }
            public void OnCheeseTriggerEnter(Collider obj) {

                if (obj.TryGetComponent(out CheeseObject cheese)) {
                    if (!player.stackManager.cheeseList.Contains(cheese)) {
                        player.stackManager.Add(cheese);
                    }
                }
                if (obj.TryGetComponent(out ReduceGate reduceGate)) {

                    if (player.stackManager.cheeseList.Count >= reduceGate.ReducedAmount) {
                        for (int i = 0; i < reduceGate.ReducedAmount; i++) {
                            player.stackManager.Remove(player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1]);
                        }
                    } else {
                        var reduced = player.stackManager.cheeseList.Count;
                        for (int i = 0; i < reduced; i++) {
                            player.stackManager.Remove(player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1]);
                        }
                        player.state = States.FailFinish;
                    }
                    reduceGate.gameObject.transform.parent.gameObject.SetActive(false);
                }
                if (obj.TryGetComponent(out IncreaseGate increaseGate)) {
                    for (int i = 0; i < increaseGate.increasedAmount; i++) {
                        player.stackManager.GetCheeseObject();
                    }
                    increaseGate.gameObject.transform.parent.gameObject.SetActive(false);
                }
                if (obj.TryGetComponent(out Blade blade)) {
                    player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1].gameObject.transform.position = blade.gameObject.transform.position;
                    player.stackManager.Remove(player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1]);
                }
                if (obj.TryGetComponent(out BurnerObject burnerObject)) {
                    player.stackManager.Remove(player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1]);
                }
                if (obj.TryGetComponent(out Arrow arrow)) {
                    if (player.stackManager.cheeseList.Count >= arrow.reduceCheese) {
                        for (int i = arrow.reduceCheese; i > 0; i--) {
                            player.stackManager.Remove(player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1]);
                        }
                        player.animator.SetTrigger("Reaction");
                    } else {
                        player.deathManager.MakeDeath();
                        player.state = States.FailFinish;
                    }
                    arrow.gameObject.SetActive(false);
                }
                if (obj.TryGetComponent(out RotatingObstacle rotatingObstacle)) {
                    var lastCheese = player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1].gameObject;
                    lastCheese.GetComponent<Rigidbody>().AddForce(new Vector3(150, 0, 0));
                    player.stackManager.Remove(player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1]);
                    // çarpan objeyi burda removelamam mümkün mü?
                }

            }
        }
        [Serializable]
        public class StateControl {
            Player player;
            public void Init(Player p) {
                player = p;
                player.state = States.Idle;
            }
            public void Update() {
                switch (player.state) {
                    case States.Idle:
                        break;
                    case States.Running:
                        break;
                    case States.Holding:
                        break;
                    case States.Burned:
                        if (player.transform.localScale.y > 0) {
                            player.transform.localScale -= new Vector3(0.005f, 0.01f, 0);
                        }
                        player.animator.SetBool("Running", false);
                        break;
                    case States.WellFinish:
                        player.animator.SetBool("FinishedWell", true);
                        Debug.Log("Finished Well.");
                        player.endMouse.GetComponent<Animator>().SetBool("Victory", true);
                        break;
                    case States.FailFinish:
                        player.animator.SetBool("FinishedFail", true);
                        Debug.Log("Finished fail");
                        player.endMouse.GetComponent<Animator>().SetBool("Defeat", true);
                        break;
                    default:
                        break;
                }
            }
        }
        public struct HealthManager {
            Player player;
            public bool canDeath;
            public void Init(Player p) {
                player = p;
                canDeath = false;
            }
            public void Update() {
                IsDeath();
            }
            public void IsDeath() {
                if (canDeath) {
                    player.movementModule.moveSpeed = 0;
                    player.movementModule.sideSpeed = 0;
                    player.animator.SetBool("Death", true);
                    for (int i = 0; i < player.stackManager.cheeseList.Count; i++) {
                        player.stackManager.Remove(player.stackManager.cheeseList[player.stackManager.cheeseList.Count - 1]);
                    }
                }
            }
            public void MakeDeath() {
                canDeath = true;
            }
        }
    }
}