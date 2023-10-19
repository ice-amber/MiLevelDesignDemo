using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool IsNeedInteractionActive;

    public bool IsNeedKeyToInteration;

    public GameObject Key;

    public GameObject[] GameObjectsToBeEnabledWhenPressInteraction;

    public GameObject[] GameObjectsToBeUnEnabledWhenPressInteraction;

    public GameObject[] GameObjectsToBeEnabledWhenEnter;
    
    public GameObject[] GameObjectsToBeUnEnabledWhenExit;

    public GameObject[] GameObjectsToBeEnabledWhenStay;

    Collider thisTrigger;

    CharacterInputSystem _inputSystem;

    bool IsNeedToCheckInteraction = false;
    // Start is called before the first frame update
    void Start()
    {
        _inputSystem = FindObjectOfType<CharacterInputSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsNeedInteractionActive)
        {
            if (!IsNeedKeyToInteration)
            {
                if (IsNeedToCheckInteraction)
                {
                    if (_inputSystem.Interaction)
                    {
                        for (int i = 0; i < GameObjectsToBeEnabledWhenPressInteraction.Length; i++)
                        {
                            if (GameObjectsToBeEnabledWhenPressInteraction[i] != null)
                            {
                                GameObjectsToBeEnabledWhenPressInteraction[i].SetActive(true);
                            }
                        }

                        for (int i = 0; i < GameObjectsToBeUnEnabledWhenPressInteraction.Length; i++)
                        {
                            if (GameObjectsToBeUnEnabledWhenPressInteraction[i] != null)
                            {
                                GameObjectsToBeUnEnabledWhenPressInteraction[i].SetActive(false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (IsNeedToCheckInteraction)
                {
                    if (_inputSystem.Interaction && Key.activeSelf == true)
                    {
                        for (int i = 0; i < GameObjectsToBeEnabledWhenPressInteraction.Length; i++)
                        {
                            if (GameObjectsToBeEnabledWhenPressInteraction[i] != null)
                            {
                                GameObjectsToBeEnabledWhenPressInteraction[i].SetActive(true);
                            }
                        }

                        for (int i = 0; i < GameObjectsToBeUnEnabledWhenPressInteraction.Length; i++)
                        {
                            if (GameObjectsToBeUnEnabledWhenPressInteraction[i] != null)
                            {
                                GameObjectsToBeUnEnabledWhenPressInteraction[i].SetActive(false);
                            }
                        }
                    }
                }
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player in Trigger");

            for (int i = 0; i < GameObjectsToBeEnabledWhenEnter.Length; i++)
            {
                if (GameObjectsToBeEnabledWhenEnter[i] != null)
                {
                    GameObjectsToBeEnabledWhenEnter[i].SetActive(true);
                }
            }

            for (int i = 0; i < GameObjectsToBeEnabledWhenStay.Length; i++)
            {
                if (GameObjectsToBeEnabledWhenStay[i] != null)
                {
                    GameObjectsToBeEnabledWhenStay[i].SetActive(true);
                }
            }

            IsNeedToCheckInteraction = true;
        }

        


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player Exit Trigger");

            for (int i = 0; i < GameObjectsToBeUnEnabledWhenExit.Length; i++)
            {
                if (GameObjectsToBeUnEnabledWhenExit[i] != null)
                {
                    GameObjectsToBeUnEnabledWhenExit[i].SetActive(false);
                }
            }

            for (int i = 0; i < GameObjectsToBeEnabledWhenStay.Length; i++)
            {
                if (GameObjectsToBeEnabledWhenStay[i] != null)
                {
                    GameObjectsToBeEnabledWhenStay[i].SetActive(false);
                }
            }

            IsNeedToCheckInteraction = false;
        }

    }

}
