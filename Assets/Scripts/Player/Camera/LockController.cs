using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;
using System;
using UnityEngine.Rendering.PostProcessing;
using System.Reflection;
using Unity.Burst.Intrinsics;
using UnityEngine.ProBuilder;

public class LockController : MonoBehaviour
{

    protected CharacterInputSystem _inputSystem;


    public CombatCameraManager combatCameraManager;
    public bool isLocked;


    [Space]

    public List<Transform> screenTargets = new List<Transform>();
    public Transform target;


    [Space]

    [Header("Canvas")]
    public Image aim;
    public Image lockAim;
    public Vector2 uiOffset;


    private bool lastLockState = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        _inputSystem = GetComponent<CharacterInputSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (screenTargets.Count < 1)
        {
            //aim.gameObject.SetActive(false);
            //aim.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            aim.color = Color.clear;
            lockAim.DOFade(0, .15f);
            target = null;
            return;
        }





        if (!isLocked)
        {
            target = screenTargets[targetIndex()];
        }


        if (target == null)
        {
            //LockInterface(false);
            isLocked = false;
            //aim.gameObject.SetActive(false);
            aim.color = Color.clear;
            //lockAim.DOFade(0, .15f);
            return;
        }

        UserInterface();


        if (lastLockState != _inputSystem.cameraLock && _inputSystem.cameraLock)
        {
            LockInterface(true);
        }
        else if (lastLockState != _inputSystem.cameraLock && !_inputSystem.cameraLock)
        {
            LockInterface(false);
        }

        if (_inputSystem.cameraLock)
        {
           // LockInterface(true);
            isLocked = true;
            combatCameraManager.Target = target;
        }
        else if (!_inputSystem.cameraLock)
        {
            //LockInterface(false);
            isLocked = false;
            combatCameraManager.Target = null;
        }

        lastLockState = _inputSystem.cameraLock;

        if (!isLocked)
            return;

    }

    private void UserInterface()
    {

        aim.transform.position = Camera.main.WorldToScreenPoint(target.position + (Vector3)uiOffset);

        /*        if (!input.canMove)
                    return;
        */
        //aim.gameObject.SetActive(true);
        Color c = screenTargets.Count < 1 ? Color.clear : Color.white;

        aim.color = c;
        //lockAim.color = c;
    }

    void LockInterface(bool state)
    {
        float size = state ? 1 : 2;
        float fade = state ? 1 : 0;
        lockAim.DOFade(fade, .15f);
        lockAim.transform.DOScale(size, .15f).SetEase(Ease.OutBack);
        lockAim.transform.DORotate(Vector3.forward * 180, .15f, RotateMode.FastBeyond360).From();
        aim.transform.DORotate(Vector3.forward * 90, .15f, RotateMode.LocalAxisAdd);
    }

    
    public int targetIndex()
    {
        float[] distances = new float[screenTargets.Count];

        for (int i = 0; i < screenTargets.Count; i++)
        {
            //distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(screenTargets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
            distances[i] = Vector3.Distance(screenTargets[i].position, this.transform.position);
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }

        return index;

    }

}
