﻿using UnityEngine;
using System.Collections;

public class BattleLogic : MonoBehaviour
{
    private Client client;

    void Start()
    {
        client = GameObject.Find("Client").GetComponent<Client>();
        StartCoroutine("SendNewChars");
    }

	private IEnumerator SendNewChars()
    {
        yield return new WaitForSeconds(5f);
        Vector3 rotOffset = new Vector3(0f, -90f, 0f),
            posOffset = new Vector3(10f, 0f, 0f);

        if (client.playerNum == 1)
        {
            rotOffset = new Vector3(0f, 90f, 0f);
            posOffset = new Vector3(-10f, 0f, 0f);
        }
       
        print("p1char: " + client.p1Char);
        print("p2char: " + client.p2Char);

        client.networkView.RPC("CreateCharacter", RPCMode.Server, client.p1Char, client.p2Char, client.playerNum, posOffset, rotOffset, client.mySID, client.CharUserName);
    }
}
