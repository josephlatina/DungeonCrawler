using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalpelController : WeaponItemController
{
    float currentSpeed = 0f;

    public void Fire()
    {
        currentSpeed = item.GetSpeed();
        transform.parent = null;
        GetComponent<ScalpelBehaviour>().currentSpeed = currentSpeed;
        StartCoroutine(FireScalpel());
    }

    protected IEnumerator FireScalpel()
    {
        yield return new WaitForSeconds(2f);
        ReturnScalpel();
    }

    public void ReturnScalpel()
    {
        currentSpeed = 0;
        GetComponent<ScalpelBehaviour>().currentSpeed = currentSpeed;
        transform.position = transform.parent.position;
    }
}
