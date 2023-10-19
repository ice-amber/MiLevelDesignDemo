using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UGG.Combat
{
    public class PlayerCombatSystem : CharacterCombatSystemBase
    {
        public event Action<int, int> UpdateHealthBarOnAttacked;

        public int health = 20;
        public int MaxHealth = 20;
        //Speed
        [SerializeField, Header("攻击移动速度倍率"), Range(.1f, 10f)]
        private float attackMoveMult;

        public CombatCameraManager CombatCamera;
        
        //检测
        [SerializeField, Header("检测敌人")] private Transform detectionCenter;
        [SerializeField] private float NormalAttackDetectionRang;
        [SerializeField] private float SkillAttackDetectionRang;

        //缓存
        [SerializeField]  private Collider[] NormalAttackDetectionTarget = new Collider[10];
        [SerializeField] private Collider[] SkillAttackDetectionTarget = new Collider[10];

        public GameObject HandGreatSword;
        public GameObject BackGreatSword;
        public GameObject HandKatanaSword;


        [SerializeField] private ParticleSystemScript AttackParticle;
        [SerializeField] private Transform AttackPosition;

        //public UnityEvent<PlayerCombatSystem> OnHit;

        public CinemachineImpulseSource NormalAttackImpulseSource;
        public CinemachineImpulseSource SkillImpulseSource;
        private void Start()
        {
            //this.OnHit.AddListener((x) => OnAnimationAttackEvent());
        }
        private void Update()
        {
            PlayerAttackAction();
            DetectionTarget();
            ActionMotion();
        }

        private void LateUpdate()
        {
            OnAttackActionCameraLocked();
        }

        private void PlayerAttackAction()
        {
            if (_characterInputSystem.playerLAtk)
            {

                _animator.SetTrigger(lAtkID);
                if (_animator.CheckAnimationTag("Attack0") && _animator.GetInteger("ComboNumber") == 0)
                {
                    _animator.SetInteger("ComboNumber",1);
                }
                else if (_animator.CheckAnimationTag("Attack1") && _animator.GetInteger("ComboNumber") == 1)
                {
                    _animator.SetInteger("ComboNumber", 2);
                }
                else if (_animator.CheckAnimationTag("Attack2") && _animator.GetInteger("ComboNumber") == 2)
                {
                    _animator.SetInteger("ComboNumber", 3);
                }
                else if (_animator.CheckAnimationTag("Attack3") && _animator.GetInteger("ComboNumber") == 3)
                {
                    _animator.SetInteger("ComboNumber", 0);
                }


            }

            if (_characterInputSystem.playerRAtk)
            {
                _animator.SetTrigger("RAtk");

            }

            }

        private void OnAttackActionCameraLocked()
        {
            if (CanAttackLockOn())
            {
                if (CombatCamera.isLocking && CombatCamera.Target != null)//&& _characterInputSystem.playerMovement == Vector2.zero
                {
                    this.transform.parent.transform.parent.LookAt(new Vector3(CombatCamera.Target.transform.position.x, this.transform.root.position.y, CombatCamera.Target.transform.position.z));
                    
                }
            }

        }




        private void ActionMotion()
        {
            if (_animator.CheckAnimationTag("Attack0") || _animator.CheckAnimationTag("Attack1") || _animator.CheckAnimationTag("Attack2") || _animator.CheckAnimationTag("Attack3") || _animator.CheckAnimationTag("SkillAttack"))
            {
                if (!_animator.CheckAnimationTag("Roll") && !_animator.CheckAnimationTag("Dodge"))
                {
                    _characterMovementBase.CharacterMoveInterface(transform.forward, _animator.GetFloat(animationMoveID) * attackMoveMult, true);
                }
               
            }

            if (_animator.CheckAnimationTag("SkillAttack"))
            {
                HandGreatSword.SetActive(true);
                BackGreatSword.SetActive(false);
                HandKatanaSword.SetActive(false);
            }
            else
            {
                HandGreatSword.SetActive(false);
                BackGreatSword.SetActive(true);
                HandKatanaSword.SetActive(true);
            }
        }

        #region 动作检测
        
        /// <summary>
        /// 攻击状态是否允许自动锁定敌人
        /// </summary>
        /// <returns></returns>
        private bool CanAttackLockOn()
        {
            if (_animator.CheckAnimationTag("Attack0") || _animator.CheckAnimationTag("Attack1") || _animator.CheckAnimationTag("Attack2") || _animator.CheckAnimationTag("Attack3") || _animator.CheckAnimationTag("SkillAttack"))
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
                {
                    return true;
                }
            }
            return false;
        }

        int NormalAttacktargetCount;
        int SkillAttacktargetCount;

        private void DetectionTarget()
        {
                NormalAttacktargetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, NormalAttackDetectionRang, NormalAttackDetectionTarget, enemyLayer);

                SkillAttacktargetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, SkillAttackDetectionRang, SkillAttackDetectionTarget, enemyLayer);



            if (NormalAttacktargetCount == 0)
            {
                for (int i = 0; i < NormalAttackDetectionTarget.Length; i++)
                {
                    NormalAttackDetectionTarget[i] = null;
                }
               
            }

            if (SkillAttacktargetCount == 0)
            {
                for (int i = 0; i < SkillAttackDetectionTarget.Length; i++)
                {
                    SkillAttackDetectionTarget[i] = null;
                }

            }

        }

        public void OnBeHitted(int Damage)
        {
            if (!_animator.CheckAnimationTag("SkillAttack") && !_animator.CheckAnimationTag("Roll") && !_animator.CheckAnimationTag("Dodge") && !_animator.CheckAnimationTag("hit") && !_animator.CheckAnimationTag("Attack0"))
            {
                _animator.SetTrigger("hit");
            }

            if (!_animator.CheckAnimationTag("Roll") && !_animator.CheckAnimationTag("Dodge") && !_animator.CheckAnimationTag("hit"))
            {
                health -= Damage;
                UpdateHealthBarOnAttacked?.Invoke(health, MaxHealth);
            }

        }

        public void OnAnimationAttackEvent()
        {
            

            if (_animator.CheckAnimationTag("Attack0") || _animator.CheckAnimationTag("Attack1") || _animator.CheckAnimationTag("Attack2") || _animator.CheckAnimationTag("Attack3"))
            {
                NormalAttackImpulseSource.GenerateImpulse();
                if (NormalAttacktargetCount != 0)
                {
                    
                    for (int i = 0; i < NormalAttackDetectionTarget.Length; i++)
                    {
                        if (NormalAttackDetectionTarget[i] != null)
                        {
                            NormalAttackDetectionTarget[i].gameObject.GetComponent<EnemyScript>().OnPlayerHit(NormalAttackDetectionTarget[i].gameObject.GetComponent<EnemyScript>(),1);
                            Debug.Log("NormalhitEvent");
                        }

                    }

                }

            }
            else if (_animator.CheckAnimationTag("SkillAttack"))
            {
                SkillImpulseSource.GenerateImpulse();
                if (SkillAttacktargetCount != 0)
                {

                    for (int i = 0; i < SkillAttackDetectionTarget.Length; i++)
                    {
                        if (SkillAttackDetectionTarget[i] != null)
                        {
                            SkillAttackDetectionTarget[i].gameObject.GetComponent<EnemyScript>().OnPlayerHit(SkillAttackDetectionTarget[i].gameObject.GetComponent<EnemyScript>(),3);
                            Debug.Log("SkillhitEvent");
                        }
                    }
                }

            }

            //AttackParticle.PlayParticleAtPosition(AttackPosition.position);
        }
        
       public void DamageEvent()
        {

        }


        #endregion
    }
}

