using UnityEngine;
using System.Collections;

public class HitBoxLogic : MonoBehaviour
{
    int me = 0;
    private GameObject mine, theirs, smackParticles;
    private Client client;
    void Start()
    {
        client = GameObject.Find("Client").GetComponent<Client>();

        smackParticles = Resources.Load("Smack") as GameObject;

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



    void OnTriggerStay(Collider co)
    {
        if (co.transform.tag.Contains("Hurt") && !co.transform.name.Contains(me.ToString()) && mine.GetComponent<Player>().isAttacking)
        {
            print("hitconfirm!");
            mine.GetComponent<Player>().WinLoss_Ratio += 0.1f;
            
            if (mine.GetComponent<Player>().WinLoss_Ratio > 1.0f)
                mine.GetComponent<Player>().WinLoss_Ratio = 1.0f;
            
          //  GameObject.FindGameObjectWithTag("Text" + me.ToString()).GetComponent<TextMesh>().text = mine.GetComponent<Player>().WinLoss_Ratio.ToString();

        //    print("gained health. my health is now: "+mine.GetComponent<Player>().WinLoss_Ratio);

            try
            {

                theirs.GetComponent<Player>().KnockBack();
                client.networkView.RPC("SendKnockback", RPCMode.Server, client.mySID, client.playerNum);

            }
            catch
            {

                theirs = GameObject.Find("them");
                theirs.GetComponent<Player>().KnockBack();
                client.networkView.RPC("SendKnockback", RPCMode.Server, client.mySID, client.playerNum);


            }
            StartCoroutine("DisableCollider");
        }
    }

    private IEnumerator DisableCollider()
    {
        collider.enabled = false;

        while (mine.GetComponent<Player>().isAttacking)
            yield return new WaitForEndOfFrame();

        collider.enabled = true;

    }
}
