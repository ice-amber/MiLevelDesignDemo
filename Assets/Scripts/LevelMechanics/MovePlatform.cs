using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public GameObject LowestPosition;
    public GameObject HighestPosition;
    public float MovingRate;
    public bool IsCanMove;
    public bool IsNeedToResetPosition = false;
    private Vector3 FirstPosition;

    protected CharacterInputSystem _inputSystem;

    // Start is called before the first frame update
    void Start()
    {
        _inputSystem = FindObjectOfType<CharacterInputSystem>();
        FirstPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCanMove)
        {
            if (this.transform.position.y < HighestPosition.transform.position.y)
            {
                if (_inputSystem.PlatformUp)
                {
                    this.transform.position += new Vector3(0, MovingRate * Time.deltaTime, 0);
                }

            }
            if (this.transform.position.y > LowestPosition.transform.position.y)
            {
                if (_inputSystem.PlatformDown)
                {
                    this.transform.position -= new Vector3(0, MovingRate * Time.deltaTime, 0);
                }
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player in Trigger");

            IsCanMove = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player Exit Trigger");

            IsCanMove = false;

            if (IsNeedToResetPosition)
            {
                this.transform.position = FirstPosition;
            }
        }

    }
}
