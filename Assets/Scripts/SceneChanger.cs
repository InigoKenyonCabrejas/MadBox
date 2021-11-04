using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToChangeTo;
    public List<string> scenesToAdd;

    private void Start()
    {
        SceneManager.LoadScene(sceneToChangeTo);
        for(int i = 0; i < scenesToAdd.Count; i++)
        {
            SceneManager.LoadSceneAsync(scenesToAdd[i], LoadSceneMode.Additive);
        }
    }
}
