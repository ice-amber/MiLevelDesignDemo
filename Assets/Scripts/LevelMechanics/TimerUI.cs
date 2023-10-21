using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private int LastCountTime = 0;
    private float currentTime = 0;
    private int MaxTime = 30;
    public bool IsStopCount;
    public GameObject[] EnableObjects;
    public GameObject[] UnEnableObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsStopCount)
        {
            currentTime += Time.deltaTime;
            if (currentTime - LastCountTime >= 1)
            {
                MaxTime--;
                LastCountTime++;
                this.GetComponent<TextMeshProUGUI>().text = MaxTime.ToString();
            }
            if (MaxTime == 0)
            {
                IsStopCount = true;
                OnCounterTo0();
            }
        }

    }

    private void OnEnable()
    {
        MaxTime = 30;
        LastCountTime = 0;
        currentTime = 0;
        IsStopCount = false;
        this.GetComponent<TextMeshProUGUI>().text = MaxTime.ToString();
    }

    private void OnCounterTo0()
    {

        for (int i = 0; i < EnableObjects.Length; i++)
        {
            if (EnableObjects[i] != null)
            {
                EnableObjects[i].SetActive(true);
            }
        }

        for (int i = 0; i < UnEnableObjects.Length; i++)
        {
            if (UnEnableObjects[i] != null)
            {
                UnEnableObjects[i].SetActive(false);
            }
        }
    }

    public void StopCount()
    {
        IsStopCount = true;
    }

}
