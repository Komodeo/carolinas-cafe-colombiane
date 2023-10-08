using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cherrydev;

public class DayManager : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;

    private void Start() {
        dialogBehaviour.StartDialog(dialogGraph);
    }
}
