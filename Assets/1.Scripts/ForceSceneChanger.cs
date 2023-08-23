using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSceneChanger : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            StartCoroutine(SceneChanger.Instance.ChangeSceneStart("Boss"));
    }
}
