using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIBar: MonoBehaviour
{
    public GameObject HealthUiPrefab;

    public Transform barpoint;

    public bool alwaysVisble;

    public float visableTime = 3;
    private float timeLeft;

    Image healthSlider;

    Transform UIbar;

    Transform cam;

    EnemyScript thisEnemy;

    private void Awake()
    {
        thisEnemy = GetComponent<EnemyScript>();

        thisEnemy.UpdateHealthBarOnAttacked += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(HealthUiPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisble);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int MaxHealth)
    {
        if (currentHealth<=0)
        {
            if (UIbar != null)
            {
                Destroy(UIbar.gameObject);
            }

        }
        if (UIbar != null)
        {
            UIbar.gameObject.SetActive(true);
        }

        timeLeft = visableTime;

        float sliderPercent = (float)currentHealth / MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barpoint.position;
            UIbar.forward = -cam.forward;

            if (timeLeft <= 0 && !alwaysVisble)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
