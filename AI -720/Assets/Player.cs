using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNamespace;
namespace GameNamespace
{
    public enum Action
    {
        s,//shoot
        r,//reload
        b,//block
        z,//bazuka
        OutOfAmmo
    }
}
public class Player : MonoBehaviour
{   
  

    public Action action;
    public int bullets;
    public Image [] bulletsSprite;
    public bool hasCannon;
    public int turn = 0;

    public void ChooseAction(int actionIndex)
    {
        switch (actionIndex)
        {
            case 3:
                if (bullets >= 6)
                {
                    action = Action.z;
                    bullets = 0;
                }
                else
                    action = Action.OutOfAmmo;
                break;
            case 2:
                if (bullets > 0)
                {
                    action = Action.s;
                    bullets--;
                    SetBulletVisuals();

                }
                else
                    action = Action.OutOfAmmo;
                break;
            case 1:
                action = Action.r;
                bullets++;
                if(bullets>=6)
                { hasCannon = true; }
                SetBulletVisuals();
                break;
            case 0:
                action = Action.b;
                break;
        }
    }
    public virtual GameNamespace.Action ChooseActionAI()
    {


        return GameNamespace.Action.b;
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
