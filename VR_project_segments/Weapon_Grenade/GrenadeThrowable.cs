using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class GrenadeThrowable : Throwable
{
    private bool GrenadeState = false;

    // audio effects :: for timer beeping, explosion
    private AudioSource grenadeSound;
    public AudioClip timeBeeps;
    public AudioClip explosionNoise;

    // Start is called before the first frame update
    protected override void Awake()
    {
        GrenadeState = false; 
    }

    protected override void OnDetachedFromHand(Hand hand)
    {
        Debug.Log("gthr :: Detachment occur");
        GrenadeState = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GrenadeState == true)
        {
            StartCoroutine(activateGrenade());

        }

    }

    private IEnumerator activateGrenade()
    {
        Debug.Log("grnd :: Grenade get activated");

        yield return new WaitForSeconds(0.3f);
        Debug.Log("grnd :: Beep 1 ");
        yield return new WaitForSeconds(0.3f);
        Debug.Log("grnd :: Beep 2 ");
        yield return new WaitForSeconds(0.3f);
        Debug.Log("grnd :: Beep 3 ");
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }


}

