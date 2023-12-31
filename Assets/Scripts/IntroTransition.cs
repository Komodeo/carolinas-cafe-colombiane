using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroTransition : MonoBehaviour
{
    [SerializeField] Button StartGameButton;
    [SerializeField] Button OptionButton;
    [SerializeField] Button CreditsButton;
    [SerializeField] GameObject OptionPage;
    [SerializeField] GameObject CreditPage;
    [SerializeField] TransitionSettings transitionSettings;


    private void Awake()
    {
        StartGameButton.onClick.AddListener(StartGame);
        OptionButton.onClick.AddListener(OptionPageActive);
        CreditsButton.onClick.AddListener(CreditPageActive);
    }

    private void Start() {
        DayManager dayManagerInstance = DayManager.Instance;
        if (dayManagerInstance != null) {
            CreditPageActive();
            dayManagerInstance.EndGame();
        }
    }

    private void CreditPageActive()
    {
        if (OptionPage != null && CreditPage != null)
        {
            if (CreditPage.activeInHierarchy)
            {
                OptionPage.SetActive(false);
                CreditPage.SetActive(false);
            }
            else
            {
                AudioManager.Instance.PlaySadBackground();
                OptionPage.SetActive(false);
                CreditPage.SetActive(true);
            }
        
        }
        
    }

    public void CloseCreditPage() {
        CreditPage.SetActive(false);
        AudioManager.Instance.PlayGameLoopBackground();
    }

    private void OptionPageActive()
    {
        if(OptionPage!=null && CreditPage != null)
        {

            if (OptionPage.activeInHierarchy)
            {
                OptionPage.SetActive(false);
                CreditPage.SetActive(false);
            }
            else
            {
                OptionPage.SetActive(true);
                CreditPage.SetActive(false);
            }

         
        }
    }

    public void StartGame()
    {
        //   string _sceneName = NameFromIndex((SceneManager.GetActiveScene().buildIndex) + 1);
        string _sceneName = NameFromIndex(1);
        EasyTransition.TransitionManager.Instance().Transition(_sceneName, transitionSettings, 0);
    }

    public void StartHardcore()
    {
        //TODO trigger reloadLevel or restart game
        PlayerPrefs.SetInt("HardcoreCheck", 1);
        //PlayerPrefs.SetInt("TemporaryScore", 0);
        //PlayerPrefs.SetInt("CurrentScore", 0);
        string _sceneName = NameFromIndex(3);
        EasyTransition.TransitionManager.Instance().Transition(_sceneName, transitionSettings, 0.5f);
    }

    public void StartAdventure()
    {
        //TODO trigger reloadLevel or restart game
        string _sceneName = NameFromIndex(3);
        PlayerPrefs.SetInt("HardcoreCheck", 0);
        //PlayerPrefs.SetInt("TemporaryScore", 0);
        //PlayerPrefs.SetInt("CurrentScore", 0);
        EasyTransition.TransitionManager.Instance().Transition(_sceneName, transitionSettings, 0.5f);
    }

    public void MainMenu()
    {
        //TODO trigger reloadLevel or restart game
        string _sceneName = NameFromIndex(0);
        //PlayerPrefs.SetInt("TemporaryScore", 0);
        //PlayerPrefs.SetInt("CurrentScore", 0);
        EasyTransition.TransitionManager.Instance().Transition(_sceneName, transitionSettings, 0);
    }


    private static string NameFromIndex(int BuildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }

}
