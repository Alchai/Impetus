using UnityEngine;
using System.Collections;

public class ButtonSelect : MonoBehaviour
{
    public int currentSelection = 0;

    void Start()
    {
        if (name.Contains("one"))
            currentSelection = 1;
        else if(name.Contains("two"))
            currentSelection = 2;
    }

}
