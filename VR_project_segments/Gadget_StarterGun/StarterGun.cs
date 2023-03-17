using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using TMPro;

public class StarterGun : MonoBehaviour
{
    // SteamVR 
    public SteamVR_Action_Boolean shoot;
    public SteamVR_Input_Sources handType;

    // Effects Variables
    // audio effects :: for gun shot
    private AudioSource gunSound;
    public AudioClip gunSoundClip;
    public AudioClip noAmmoSoundClip;
    // Particle :: gunflare 
    public ParticleSystem gunFireEffect;
    public ParticleSystem FlareEffect;
    // check if player picked up or not
    public bool handed = false;
    // ammo
    private int max_ammo = 1;
    private int current_ammo = 0;
    // Textmeshpro
    public TextMeshPro GuideMsg;
    public TextMeshPro BigTextMsg;

    // Start is called before the first frame update
    void Start()
    {
        // SteamVR input listener
        shoot.AddOnStateDownListener(Shoot, handType);
        // effect :: initialize
        gunSound = GetComponent<AudioSource>();
        current_ammo = max_ammo;

        //
    }

    private void Shoot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // Able to shoot action :: 1. handed, 2. enough ammo
        if (handed == true && current_ammo > 0)
        {
            // effect :: gunsound, gunflare
            gunFireEffect.Play();
            gunSound.PlayOneShot(gunSoundClip);
            // start coroutine
            StartCoroutine(FlareCoroutine());
            // consume one ammo
            current_ammo--;
            // light change
            StartCoroutine(LightDarkerCoroutine());
            // 웨이브가 이제 시작하므로 System Manager의 웨이브 스타트를 콜하기
            SystemManager sysman = GameObject.Find("Player").GetComponent<SystemManager>();
            sysman.WaveStart();
            //sysman.GetComponent<SystemManager>().WaveStart();

        }
        else if (handed == true && current_ammo == 0)
        {
            // although player holds revolver, no ammo
            gunSound.PlayOneShot(noAmmoSoundClip);
        }

    }
    private IEnumerator LightDarkerCoroutine()
    {
        // 조명 꺼주기
        IngameLightSetting lightc = GameObject.Find("Directional Light").GetComponent<IngameLightSetting>();
        yield return new WaitForSeconds(1.25f);
        lightc.mainlight.intensity = 0.8f;
        yield return new WaitForSeconds(1.25f);
        lightc.mainlight.intensity = 0.5f;
        yield return new WaitForSeconds(1.25f);
        lightc.mainlight.intensity = 0.25f;
        //yield return new WaitForSeconds(0.75f);
        //lightc.mainlight.intensity = 0.125f;
        //yield return new WaitForSeconds(0.75f);
        //lightc.mainlight.intensity = 0.1f;
        //yield return new WaitForSeconds(0.75f);
        //lightc.mainlight.intensity = 0.075f;
    }

    private IEnumerator FlareCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        FlareEffect.Play();
        BigTextMsg.text = "WAVE\nSTART";
        BigTextMsg.color = Color.cyan;
        yield return new WaitForSeconds(1.5f);
        BigTextMsg.text = "";
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
        GuideMsg.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // if player holding gun and has ammo, print guide message
        if (handed == true && current_ammo != 0)
        {
            GuideMsg.text = "Pull the trigger\nto Start the Wave";
        }
        if (current_ammo == 0)
        {
            GuideMsg.text = "";
        }
    }
}
