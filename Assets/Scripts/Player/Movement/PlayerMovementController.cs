using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Windows;

namespace UGG.Move
{
    public class PlayerMovementController : CharacterMovementBase
    {
        //引用
        private Transform characterCamera;
        private TP_CameraController _tpCameraController;
        
        [SerializeField,Header("相机锁定点")] private Transform standCameraLook;
        [SerializeField]private Transform crouchCameraLook;
        
        //Ref Value
        private float targetRotation;
        private float rotationVelocity;
        
        //LerpTime
        [SerializeField,Header("旋转速度")] private float rotationLerpTime;
        [SerializeField] private float moveDirctionSlerpTime;
        

        //Move Speed
        [SerializeField,Header("移动速度")] private float walkSpeed;
        [SerializeField,Header("移动速度")] private float runSpeed;
        [SerializeField,Header("移动速度")] private float crouchMoveSpeed;

        [SerializeField] private float DodgeAnimationMoveSpeedMuti;
        [SerializeField] private float RollAnimationMoveSpeedMuti;


        [SerializeField,Header("角色胶囊控制(下蹲)")] private Vector3 crouchCenter;
        [SerializeField] private Vector3 originCenter;
        [SerializeField] private Vector3 cameraLookPositionOnCrouch;
        [SerializeField] private Vector3 cameraLookPositionOrigin;
        [SerializeField] private float crouchHeight;
        [SerializeField] private float originHeight;
        [SerializeField] private bool isOnCrouch;
        [SerializeField] private Transform crouchDetectionPosition;
        [SerializeField] private Transform CameraLook;
        [SerializeField] private LayerMask crouchDetectionLayer;

        public bool IsFlyMode;

        //animationID
        private int crouchID = Animator.StringToHash("Crouch");


        #region 内部函数

        protected override void Awake()
        {
            base.Awake();

            characterCamera = Camera.main.transform.root.transform;
            //_tpCameraController = characterCamera.GetComponent<TP_CameraController>();
        }

        protected override void Start()
        {
            base.Start();

            
            cameraLookPositionOrigin = CameraLook.position;
        }
        Vector2 input;
        private Vector3 inputv3;
        protected override void Update()
        {
            base.Update();

            if (_inputSystem.FlyMode)
            {
                IsFlyMode = !IsFlyMode;
            }


            if (!IsFlyMode)
            {
                PlayerMoveDirection();
            }
            else
            {
                FlyMode();
            }

            //UpdateRollAnimation();
        }

        private void LateUpdate()
        {
            //CharacterCrouchControl();
            if (!IsFlyMode)
            {
                UpdateMotionAnimation();
                UpdateCrouchAnimation();
                UpdateRollAnimation();
            }

            
        }

        #endregion



        #region 条件

        private bool CanMoveContro()
        {
            return isOnGround && characterAnimator.CheckAnimationTag("Motion") || characterAnimator.CheckAnimationTag("CrouchMotion");
        }

        private bool CanCrouch()
        {
            if (characterAnimator.CheckAnimationTag("Crouch")) return false;
            if (characterAnimator.GetFloat(runID)>.9f) return false;
            
            return true;
        }
        
        
        private bool CanRunControl()
        {
            if (Vector3.Dot(movementDirection.normalized, transform.forward) < 0.75f) return false;
            if (!CanMoveContro()) return false;
           

            return true;
        }

        #endregion
        
        private void FlyMode()
        {
            Vector3 direction;

                if (_inputSystem.playerMovement != Vector2.zero)
                {

                    targetRotation = Mathf.Atan2(_inputSystem.playerMovement.x, _inputSystem.playerMovement.y) * Mathf.Rad2Deg + characterCamera.localEulerAngles.y;

                    //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationLerpTime);

                    direction = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;

                    direction = direction.normalized;


                    this.transform.DOMove(this.transform.position + direction, 0.4f);
                }
            if (_inputSystem.PlatformUp)
            {
                this.transform.DOMoveY((this.transform.position + Vector3.up).y, 0.2f);
            }
            if (_inputSystem.PlatformDown)
            {
                this.transform.DOMoveY((this.transform.position + Vector3.down).y, 0.2f);
            }
        }

        private void PlayerMoveDirection()
        {
            
            if (isOnGround && _inputSystem.playerMovement == Vector2.zero)
                movementDirection = Vector3.zero;
            
            if(CanMoveContro()) 
            {
                if (_inputSystem.playerMovement != Vector2.zero)
                {
            
                    targetRotation = Mathf.Atan2(_inputSystem.playerMovement.x, _inputSystem.playerMovement.y) * Mathf.Rad2Deg + characterCamera.localEulerAngles.y;
            
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationLerpTime);

                    var direction = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
            
                    direction = direction.normalized;

                    movementDirection = Vector3.Slerp(movementDirection, ResetMoveDirectionOnSlop(direction),
                        moveDirctionSlerpTime * Time.deltaTime);

                }
            }
            else 
            {
                movementDirection = Vector3.zero;
            }
            
            control.Move((characterCurrentMoveSpeed * Time.deltaTime)
                * movementDirection.normalized + Time.deltaTime
                * new Vector3(0.0f, verticalSpeed, 0.0f));


/*            if (characterAnimator.CheckAnimationTag("Roll"))
            {
                targetRotation = Mathf.Atan2(_inputSystem.playerMovement.x, _inputSystem.playerMovement.y) * Mathf.Rad2Deg + characterCamera.localEulerAngles.y;

                transform.eulerAngles = Vector3.up * targetRotation;

            }*/

        }



        private void UpdateMotionAnimation()
        {

            //input = new Vector2(inputv3.x, inputv3.z).normalized;

            characterAnimator.SetFloat(inputID, _inputSystem.playerMovement.sqrMagnitude);



            if (CanRunControl())
            {

                characterAnimator.SetFloat(movementID, _inputSystem.playerMovement.sqrMagnitude * ((_inputSystem.playerRun && !isOnCrouch) ? 2f : 1f), 0.1f, Time.deltaTime);
                characterCurrentMoveSpeed = (_inputSystem.playerRun && !isOnCrouch) ? runSpeed : walkSpeed;
            }
            else
            {
                characterAnimator.SetFloat(movementID,0f,0.05f,Time.deltaTime);
                characterCurrentMoveSpeed = 0f;
            }

            characterAnimator.SetFloat(runID, (_inputSystem.playerRun && !isOnCrouch) ? 1f : 0f);
        }

        private void UpdateCrouchAnimation()
        {
            if (isOnCrouch)
            {
                characterCurrentMoveSpeed = crouchMoveSpeed;
            }
            
        }

        Vector2 inputDodgeDirection = Vector2.zero;
        bool DodgeTag = false;
        Vector3 inputworldspaceDirection;
        private void UpdateRollAnimation()
        {

            
/*            targetRotation = Mathf.Atan2(_inputSystem.playerMovement.x, _inputSystem.playerMovement.y) * Mathf.Rad2Deg + characterCamera.localEulerAngles.y;

            transform.eulerAngles = Vector3.up * targetRotation;

            var direction = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;

            direction = direction.normalized;
*/
            if (_inputSystem.playerRoll)
            {
                
                characterAnimator.SetTrigger(rollID);

                input = _inputSystem.playerMovement.normalized;
                if (input.magnitude != 0)
                {
                    Vector2 transformDirection = new Vector2(transform.root.forward.x, transform.root.forward.z).normalized;
                    float angle1 = Vector2.SignedAngle(new Vector2(characterCamera.forward.x, characterCamera.forward.z), Vector2.up);
                    inputv3 = Quaternion.Euler(new Vector3(0, angle1, 0)) * new Vector3(input.x, 0, input.y);

                    float angle2 = Vector2.SignedAngle(transformDirection, Vector2.up);
                    inputv3 = Quaternion.Euler(new Vector3(0, -angle2, 0)) * inputv3;

                    characterAnimator.SetFloat(horizontalID, inputv3.x);
                    characterAnimator.SetFloat(verticalID, inputv3.z);
                }
                else
                {
                    characterAnimator.SetFloat(horizontalID, 0);
                    characterAnimator.SetFloat(verticalID, 0);
                }
            }

            if (characterAnimator.CheckAnimationTag("Roll"))
            {
                //this.transform.root.LookAt(new Vector3(targetRotation.x, 0, targetRotation.z));

                //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationLerpTime);
                CharacterMoveInterface(transform.forward, characterAnimator.GetFloat(animationMoveID) * RollAnimationMoveSpeedMuti, true);
            }
            if (characterAnimator.CheckAnimationTag("Dodge"))
            {
                characterAnimator.speed = 1;


                if (!DodgeTag)
                {
                    DodgeTag = true;
                    inputDodgeDirection = _inputSystem.playerMovement;
                    //this.transform.root.LookAt(new Vector3(targetRotation.x, 0, targetRotation.z));
                    float angle1 = Vector2.SignedAngle(new Vector2(characterCamera.forward.x, characterCamera.forward.z), Vector2.up);
                    inputworldspaceDirection = (Quaternion.Euler(new Vector3(0, angle1, 0)) * new Vector3(inputDodgeDirection.x, 0, inputDodgeDirection.y)).normalized;
                    if (inputDodgeDirection == Vector2.zero)
                    {
                        inputworldspaceDirection = -transform.root.forward;
                    }
                }

                //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationLerpTime);
                CharacterMoveInterface(inputworldspaceDirection, characterAnimator.GetFloat(animationMoveID) * DodgeAnimationMoveSpeedMuti, true);
            }
            else
            {
                inputDodgeDirection = Vector2.zero;
                DodgeTag = false;
                characterAnimator.speed = 1;
            }
            
        }
        
       /* private void CharacterCrouchControl()
        {
            if(!CanCrouch()) return;

            if (_inputSystem.playerCrouch)
            {
                
                if (isOnCrouch)
                {
                    if (!DetectionHeadHasObject())
                    {
                        isOnCrouch = false;
                        characterAnimator.SetFloat(crouchID,0f);
                        SetCrouchColliderHeight(originHeight,originCenter);
                        _tpCameraController.SetLookPlayerTarget(standCameraLook);
                    }
                    
                }
                else
                {
                    isOnCrouch = true;
                    characterAnimator.SetFloat(crouchID,1f);
                    SetCrouchColliderHeight(crouchHeight,crouchCenter);
                    _tpCameraController.SetLookPlayerTarget(crouchCameraLook);
                }
            }
        }*/
        
        
        private void SetCrouchColliderHeight(float height,Vector3 center)
        {
            control.center = center;
            control.height = height;
            
        }
        
        
        private bool DetectionHeadHasObject()
        {
            Collider[] hasObjects = new Collider[1];
            
            int objectCount = Physics.OverlapSphereNonAlloc(crouchDetectionPosition.position, 0.5f, hasObjects,crouchDetectionLayer);
            
            if (objectCount > 0)
            {
                return true;
            }

            return false;
        }
        
    }
    
    
}
