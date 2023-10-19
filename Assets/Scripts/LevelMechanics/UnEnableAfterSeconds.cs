using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnEnableAfterSeconds : MonoBehaviour
{

    public float UnEnableTime = 3;
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
        Invoke("DeActiveSelf", UnEnableTime);
    }

    void DeActiveSelf()
    {
        this.gameObject.SetActive(false);
    }
}
