using UnityEngine;
using System.Collections;

public class DestroyExtraneousClient : MonoBehaviour 
{
	void Start () 
    {
	    

	}
	
	void Update () 
    {
      Client [] clients =   GameObject.FindObjectsOfType<Client>();
      if (clients.Length > 1)
          Destroy(clients[1]);
	}
}
