using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    [SerializeField] GameObject footStep;
    [SerializeField] Transform[] footStepPos;

    public void FootStepRight()
    {
        GameObject fs = Instantiate(footStep, footStepPos[0].position, transform.rotation);
        Destroy(fs, 1f);
    }

    public void FootStepLeft()
    {
        GameObject fs = Instantiate(footStep, footStepPos[1].position, transform.rotation);
        Destroy(fs, 1f);
    }
}
