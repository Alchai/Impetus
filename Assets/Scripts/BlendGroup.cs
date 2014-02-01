using UnityEngine;
using System.Collections;

public class BlendGroup : MonoBehaviour
{
    private Player player1;
    private Player player2;
    private GameObject fader;
    public Texture2D[] textures;
    public float LerpValue = 1.0f;

    public void InitializeBlend()
    {
        fader = GameObject.Find("Fader");

        if (name.Contains("SM_Left_Platform"))
            fader.GetComponent<AlphaFade>().FadeOut();

        // Initialize
        player1 = GameObject.Find("me").GetComponent<Player>();
        player2 = GameObject.Find("them").GetComponent<Player>();
        int index = 0;

        // Initialize the main texture for each child to the texture relative to the character
        if (player1.tag == "Character 1")
            index = 0;
        else if (player1.tag == "Character 2")
            index = 1;
        else if (player1.tag == "Character 3")
            index = 2;
        else
            index = 3;
        renderer.material.SetTexture("_MainTex", textures[index]);
    }

    // Update is called once per frame
    // BlendGroup will access the variable in the player responsible for handling the blending of both textures (ratio from 0.0 to 1.0)
    // To have a more drastic change, consider waiting a certain amount of frames before you update the LerpValue
    // BlendGroup will then assign the variable in the pixel shader each frame
    void Update()
    {

    }
}
