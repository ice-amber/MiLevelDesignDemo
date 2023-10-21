using System.Collections;
using System.Collections.Generic;
using UGG.Move;
using UnityEngine;

public class TeleportToPosition : MonoBehaviour
{

    //public Transform TeleportTargetPosition;

    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        //Player = FindObjectOfType<PlayerMovementController>();
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.root.position = this.transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        this.gameObject.SetActive(false);
    }
}
