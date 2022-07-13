using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Animator animator;
    Collider finishingTrigger;

    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        finishingTrigger = GetComponent<Collider>();
        DisableRagdoll();
    }

    public void Die()
    {
        finishingTrigger.enabled = false;
        EnableRagdoll();
        StartCoroutine(Respawn());
    }

    void EnableRagdoll()
    {
        animator.enabled = false;
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
    }

    void DisableRagdoll()
    {
        animator.enabled = true;
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5);

        transform.position = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
        DisableRagdoll();
        finishingTrigger.enabled = true;
    }
}
