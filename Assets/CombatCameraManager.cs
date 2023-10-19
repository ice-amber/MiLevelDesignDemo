using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCameraManager : MonoBehaviour
{
    public CharacterInputSystem controller;
    public CinemachineVirtualCamera CombatCamera;
    public Transform Target;
    public Transform PlayerFollow;
    public Transform Playerlookat;
    public bool isLocking = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (controller.cameraLock && Target != null)
        {
            isLocking = true;
            //CombatCamera.Follow = Target.transform;
            //CombatCamera.LookAt = Target.transform;
            CombatCamera.gameObject.SetActive(true);
            CombatCamera.GetComponent<LockCameraLogic>().enemy = Target;
            CombatCamera.GetComponent<CinemachineVirtualCamera>().LookAt = CombatCamera.GetComponent<LockCameraLogic>().LookatTarget;

            /*            CombatCamera.m_Orbits[0].m_Height = 4.5f;
                        CombatCamera.m_Orbits[0].m_Radius = 5f;
                        CombatCamera.m_Orbits[1].m_Height = 2.5f;
                        CombatCamera.m_Orbits[1].m_Radius = 8f;
                        CombatCamera.m_Orbits[1].m_Height = 0.4f;
                        CombatCamera.m_Orbits[1].m_Radius = 5f;*/




        }
        else
        {
            /*            CombatCamera.Follow = PlayerFollow.transform;
                        CombatCamera.LookAt = Playerlookat.transform;*/
            
            CombatCamera.gameObject.SetActive(false);
            isLocking = false;


            /*            CombatCamera.m_Orbits[0].m_Height = 5.5f;
                        CombatCamera.m_Orbits[0].m_Radius = 2f;
                        CombatCamera.m_Orbits[1].m_Height = 2.5f;
                        CombatCamera.m_Orbits[1].m_Radius = 5f;
                        CombatCamera.m_Orbits[1].m_Height = 0.4f;
                        CombatCamera.m_Orbits[1].m_Radius = 3f;*/
        }
    }
}
