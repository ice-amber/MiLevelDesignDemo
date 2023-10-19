using System.Collections;
using System.Collections.Generic;
using UGG.Combat;
using UnityEngine;

public class MedicPack : MonoBehaviour
{
    public PlayerCombatSystem Player;
    public int HealthGain;
    public PlayerHealthUIBar HealthBar;
    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<PlayerCombatSystem>();
        HealthBar = FindObjectOfType<PlayerHealthUIBar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player get Medic");

            
            if (Player.health + HealthGain > Player.MaxHealth)
            {
                Player.health = Player.MaxHealth;
            }
            else
            {
                Player.health += HealthGain;
            }

            HealthBar.UpdateHealthBar(Player.health, Player.MaxHealth);
        }

    }
}
