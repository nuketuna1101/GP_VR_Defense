using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using TMPro;

public class Revolver : MonoBehaviour
{
    // SteamVR 
    public SteamVR_Action_Boolean shoot;
    public SteamVR_Input_Sources handType;
    // Location :: where ray starts for hit-scan
    public Transform shootPoint;

    // Effects Variables
    // audio effects :: for gun shot
    private AudioSource gunSound;
    public AudioClip gunSoundClip;
    public AudioClip noAmmoSoundClip;
    // LineRenderer :: visualize gunshot Ray
    private LineRenderer shotlineRenderer;
    // Particle :: gunfire muzzle
    public ParticleSystem muzzleFlasheffect;
    // check if player picked up or not
    public bool handed = false;
    // ammo
    private int max_ammo = 40;
    private int current_ammo = 0;
    // Textmeshpro
    public TextMeshPro text_ammo;
    // 피격으로 전달할 인자
    private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        // SteamVR input listener
        shoot.AddOnStateDownListener(Shoot, handType);
        // effect :: initialize
        gunSound = GetComponent<AudioSource>();
        shotlineRenderer = GetComponent<LineRenderer>();
        shotlineRenderer.positionCount = 2;
        shotlineRenderer.enabled = false;
        // initial ammo
        current_ammo = max_ammo;
    }

    private void Shoot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // Able to shoot action :: 1. handed, 2. enough ammo
        if (handed == true && current_ammo > 0)
        {
            Debug.Log("0 : gun fire start");
            Debug.DrawRay(shootPoint.position, shootPoint.forward * 30, Color.red);
            // effect :: gunfire
            StartCoroutine(GunfireEffect(shootPoint.position + shootPoint.forward * 30));
            // consume one ammo
            current_ammo --;
            // Raycast for hit-scan
            RaycastHit hitInfo;
            if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hitInfo, 30.0f))
            {
                Debug.Log("1 : Name is  " + hitInfo.transform.name);
                // 레이 캐스트로 좀비스 태그 충돌시
                //if (hitInfo.collider.gameObject.tag.Contains("Zombies"))
                if (hitInfo.collider.gameObject.name == "Zombie(Clone)")
                {
                    Debug.Log("2 : Tag is  " + hitInfo.collider.tag);
                    // 리볼버 피격 인자 전달해주기.
                    hitInfo.collider.gameObject.GetComponent<Zombie>().OnRevolver();
                }
                else if (hitInfo.collider.gameObject.name == "ZombieMage(Clone)")
                {
                    Debug.Log("2 : Tag is  " + hitInfo.collider.tag);
                    // 리볼버 피격 인자 전달해주기.
                    hitInfo.collider.gameObject.GetComponent<ZombieMage>().OnRevolver();
                }
                else if (hitInfo.collider.gameObject.name == "BigZombie(Clone)")
                {
                    Debug.Log("2 : Tag is  " + hitInfo.collider.tag);
                    // 리볼버 피격 인자 전달해주기.
                    hitInfo.collider.gameObject.GetComponent<BigZombie>().OnRevolver();
                }
            }
        }
        else if (handed == true && current_ammo == 0)
        {
            // although player holds revolver, no ammo
            gunSound.PlayOneShot(noAmmoSoundClip);
        }

    }

    private IEnumerator GunfireEffect(Vector3 hitPosition)
    {
        // Coroution for gunshot Effects
        // particle and audio effect
        muzzleFlasheffect.Play();
        gunSound.PlayOneShot(gunSoundClip);
        // gunshot trail effect :: visualize ray
        shotlineRenderer.SetPosition(0, shootPoint.position);
        shotlineRenderer.SetPosition(1, hitPosition);
        shotlineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);
        shotlineRenderer.enabled = false;
    }

    private void OnAttachedToHand(Hand hand)
    {
        // if player holds revolver
        handed = true;
    }
    
    private void OnDetachedFromHand(Hand hand)
    {
        // if player dont holds revolver
        handed = false;
        // do not display tmp
        text_ammo.text = "";
    }


    // Update is called once per frame
    void Update()
    {
        // remain ammo updates
        if (handed == true)
        {
            text_ammo.text = "Ammo\n" + current_ammo + " / " + max_ammo;
        }
        if (current_ammo == 0)
        {
            text_ammo.color = Color.red;
        }
    }
}


/*
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Revolver : MonoBehaviour
{
    // SteamVR 
    public SteamVR_Action_Boolean shoot;
    public SteamVR_Input_Sources handType;

    // Location :: where ray starts for hit-scan
    public Transform shootPoint;

    // Effects Variables
    // audio effects :: for gun shot
    private AudioSource gunSound;
    public AudioClip gunSoundClip;
    // LineRenderer :: visualize gunshot Ray
    private LineRenderer shotlineRenderer;
    // Particle :: gunfire muzzle
    public ParticleSystem muzzleFlasheffect;


    // Start is called before the first frame update
    void Start()
    {
        // SteamVR input listener
        shoot.AddOnStateDownListener(Shoot, handType);
        // effect :: initialize
        gunSound = GetComponent<AudioSource>();
        shotlineRenderer = GetComponent<LineRenderer>();
        shotlineRenderer.positionCount = 2;
        shotlineRenderer.enabled = false;
    }

    private void Shoot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // Shoot :: when player pull the trigger action >> gunshot event
        Debug.Log("0 : gun fire start");
        Debug.DrawRay(shootPoint.position, shootPoint.forward * 30, Color.red);
        // effect :: gunfire
        StartCoroutine(GunfireEffect(shootPoint.position + shootPoint.forward * 30));
        // Raycast for hit-scan
        RaycastHit hitInfo;
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hitInfo, 30.0f))
        {
            Debug.Log("1 : Name is  "+ hitInfo.transform.name);    
            if (hitInfo.collider.gameObject.tag.Contains("Target"))
            {
                Debug.Log("2 : Tag is  "+ hitInfo.collider.tag);
                Destroy(hitInfo.collider.gameObject);
            }
        }
    }

    private IEnumerator GunfireEffect(Vector3 hitPosition)
    {
        // Coroution for gunshot Effects
        // particle and audio effect
        muzzleFlasheffect.Play();
        gunSound.PlayOneShot(gunSoundClip);
        // gunshot trail effect :: visualize ray
        shotlineRenderer.SetPosition(0, shootPoint.position);
        shotlineRenderer.SetPosition(1, hitPosition);
        shotlineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);
        shotlineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}


*/