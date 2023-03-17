using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
using TMPro;

public class Bombpointer : MonoBehaviour
{
    // SteamVR 
    public SteamVR_Action_Boolean shoot;
    public SteamVR_Input_Sources handType;
    // Location :: where ray starts for hit-scan
    public Transform laserPoint;
    // Effects Variables
    // audio effects :: for gun shot
    private AudioSource gunSound;
    public AudioClip gunSoundClip;
    public AudioClip noAmmoSoundClip;
    public AudioClip confirmedSoundClip;

    // LineRenderer :: visualize gunshot Ray
    private LineRenderer shotlineRenderer;

    // check if player picked up or not
    public bool handed = false;
    // ammo
    private int max_ammo = 5;
    private int current_ammo = 0;
    // Textmeshpro
    public TextMeshPro text_ammo;
    public TextMeshPro showState;


    // show visualization able or not able
    public GameObject ConfirmedObj;
    public GameObject BombBlastAction;
    public GameObject PointAble;
    public GameObject PointNotAble;
    // value of range
    private float rangeAble = 25.0f;
    // 플래그 변수 :: 쏠 수 있는 상태인지 아닌지
    private bool stateAble = false;

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
        // Able to shoot action :: 1. handed, 2. enough ammo 3.쏠수있는 상태
        if (handed == true && current_ammo > 0 && stateAble == true)
        {
            // effect :: gunfire
            // consume one ammo
            current_ammo--;
            // Raycast for hit-scan
            RaycastHit hitInfo;
            // 레이 충돌 시,
            if (Physics.Raycast(laserPoint.position, laserPoint.forward, out hitInfo, rangeAble))
            {
                // 확인 폭격 지점 설정, 3초 동안 보여줌
                GameObject CofirmedMark = Instantiate(ConfirmedObj, hitInfo.point, Quaternion.LookRotation(hitInfo.point));
                gunSound.PlayOneShot(confirmedSoundClip);
                Destroy(CofirmedMark, 3.0f);
                // 폭격지점 3초 후 폭격 코루틴
                StartCoroutine(BombBlast(hitInfo));
            }
        }
        else if (handed == true && current_ammo == 0 && stateAble == true)
        {
            // although player holds revolver, no ammo
            gunSound.PlayOneShot(noAmmoSoundClip);
        }
    }

    // 3초 후 폭발 코루틴
    private IEnumerator BombBlast(RaycastHit hitInfo)
    {
        // 3초 대기
        yield return new WaitForSeconds(3.0f);
        // 사운드와 이펙트 달려있는 오브젝트 콜하기
        GameObject BombExpAction = Instantiate(BombBlastAction, hitInfo.point, Quaternion.LookRotation(hitInfo.point));
        // 구형 캐스트로 폭발 구현
        // 폭발 레이캐스트
        RaycastHit[] hitinfos = Physics.SphereCastAll(hitInfo.point, 5, Vector3.up, 0f, LayerMask.GetMask("Zombies"));
        foreach (RaycastHit hitObj in hitinfos)
        {
            // 좀비의 수류탄 피격을 콜함과 동시에 수류탄 폭발 위치 보내주기
            if (hitObj.transform.gameObject.name == "Zombie(Clone)")
            {
                hitObj.collider.gameObject.GetComponent<Zombie>().OnBombBlast(transform.position);
            }
            else if (hitObj.transform.gameObject.name == "ZombieMage(Clone)")
            {
                hitObj.collider.gameObject.GetComponent<ZombieMage>().OnBombBlast(transform.position);
            }
            else if (hitObj.transform.gameObject.name == "BigZombie(Clone)")
            {
                hitObj.collider.gameObject.GetComponent<BigZombie>().OnBombBlast(transform.position);
            }
        }

    }


    private void LaserPointVisual()
    {
        Vector3 hitPosition = laserPoint.position + laserPoint.forward * rangeAble;
        if (handed == true)
        {
            shotlineRenderer.SetPosition(0, laserPoint.position);
            shotlineRenderer.SetPosition(1, hitPosition);
            shotlineRenderer.enabled = true;


            RaycastHit hitInfo;
            // 레이 충돌 시,
            if (Physics.Raycast(laserPoint.position, laserPoint.forward, out hitInfo, rangeAble))
            {
                // 가능한 범위 충돌이면 가능 표시
                if (hitInfo.collider.gameObject.tag.Contains("Target"))
                {
                    // 가능 지점 표시
                    GameObject MakeObj = Instantiate(PointAble, hitInfo.point, Quaternion.LookRotation(hitInfo.point));
                    Destroy(MakeObj, 0.025f);
                    // 쏠 수 있음 업데이트
                    stateAble = true;
                }
                // 불가능한 범위 충돌이면 불가능 표시
                else
                {
                    // 불가능 지점 표시
                    GameObject MakeObj2 = Instantiate(PointNotAble, hitInfo.point, Quaternion.LookRotation(hitInfo.point));
                    Destroy(MakeObj2, 0.025f);
                    // 쏠 수 없음 업데이트
                    stateAble = false;
                }
            }
            // 충돌 못할 시
            else
            {
                // 불가능 표시, 라인 숨기기
                shotlineRenderer.enabled = false;
                // 쏠 수 없음 업데이트
                stateAble = false;
            }

        }
        else
        {
            shotlineRenderer.enabled = false;
            stateAble = false;
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
        shotlineRenderer.enabled = false;
        // do not display tmp
        text_ammo.text = "";
        showState.text = "";
    }


    // Update is called once per frame
    void Update()
    {
        // ammo 관련 TMP 업데이트
        if (handed == true)
        {
            text_ammo.text = "Ammo\n" + current_ammo + " / " + max_ammo;
            LaserPointVisual();
        }

        if (current_ammo == 0)
        {
            text_ammo.color = Color.red;
        }

        // AVAILABLE 관련 TMP 업데이트
        if (stateAble == true && current_ammo != 0)
        {
            showState.text = "AVAILABLE";
            showState.color = Color.green;
        }
        else if (handed == false)
        {
            showState.text = "";
        }
        else
        {
            showState.text = "UNAVAILABLE";
            showState.color = Color.red;
        }
    }
}
