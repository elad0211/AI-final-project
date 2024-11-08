using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{   
  
    public enum Action
    {
        Shoot ,
        Reload,
        Block ,
        Bazuka,
        OutOfAmmo
    };
    public Action action;
    public int bullets;
    public Image [] bulletsSprite; 
    public  void ChooseAction(int actionIndex)
    {
        switch (actionIndex)
        {
            case 3:
                action = Action.Bazuka;
                bullets = 0;
                break;
            case 2:
                if (bullets > 0)
                {
                    action = Action.Shoot;
                    bullets--;
                    SetBulletVisuals();

                }
                else
                    action = Action.OutOfAmmo;
                break;
            case 1:
                action = Action.Reload;
                bullets++;
                SetBulletVisuals();

                break;
            case 0:
                action = Action.Block;
                break;
        }
    }
    public virtual void ChooseActionAI()
    {
      

       
    }

    public void SetBulletVisuals()
    {
        for (int i = 0; i < bulletsSprite.Length; i++)
        {
            if (bullets>i)
            {
                bulletsSprite[i].color = Color.white;
            }
            else
                bulletsSprite[i].color = Color.black;

        }
    }
}
