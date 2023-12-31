using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cherrydev;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    [SerializeField] private int goodEndingThreshold;
    [SerializeField, Range(0f, 5f)] private float transitionDelay;

    public DayDialogue dayOne;
    public DayDialogue dayTwo;
    public DayDialogue dayThree;
    public DayDialogue dayFour;
    public DayDialogue dayFive;

    public List<DialogNodeGraph> badEndings;

    private DayDialogue currentDay;
    private List<DayDialogue> dayCollection = new List<DayDialogue>();
    private int day;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            PointSystem.ResetPoints();
            DontDestroyOnLoad(gameObject);
        } else {
            Instance.OnNodeGraphStarted();
            Destroy(gameObject);
        }
        dayCollection.Add(dayOne);
        dayCollection.Add(dayTwo);
        dayCollection.Add(dayThree);
        dayCollection.Add(dayFour);
        dayCollection.Add(dayFive);
        if (TestScriptDebug.isTesting) {
            day = TestScriptDebug.chooseDay - 1;
            currentDay = dayCollection[day++];
            currentDay.currentDialogue = TestScriptDebug.chooseCharacter;
        } else {
            day = 0;
            currentDay = dayCollection[day++];
        }
    }

    private void Start() {
        OnNodeGraphStarted();
    }

    public void OnNodeGraphStarted() {
        string result = MiniGameResult.GetResult();
        MiniGameResult.ResetMinigame();
        if (DialogBehaviour.Instance != null) {
            DialogBehaviour.Instance.AddListenerToOnDialogFinished(OnNodeGraphFinished);
        }
        if (result == string.Empty) {
            if (currentDay.IsOutOfDialogue()) {
                if (day == dayCollection.Count) {
                    //Signal End of Game
                    StartCoroutine(TransitionToEndCredits());
                    return;
                }
                currentDay = dayCollection[day++];
                StartCoroutine(TransitionToNewDay());
                return;
            }

            Debug.Log($"Speaker {currentDay.currentDialogue - 1} has {PointSystem.GetPoints(currentDay.currentDialogue - 1)} points");

            if (day == 5 && currentDay.currentDialogue > 0 && PointSystem.GetPoints(currentDay.currentDialogue - 1) < goodEndingThreshold) {
                Debug.Log($"Speaker {currentDay.currentDialogue - 1} Bad Ending");
                DialogBehaviour.Instance.StartDialog(badEndings[currentDay.currentDialogue - 1]);
            } else {
                DialogBehaviour.Instance.StartDialog(currentDay.GetCurrentConversation());
            }
        } else {
            DialogBehaviour.Instance.StartDialog(currentDay.GetNextConversation(), result);
        }
    }

    public void OnNodeGraphFinished() {
        DialogNodeGraph currentNodeGraph = DialogBehaviour.Instance.GetCurrentDialogNodeGraph();

        //if (currentDay.currentDialogue > 0 && currentDay.dialogGraphs.Contains(currentNodeGraph)) {
        if (currentDay.HasNextNodeGraph(currentNodeGraph)) { 
            StartCoroutine(TransitionToMinigame());
        } else {
            StartCoroutine(TransitionToNextCustomer());
        }
    }

    IEnumerator TransitionToMinigame() {
        yield return new WaitForSeconds(transitionDelay);

        AudioManager.Instance.PlayMiniGameBackground();
        Loader.Instance.PlayMinigame();
    }
    IEnumerator TransitionToNextCustomer() {
        currentDay.currentDialogue++;

        yield return new WaitForSeconds(transitionDelay);

        OnNodeGraphStarted();
    }

    IEnumerator TransitionToNewDay() {
        yield return new WaitForSeconds(transitionDelay);

        Loader.Instance.LoadNewDay();
    }

    IEnumerator TransitionToEndCredits() {
        yield return new WaitForSeconds(transitionDelay);
        Loader.Instance.LoadEndGame();
    }

    public int GetDay() {
        return day;
    }

    public void EndGame() {
        Instance = null;
        Destroy(gameObject);
    }
}

[System.Serializable]
public class DayDialogue {
    public List<DialogNodeGraph> dialogGraphs;
    [HideInInspector] public int currentDialogue = 0;

    public DialogNodeGraph GetCurrentConversation() {
        return dialogGraphs[currentDialogue];
    }

    public DialogNodeGraph GetNextConversation() {
        return dialogGraphs[currentDialogue].nextDialog;
    }

    public bool HasNextNodeGraph(DialogNodeGraph dialogNodeGraph) {
        return dialogNodeGraph.nextDialog != null;
    }

    public bool IsOutOfDialogue() {
        return currentDialogue >= dialogGraphs.Count;
    }
}