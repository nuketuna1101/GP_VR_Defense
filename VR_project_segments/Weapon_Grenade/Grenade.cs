using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
using TMPro;

public class Grenade : MonoBehaviour
{

    public SteamVR_Action_Boolean pressA;
    public SteamVR_Input_Sources handType;

    // grenade state flag
    private bool GrenadeState = false;

    // audio effects :: for timer beeping, explosion
    private AudioSource grenadeSound;
    public AudioClip timeBeeps;
    public AudioClip explosionNoise;
    public AudioClip ThrowSoundClip;
    public AudioClip ImpactSoundClip;
    public AudioClip ActivateSoundClip;

    // particles
    public ParticleSystem explosionEffect;
    public ParticleSystem blinkEffect;

    // check if player picked up or not
    public bool handed = false;

    // TMP
    public TextMeshPro stateTMP;
    // Pop out splint location
    public Transform popoutPoint;
    public GameObject splintObj;



    // Start is called before the first frame update
    void Start()
    {
        pressA.AddOnStateDownListener(Activate, handType);

        grenadeSound = GetComponent<AudioSource>();
        GrenadeState = false;
        handed = false;
    }

    private void Activate(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (handed == true && GrenadeState == false)
        {
            // activate Grenade
            // soundeffect
            grenadeSound.PlayOneShot(ActivateSoundClip);
            GrenadeState = true;
            // splint remove
            GameObject RemoveSplint = GameObject.Find("splint");
            Destroy(RemoveSplint);
            // dispense splint
            GameObject SplintOut = Instantiate(splintObj, popoutPoint.position, Quaternion.LookRotation(popoutPoint.right));
            SplintOut.GetComponent<Rigidbody>().velocity = popoutPoint.up * 1.5f;
        }
    }
    private void OnAttachedToHand(Hand hand)
    {
        // if player holds revolver
        handed = true;
    }

    private void OnDetachedFromHand(Hand hand)
    {
        Debug.Log("gthr :: Detachment occur");
        handed = false;
        //GrenadeState = true;
        grenadeSound.PlayOneShot(ThrowSoundClip);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (handed == false && GrenadeState == true)
        {
            grenadeSound.PlayOneShot(ImpactSoundClip);
            StartCoroutine(activateGrenade());
        }

    }

    private IEnumerator activateGrenade()
    {
        Debug.Log("grnd :: Grenade get activated");
        blinkEffect.Play();

        yield return new WaitForSeconds(1.0f);
        blinkEffect.Play();
        grenadeSound.PlayOneShot(timeBeeps);

        yield return new WaitForSeconds(1.0f);
        blinkEffect.Play();
        // 수류탄 오브젝트 속도 0으로 죽이기
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        yield return new WaitForSeconds(1.0f);
        grenadeSound.PlayOneShot(explosionNoise);
        explosionEffect.Play();
        // 폭발 레이캐스트
        RaycastHit[] hitinfos = Physics.SphereCastAll(transform.position, 5, Vector3.up, 0f, LayerMask.GetMask("Zombies"));
        foreach (RaycastHit hitObj in hitinfos)
        {
            // 좀비의 수류탄 피격을 콜함과 동시에 수류탄 폭발 위치 보내주기
            if (hitObj.transform.gameObject.name == "Zombie(Clone)")
            {
                hitObj.collider.gameObject.GetComponent<Zombie>().OnGrenade(transform.position);
            }
            else if (hitObj.transform.gameObject.name == "ZombieMage(Clone)")
            {
                hitObj.collider.gameObject.GetComponent<ZombieMage>().OnGrenade(transform.position);
            }
            else if (hitObj.transform.gameObject.name == "BigZombie(Clone)")
            {
                hitObj.collider.gameObject.GetComponent<BigZombie>().OnGrenade(transform.position);
            }
        }
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        ///
    }

    void Update()
    {
        // remain ammo updates
        if (handed == false)
        {
            stateTMP.text = "";
        }
        else if (GrenadeState == false)
        {
            stateTMP.text = "IDLE";
            stateTMP.color = Color.white;
        }
        else if (GrenadeState == true)
        {
            stateTMP.text = "ACTIVATE";
            stateTMP.color = Color.yellow;
        }
    }
}