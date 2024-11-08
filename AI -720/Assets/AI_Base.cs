using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Base : Player
{
    int actionRange;
    public override void ChooseActionAI()
    {
        //check if has bullets to limit game from using impossible tasks
        if (bullets > 5)
        {
            actionRange = 4;
        }
        else if (bullets > 0)
        {
            actionRange = 3;
        }
        else
            actionRange = 2;

        int choosenAactionIndex = Random.Range(0, actionRange);

        switch (choosenAactionIndex)
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
}
