using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using TMPro;

public class SubMachinegun : MonoBehaviour
{
    public SteamVR_Action_Boolean shoot;
    public SteamVR_Input_Sources handType;
    public GameObject Bullet;
    public GameObject BulletCasing;

    public Transform shootPoint;
    public Transform casingDispensePoint;

    private float Bullet_Power = 20.0f;
    private float BulletCasingDispense_power = 1.5f;


    // audio effects :: for gun shot
    private AudioSource gunSound;
    public AudioClip gunSoundClip;
    public AudioClip noAmmoSoundClip;

    public ParticleSystem muzzleFlasheffect;
    // check if player picked up or not
    public bool handed = false;
    // ammo
    private int max_ammo = 240;
    private int current_ammo = 0;
    // Textmeshpro
    public TextMeshPro text_ammo;

    // Start is called before the first frame update
    void Start()
    {
        shoot.AddOnStateDownListener(Shoot, handType);
        // initialize for effects
        gunSound = GetComponent<AudioSource>();
        // initial ammo
        current_ammo = max_ammo;
    }


    private void Shoot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // Able to shoot action :: 1. handed, 2. enough ammo
        if (handed == true && current_ammo > 0)
        {
            Debug.Log("0 : gun fire start");
            // effect :: gunfire particle, sound
            muzzleFlasheffect.Play();
            gunSound.PlayOneShot(gunSoundClip);
            // dispense Bullet projectile
            GameObject obj = Instantiate(Bullet, shootPoint.position, Quaternion.LookRotation(shootPoint.right));
            obj.GetComponent<Rigidbody>().velocity = shootPoint.forward * Bullet_Power;
            // dispense BulletCasing projectile
            GameObject objCasing = Instantiate(BulletCasing, casingDispensePoint.position, Quaternion.LookRotation(casingDispensePoint.right));
            objCasing.GetComponent<Rigidbody>().velocity = casingDispensePoint.right * BulletCasingDispense_power;
            // consume one ammo
            current_ammo--;
        }
        else if (handed == true && current_ammo == 0)
        {
            // although player holds revolver, no ammo
            gunSound.PlayOneShot(noAmmoSoundClip);
        }
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

public class SubMachinegun : MonoBehaviour
{
    public SteamVR_Action_Boolean shoot;
    public SteamVR_Input_Sources handType;
    public GameObject Bullet;
    public GameObject BulletCasing;

    public Transform shootPoint;
    public Transform casingDispensePoint;

    private float Bullet_Power = 16.0f;
    private float BulletCasingDispense_power = 1.5f;


    // audio effects :: for gun shot
    private AudioSource gunSound;
    public AudioClip gunSoundClip;

    public ParticleSystem muzzleFlasheffect;



    // Start is called before the first frame update
    void Start()
    {
        shoot.AddOnStateDownListener(Shoot, handType);
        // initialize for effects
        gunSound = GetComponent<AudioSource>();
    }


    private void Shoot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // for gunshot :: raycast hitscan method
        Debug.Log("0 : gun fire start");
        // effect :: gunfire particle, sound
        muzzleFlasheffect.Play();
        gunSound.PlayOneShot(gunSoundClip);
        // dispense Bullet projectile
        GameObject obj = Instantiate(Bullet, shootPoint.position, Quaternion.LookRotation(shootPoint.right));
        obj.GetComponent<Rigidbody>().velocity = shootPoint.forward * Bullet_Power;
        // dispense BulletCasing projectile
        GameObject objCasing = Instantiate(BulletCasing, casingDispensePoint.position, Quaternion.LookRotation(casingDispensePoint.right));
        objCasing.GetComponent<Rigidbody>().velocity = casingDispensePoint.right * BulletCasingDispense_power;

    }


    // Update is called once per frame
    void Update()
    {

    }
}

 
 */