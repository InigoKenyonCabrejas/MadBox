using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneToChangeTo;
    [SerializeField] private List<string> scenesToAdd;
    [SerializeField] private Button button;

    private void Awake()
    {
        if(button != null)
        {
            button.onClick.AddListener(ChangeScene);
        }
    }

    private void Start()
    {
        if(button != null)
        {
            return;
        }

        ChangeScene();
    }

    private void OnDestroy()
    {
        if(button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(sceneToChangeTo);
        for(int i = 0; i < scenesToAdd.Count; i++)
        {
            SceneManager.LoadSceneAsync(scenesToAdd[i], LoadSceneMode.Additive);
        }
    }
}
