using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finishing : StateMachineBehaviour
{
    PlayerController playerController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerController == null)
        {
            playerController = animator.GetComponentInParent<PlayerController>();
        }

        playerController.EnableSword();
        playerController.enabled = false;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController.EnableGun();
        playerController.enabled = true;
    }
}