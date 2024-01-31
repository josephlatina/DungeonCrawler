using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// a user interface that responds to internal state changes
[RequireComponent(typeof(PlayerController))]
public class PlayerStateView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;

    [SerializeField] private PlayerController player;
    private PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Start() {
        // cache to save typing
        playerStateMachine = player.PlayerStateMachine;
        // listen for any state changes
        playerStateMachine.stateChanged += OnStateChanged;
    }

    void OnDestroy()
    {
        // unregister the subscription if we destroy the object
        playerStateMachine.stateChanged -= OnStateChanged;
    }

    // change the UI.Text when the state changes
    private void OnStateChanged(IState state)
    {
        if (labelText != null)
        {
            labelText.text = state.GetType().Name;
        }
    }

}
