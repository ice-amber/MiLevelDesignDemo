using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAndHideCursor : MonoBehaviour
{
    public bool isLocked = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
/*        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isLocked = !isLocked;
        }*/

        Cursor.visible = !isLocked;
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
