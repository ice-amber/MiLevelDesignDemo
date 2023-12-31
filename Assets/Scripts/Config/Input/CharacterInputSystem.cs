﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputSystem : MonoBehaviour
{
    private InputController _inputController;


    //Key Setting
    public Vector2 playerMovement
    {
        get => _inputController.PlayerInput.Movement.ReadValue<Vector2>();
    }

    public Vector2 cameraLook
    {
        get => _inputController.PlayerInput.CameraLook.ReadValue<Vector2>();
    }

    public bool playerLAtk
    {
        get => _inputController.PlayerInput.LAtk.triggered;
    }
    
    public bool playerRAtk
    {
        get => _inputController.PlayerInput.RAtk.triggered;
    }

    public bool playerRun
    {
        get => _inputController.PlayerInput.Run.phase == InputActionPhase.Performed;
    }

    public bool playerRoll
    {
        get => _inputController.PlayerInput.Roll.triggered;
    }

    public bool playerCrouch
    {
        get => _inputController.PlayerInput.Crouch.triggered;
    }

    public bool cameraLock
    {
        get => _inputController.PlayerInput.CameraLock.phase == InputActionPhase.Performed;
    }

    public bool Interaction
    {
        get => _inputController.PlayerInput.Interaction.triggered;
    }
    public bool PlatformUp
    {
        get => _inputController.PlayerInput.PlatformUP.phase == InputActionPhase.Performed;
    }

    public bool PlatformDown
    {
        get => _inputController.PlayerInput.PlatformDown.phase == InputActionPhase.Performed;
    }

    public bool FlyMode
    {
        get => _inputController.PlayerInput.FlyMode.triggered;
    }



    //内部函数
    private void Awake()
    {
        if (_inputController == null)
            _inputController = new InputController();
    }

    private void OnEnable()
    {
        _inputController.Enable();
    }

    private void OnDisable()
    {
        _inputController.Disable();
    }
    


}