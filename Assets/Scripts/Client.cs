//Written by Daniel Stover and Steven Belowsky

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    #region Variables

    public int mySID = 0, inputDelay = 4, myChar, hisChar, playerNum;
    public int p1Char, p2Char;
    private float fullPacketLength;
    private List<float> timesSent = new List<float>(), timesRecd = new List<float>();

    private GameObject char1, char2, char3, char4;
    public GameObject me, them;

    public string CharUserName;


    #endregion

    void Start()
    {
        CharUserName = SystemInfo.deviceName;
        Network.Connect("72.238.29.102", 843);
        //Network.Connect("localhost", 843);
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;

        char1 = Resources.Load("Character1") as GameObject;
        char2 = Resources.Load("Character2") as GameObject;
        char3 = Resources.Load("Character3") as GameObject;
        char4 = Resources.Load("Character4") as GameObject;
    }

    void OnConnectedToServer()
    {
        StartCoroutine(speedTick());
    }

    private IEnumerator speedTick()
    {
        networkView.RPC("CalculateInputDelay", RPCMode.Server, Network.player);
        timesSent.Add(Time.time);

        for (int i = 0; i < 60; i++)
            yield return new WaitForEndOfFrame();

        StartCoroutine(speedTick());
    }

    [RPC]
    public void SendInput(int index, bool down_or_up, int seshID, bool p1_p2, Vector3 snapPos)
    {
        Player theirPlayer = them.GetComponent<Player>();
        //  print(them.name + " just received an input!");

        switch (index)
        {
            case 0:
                if (!down_or_up)
                {
                    if (!theirPlayer.isAttacking && !theirPlayer.isDashing)
                        theirPlayer.LeftPressed = true;
                }
                else
                    theirPlayer.LeftPressed = false;
                break;

            case 3:
                if (!down_or_up)
                {
                    if (!theirPlayer.isAttacking && !theirPlayer.isDashing)
                        theirPlayer.RightPressed = true;
                }
                else
                    theirPlayer.RightPressed = false;
                break;

            case 4:
                if (theirPlayer.canJump && !theirPlayer.isAttacking && !down_or_up)
                {
                    theirPlayer.Jump();
                    theirPlayer.transform.position = snapPos;
                    // print("enemy jumped, snapping to: " + snapPos);
                }
                break;
            case 5:
                if (!theirPlayer.isAttacking && !theirPlayer.isDashing && !down_or_up)
                {
                    theirPlayer.Attack_LightMelee(theirPlayer);
                    theirPlayer.transform.position = snapPos;
                }
                break;
            case 6:
                if (!theirPlayer.isDashing && !theirPlayer.isAttacking && !down_or_up)
                {
                    theirPlayer.Dash();
                    theirPlayer.transform.position = snapPos;
                }
                break;
            case 7:
                //ranged here
                break;
            default:
                break;
        }

        if (down_or_up)
        {
            switch (index)
            {
                case 0:
                    them.GetComponent<Player>().LeftPressed = false;
                    break;
                case 3:
                    them.GetComponent<Player>().RightPressed = false;
                    break;
                default:
                    break;
            }
        }
    }

    public void EndGame()
    {
        print("end game called locally in client");
        networkView.RPC("EndTheGame", RPCMode.Server, mySID);
    }

    [RPC]
    public void EndTheGame(int mysID)
    {
        print("loading level...");
        Application.LoadLevel("TestMenu");
        print("destroying client...");
        GameObject.Destroy(gameObject);
    }

    [RPC]
    public void GetSessionID(NetworkPlayer player, int sID, int opponentCharChoice, int myCharChoice, int whichPlayerAmI)
    {
        mySID = sID;
        myChar = myCharChoice;
        hisChar = opponentCharChoice;
        playerNum = whichPlayerAmI;
        Application.LoadLevel("CharacterSelect");
    }

    [RPC]
    public void CalculateInputDelay(NetworkPlayer player)
    {
        timesRecd.Add(Time.time);
        fullPacketLength = Mathf.Abs(timesSent[timesSent.Count - 1] - timesRecd[timesRecd.Count - 1]);
        inputDelay = Mathf.CeilToInt(fullPacketLength / (2f * .0167f));
    }

    [RPC]
    public void CreateCharacter(int p1Play, int p2Play, int whichPlayer, Vector3 pos, Vector3 rot, int SID, string playerName)
    {
        GameObject newobj = char1;
        int currentPlay = 0;

        if (whichPlayer == 1)
            currentPlay = p1Play;
        else
            currentPlay = p2Play;
        //  print("At creating of players p1 = " + p1Play + "p2= " + p2Play);

        switch (currentPlay)
        {
            case 1:
                newobj = GameObject.Instantiate(char1) as GameObject;
                newobj.transform.position = pos;
                newobj.transform.eulerAngles = rot;
                break;
            case 2:
                newobj = GameObject.Instantiate(char2) as GameObject;
                newobj.transform.position = pos;
                newobj.transform.eulerAngles = rot;
                break;
            case 3:
                newobj = GameObject.Instantiate(char3) as GameObject;
                newobj.transform.position = pos;
                newobj.transform.eulerAngles = rot;
                break;
            case 4:
                newobj = GameObject.Instantiate(char4) as GameObject;
                newobj.transform.position = pos;
                newobj.transform.eulerAngles = rot;
                break;
            default:
                Destroy(newobj);
                break;

        }

        // print("player " + whichPlayer + " wants to create char: " + whichChar);

        if (whichPlayer == playerNum)
        {
            newobj.AddComponent<InputManager>();
            //newobj.AddComponent<Player>();
            newobj.name = "me";
            me = newobj;
            //print(me);
            //print(CharUserName);
            me.transform.FindChild("3DText").GetComponent<TextMesh>().text = CharUserName;
            foreach (BlendGroup bg in GameObject.FindObjectsOfType<BlendGroup>())
            {
                bg.InitializeBlend();
            }
        }
        else
        {
            newobj.name = "them";
            // newobj.AddComponent<Player>();
            them = newobj;
            them.transform.FindChild("3DText").GetComponent<TextMesh>().text = playerName;
            foreach (BlendGroup bg in GameObject.FindObjectsOfType<BlendGroup>())
            {
                bg.InitializeBlend();
            }
        }
    }

    [RPC]
    public void JoinLobby(NetworkPlayer player)
    {

    }
    private int numcharselects = 0;

    [RPC]
    public void SelectCharacter(string who, int seshID, bool p1_p2)
    {
        numcharselects++;
        GameObject mybutton, theirbutton;
        if (!p1_p2)
        {
            mybutton = GameObject.Find("charone");
            theirbutton = GameObject.Find("chartwo");
        }
        else
        {
            mybutton = GameObject.Find("chartwo");
            theirbutton = GameObject.Find("charone");
        }
        if (who.Contains("1") && theirbutton.GetComponent<ButtonSelect>().currentSelection != 1)
        {
            mybutton.transform.position = GameObject.Find("topleftplayer").transform.position;
            mybutton.GetComponent<ButtonSelect>().currentSelection = 1;
        }
        else if (who.Contains("2") && theirbutton.GetComponent<ButtonSelect>().currentSelection != 2)
        {
            mybutton.transform.position = GameObject.Find("toprightplayer").transform.position;
            mybutton.GetComponent<ButtonSelect>().currentSelection = 2;
        }
        else if (who.Contains("3") && theirbutton.GetComponent<ButtonSelect>().currentSelection != 3)
        {
            mybutton.transform.position = GameObject.Find("bottomleftplayer").transform.position;
            mybutton.GetComponent<ButtonSelect>().currentSelection = 3;
        }
        else if (who.Contains("4") && theirbutton.GetComponent<ButtonSelect>().currentSelection != 4)
        {
            mybutton.transform.position = GameObject.Find("bottomrightplayer").transform.position;
            mybutton.GetComponent<ButtonSelect>().currentSelection = 4;
        }
        p1Char = mybutton.GetComponent<ButtonSelect>().currentSelection;
        p2Char = theirbutton.GetComponent<ButtonSelect>().currentSelection;
    }

    void OnGUI()
    {
        if (Network.isClient)
        {
            GUILayout.Label("Connected To Server!");
            if (mySID == 0)
            {
                GUILayout.Label("My session ID is: " + mySID);
                GUILayout.Label("I am player: " + playerNum);
                GUILayout.Label("Input delay: " + inputDelay);
                GUILayout.Label("Last Round Trip: " + fullPacketLength);
                GUILayout.Label("num sent char selects: " + numcharselects);
            }
            else
                GUILayout.Label("Awaiting opponent...");
        }
        else
            GUILayout.Label("Not connected to server");
    }
}
