using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCameraLogic : MonoBehaviour
{
    public Transform enemy;
    public Transform player;
    public Transform LookatTarget;
    //public float cameraSlack;
    public float cameraDistance;

    private Vector3 pivotPoint;

    void Start()
    {
        pivotPoint = transform.position;
    }

    void Update()
    {
        LookatTarget.position = (enemy.position + 1.5f * Vector3.up + player.position) / 2 + Vector3.up;
        if (enemy == null)
        {
            return;
        }
        //Vector3 current = pivotPoint;
        Vector3 target = player.transform.position + 2f * Vector3.up;
        //pivotPoint = Vector3.MoveTowards(current, target, Vector3.Distance(current, target) * cameraSlack);
        transform.DOMove(target - transform.forward * cameraDistance, 0.3f);
        //transform.DOLookAt((, 0.2f);

        
        //transform.position = pivotPoint;
        //transform.LookAt((enemy.position + 1.5f * Vector3.up + player.position) / 2 + Vector3.up);
        //transform.position -= transform.forward * cameraDistance;
    }
}
