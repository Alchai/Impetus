using UnityEngine;
using System.Collections;

public class LoadMenu : MonoBehaviour
{

    void Update()
    {
        if (Input.anyKey)
        {
            Application.LoadLevel("TestMenu");
        }
    }

}
	
