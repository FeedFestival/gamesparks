using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Assets.Scripts.Utils;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private SceneManager _sceneManager;

    public void Init(SceneManager sceneManager)
    {
        _sceneManager = sceneManager;

        FillScope();

        StartCoroutine(SetupStyles());
    }

    private void FillScope()
    {
        _sceneManager.Game.scope = new Dictionary<string, GameObject>();

        // 1.   -> fill scope

        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name.Contains("{"))
            {
                var s = child.gameObject.name.Split('{', '}');
                try
                {
                    _sceneManager.Game.scope.Add(s[1], child.gameObject);
                }
                catch (Exception exception)
                {
                    Debug.LogError("scope[\"" + child.gameObject.name + "\"] - Allready exists. \n" + exception);
                    throw;
                }
            }
        }
    }

    IEnumerator SetupStyles()
    {
        yield return new WaitForSeconds(0.5f);

        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            var style = child.gameObject.GetComponent<style>();
            if (style != null)
                style.Setup();
        }
    }

    public Dictionary<string, GameObject> InitViews(Dictionary<string, GameObject> scope)
    {
        scope["MenuView"].SetActive(false);
        scope["MainMenuView"].SetActive(false);
        scope["LoginContainer"].SetActive(false);
        scope["FriendListContainer"].SetActive(false);

        return scope;
    }
}