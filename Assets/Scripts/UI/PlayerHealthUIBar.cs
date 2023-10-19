using System;
using System.Collections;
using System.Collections.Generic;
using UGG;
using UGG.Combat;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUIBar : MonoBehaviour
{
    //public GameObject HealthUiPrefab;

    //public Canvas canvas;
    //public Transform barpoint;

    //public bool alwaysVisble;

    //public float visableTime = 3;
    //private float timeLeft;

    Image healthSlider;

    public Transform UIbar;

    //Transform cam;

    PlayerCombatSystem player;

    private void Awake()
    {
        player = GetComponent<PlayerCombatSystem>();

        player.UpdateHealthBarOnAttacked += UpdateHealthBar;
    }

    private void OnEnable()
    {
        //cam = Camera.main.transform;

        //UIbar = Instantiate(HealthUiPrefab, canvas.transform).transform;
        healthSlider = UIbar.GetChild(0).GetComponent<Image>();
    }

    public void UpdateHealthBar(int currentHealth, int MaxHealth)
    {
        if (currentHealth <= 0)
        {
            if (UIbar != null)
            {
                Destroy(UIbar.gameObject);
            }

        }

        //UIbar.gameObject.SetActive(true);
        //timeLeft = visableTime;

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
    }
}
