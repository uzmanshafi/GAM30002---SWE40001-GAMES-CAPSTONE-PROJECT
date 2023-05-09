using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
   [SerializeField] private float Max_HealthBar = 3.8f;
   public float currentHealth {get; private set;}

   public void Awake()
   {
      currentHealth = Max_HealthBar;
   }

   public void TakeDamage(float damage)
   {
      currentHealth = Mathf.Clamp(currentHealth - damage, 0, Max_HealthBar);
      if (currentHealth > 0)
      {
         
      } 
      else
      {
             
      }
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.E))
      {
         Debug.Log("E pressed for dmg");
         TakeDamage(1.25f);

         if (currentHealth == 0)
         {
            Debug.Log("You died");
         }
      }
   }
}
