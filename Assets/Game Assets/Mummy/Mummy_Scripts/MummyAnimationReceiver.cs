using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyAnimationReceiver : MonoBehaviour
{
    public void ApplyDamage(float mult)
    {
        transform.parent.GetComponent<WalkerMummy>().ApplyDamage(mult);
    }
    public void StartWalking()
    {
        transform.parent.GetComponent<WalkerMummy>().StopAttack();
    }
    public void ReleaseFlame()
    {
        transform.parent.GetComponent<PriestMummy>().ReleaseFlame();
    }
    public void ChargeFlame()
    {

    }
    public void Thump()
    {
        transform.parent.GetComponent<WalkerMummy>().Thump();
    }
}
