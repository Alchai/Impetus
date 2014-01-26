using UnityEngine;
using System.Collections;

public class HitBoxLogic : MonoBehaviour
{
    int me = 0;
    private GameObject mine, theirs;

    void Start()
    {
        if (name.Contains("1"))
            me = 1;
        if (name.Contains("2"))
            me = 2;
        if (name.Contains("3"))
            me = 3;
        if (name.Contains("4"))
            me = 4;

        mine = GameObject.Find("me");
        theirs = GameObject.Find("them");

    }

    void OnTriggerEnter(Collider co)
    {
        if (co.transform.tag.Contains("Hurt") && !co.transform.name.Contains(me.ToString()) && mine.GetComponent<Player>().isAttacking)
        {
            theirs.GetComponent<Player>().KnockBack();
            print("collided with em");
        }
    }

}
