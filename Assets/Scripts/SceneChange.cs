using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    string l_name;

    public void MoveToScene(string name)
    {
        l_name = name;
        Invoke("GoScene", 0.8f);
    }

    private void GoScene()
    {
        SceneManager.LoadScene(l_name);
    }
}
