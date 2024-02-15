using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] UnityEvent onAwake;
    [SerializeField] UnityEvent<GameObject> onEntered;
    [SerializeField] UnityEvent<GameObject> onExited;

    public void OnAwake() {
        onAwake?.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D c) {
        onEntered?.Invoke(c.gameObject);
    }
    public void OnTriggerExit2D(Collider2D c) {
        onExited?.Invoke(c.gameObject);
    }
}
