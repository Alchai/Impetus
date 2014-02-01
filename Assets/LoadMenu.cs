using UnityEngine;
using System.Collections;

public class LoadMenu : MonoBehaviour
{
    private bool canswitch = false;

    void Start()
    {
        StartCoroutine("switcher");
    }

    private IEnumerator switcher()
    {
        for (int i = 0; i < 120; i++)
            yield return new WaitForEndOfFrame();

        canswitch = true;
    }

    void Update()
    {
        if (Input.anyKey && canswitch)
        {
            Application.LoadLevel("TestMenu");
        }
    }

}

