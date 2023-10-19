using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{

    public LockController lockcontrol;

    void Start()
    {
        lockcontrol = FindObjectOfType<LockController>();

    }

    private void Update()
    {
/*        if (Vector2.Distance(Camera.main.WorldToScreenPoint(this.transform.position), new Vector2(Screen.width / 2, Screen.height / 2)) > (Screen.width / 2))
        {
            OnBecameInvisible();
        }*/
    }

    private void OnBecameVisible()
    {
        if (!lockcontrol.screenTargets.Contains(transform))
            lockcontrol.screenTargets.Add(transform);
    }

    private void OnBecameInvisible()
    {
        if(lockcontrol.screenTargets.Contains(transform))
            lockcontrol.screenTargets.Remove(transform);
    }
}
