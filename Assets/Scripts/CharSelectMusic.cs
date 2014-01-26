using UnityEngine;
using System.Collections;

public class CharSelectMusic : MonoBehaviour 
{
    Client client;
    AudioSource [] sources;
	void Start () 
    {
        client = GameObject.Find("Client").GetComponent<Client>();
        sources = this.GetComponents<AudioSource>();
        for (int i = 0; i < 4; ++i)
        {
            try 
            { 
                sources[i].mute = true;
                sources[i].volume = 0.0f;
            }
            catch { }
        }
	}

    void Update() 
    {
        switch (client.myChar)
        {
            case 0:
                sources[0].mute = false;
                sources[0].volume = 1.0f;
                for (int i = 1; i < 4; ++i)
                {
                    sources[i].mute = true;
                    sources[i].volume = 0;
                }
                break;
            case 1:
                for (int i = 0; i < 4; ++i)
                {
                    sources[i].mute = true;
                     sources[i].volume = 0;
                }
                sources[1].mute = false;
                sources[1].volume = 1.0f;
                break;
            case 2:
                for (int i = 0; i < 4; ++i)
                {
                    sources[i].mute = true;
                     sources[i].volume = 0;
                }
                sources[2].mute = false;
                sources[2].volume = 1.0f;
                break;
            case 3:
                for (int i = 0; i < 3; ++i)
                {
                    sources[i].mute = true;
                    sources[i].volume = 0;
                }
                sources[3].mute = false;
                sources[3].volume = 1.0f;
                break;
            default:
                break;
        }
	}
}
