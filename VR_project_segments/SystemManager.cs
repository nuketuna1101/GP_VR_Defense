using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
using TMPro;

public class SystemManager : MonoBehaviour
{
    // 게임에서 시작화면 초기화, 웨이브 클리어 실패 등 전체 관할 스크립트

    // 파라미터 : 흐른 날짜(스테이지), 총 좀비킬수, 플레이어 체력
    private int Days = 1;
    private int ZombieKill = 0;
    private int playerHP;
    private int playerHPmax = 20;

    // stateNum은 씬에서 어떤 상태인지
    // 1 : 초기 상태
    // 2 : 웨이브 스타트를 누른 인게임
    // 3 : 웨이브 클리어
    private int stateNum = 0;

    // 좀비 생성 플래그
    private bool FlagSpawner = false;
    // 인게임 플래그
    private bool FlagisIngame = false;
    // 플레이어 생존확인 플래그 :: 켜져 있으면 확인하고 꺼져있으면 확인x
    private bool FlagcheckingAlive = false;
    // 웨이브성공 플래그 :: 켜져 있으면 확인하고 꺼져있으면 확인x
    private bool FlagWaveSuccess = false;
    // 생성할 좀비 수
    private int numZombies = 0;
    // 생성할 메이지좀비 수
    private int nummageZombies = 0;
    // 생성할 좀비 수
    private int numBigZombies = 0;
    // 통산 죽인 좀비 수
    private int numKill = 0;
    // 필드 내 생성 좀비 제한수
    private int zombieNumConstraint = 10;
    // 전체 통산 도전횟수
    private int numTrial = 1;

    // 조명
    public GameObject dirlight;


    // 가젯 무기 테이블 오브젝트
    public GameObject Table;

    // 좀비 오브젝트
    public GameObject Zombie;
    public GameObject MageZombie;
    public GameObject BossZombie;

    // 삭제할 오브젝트
    public GameObject[] TobeRemovedsets;
    public GameObject TobeRemoved;


    // 좀비 스폰 장소
    public Transform spawnpoint1;
    public Transform spawnpoint2;
    public Transform spawnpoint3;

    public Transform[] spawnpointsetisthis;

    // 플레이어 ui tmp
    public TextMeshPro playUITMP;
    public TextMeshPro playTMP_infos;
    public TextMeshPro playRecordTMP;

    //
    private AudioSource playerSound;
    public AudioClip dohClip;
    public AudioClip deathScream;

    public Hand hand;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        playerSound = GetComponent<AudioSource>();
    }

    private void Init()
    {
        // 시스템 초기상태 설정
        // 첫째날, 0킬, 풀체력, 스타터 건 새로 주기
        Debug.Log("sysman :: init");

        Days = 1;
        ZombieKill = 0;
        playerHP = playerHPmax;
        FlagSpawner = false;
        FlagisIngame = false;
        FlagcheckingAlive = false;
        FlagWaveSuccess = false;
        numZombies = 0;
        nummageZombies = 0;
        numBigZombies = 0;
        // 조명도 원래대로
        dirlight.GetComponent<IngameLightSetting>().mainlight.intensity = 1.0f;
        playUITMP.text = "";
        // 가젯 무기 전부 초기화
        RemoveWeaponsandStartGun();
        Table.GetComponent<GadgetSettings>().InitSettingwithStartGun();
    }

    private void isKilled()
    {
        // 플레이어 체력을 보고 사망으로부터 발생
        if (playerHP <= 0 && FlagcheckingAlive == true)
        {
            // 생존확인 플래그 끄기
            FlagcheckingAlive = false;
            // 대기시간 때문에 코루틴 사용
            StartCoroutine(iskilledScenario());
            Debug.Log("sysman :: is killed");
        }
    }

    private IEnumerator iskilledScenario()
    {
        // ui 텍스트 출력
        playUITMP.text = "GAME OVER\nYOU DIE";
        playerSound.PlayOneShot(deathScream);
        // 5초 가량 대기 
        // 필드 내 좀비 치우고, 좀비 생성 끄기 
        // 무기도 다 치우기
        // 1초 후 초기 시작으로
        yield return new WaitForSeconds(5.0f);
        RemoveZombies();
        RemoveWeaponsandStartGun();
        Init();
        numTrial++;
    }

    public void OnPlayerHit()
    {
        // 좀비 스크립트들로부터 피격 시 콜 된다
        playerHP -= 1;
        Debug.Log("sysman :: onplayer hit call :: HP : " + playerHP);
        playerSound.PlayOneShot(dohClip);
    }

    private void RemoveZombies()
    {
        // 필드 위 좀비 다 제거
        TobeRemovedsets = GameObject.FindGameObjectsWithTag("Zombies");

        foreach (GameObject toberemoved in TobeRemovedsets)
        {
            // 좀비의 수류탄 피격을 콜함과 동시에 수류탄 폭발 위치 보내주기
            Destroy(toberemoved);
        }
    }

    private void RemoveWeaponsandStartGun()
    {
        // 필드 위 모든 무기와 스타트 건 제거
        
        TobeRemoved = GameObject.Find("StarterFlareGun(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("Revolver_renew(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("SMG_renew(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("BombPointer_renew(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("Grenade(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("Grenade(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("Grenade(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("PoppingSplint(Clone)");
        Destroy(TobeRemoved);
        TobeRemoved = GameObject.Find("SMG_bulletcasing(Clone)");
        Destroy(TobeRemoved);
        TobeRemovedsets = GameObject.FindGameObjectsWithTag("Weapons");
        foreach (GameObject toberemoved in TobeRemovedsets)
        {
            Destroy(toberemoved);
        }
        TobeRemovedsets = GameObject.FindGameObjectsWithTag("ThrowableWeapon");
        foreach (GameObject toberemoved in TobeRemovedsets)
        {
            Destroy(toberemoved);
        }
    }


    public void WaveStart()
    {
        // 웨이브 스타터 건으로부터 발생
        StartCoroutine(WavestartAction());
        /*
        // 좀비 웨이브 생성 루틴 시작
        // 생성 플래그 올리기
        FlagSpawner = true;
        FlagisIngame = true;
        FlagcheckingAlive = true;
        FlagWaveSuccess = false;
        // 총 생성할 좀비 수 업데이트
        // numZombies = 업데이트;
        numZombies = 15;
        // 스타트건을 제외한 무기 새것으로 보급
        Debug.Log("sysman :: wave start");
        */
    }

    private IEnumerator WavestartAction()
    {
        yield return new WaitForSeconds(5.0f);
        // 좀비 웨이브 생성 루틴 시작
        // 생성 플래그 올리기
        FlagSpawner = true;
        FlagisIngame = true;
        FlagcheckingAlive = true;
        FlagWaveSuccess = false;
        // 총 생성할 좀비 수 업데이트
        // numZombies = 업데이트;
        numZombies = 10 + 2 * Days;
        if (Days % 2 == 0)
            nummageZombies = 1;
        else
            nummageZombies = 0;
        if (Days % 3 == 0)
            numBigZombies = 1;
        else
            numBigZombies = 0;


        // 스타트건을 제외한 무기 새것으로 보급
        Debug.Log("sysman :: wave start");
    }


    private void WaveEnd()
    {
        // 좀비 웨이브 컨트롤러로부터 발생 :: 성공적으로 웨이브를 끝냈음
        // 발생조건? 웨이브 스타트 이후, 모든 좀비 생성끝마쳤고, 필드에 좀비 0마리이고 플레이어 hp >0이면
        
        if (FlagWaveSuccess == true)
        {
            // 웨이브 성공 플래그 꺼주기
            FlagWaveSuccess = false;
            StartCoroutine(goNextDay());
            Debug.Log("sysman :: Wave END");
        }

    }

    private IEnumerator goNextDay()
    {
        // 웨이브 클리어 성공 시 나오는 코루틴


        // playerUI에 스테이지 클리어 메시지 출력 :: 통산 킬수, 날짜 출력
        // 10초 대기
        // days 카운터 + 1 해주기
        // 플레이어 체력 풀체력으로 다시
        // 다른 무기 + 스타터건 포함해서 전부 치우기
        // 웨이브 스타터 건 다시 생성
        playUITMP.text = "STAGE CLEAR";
        yield return new WaitForSeconds(5.0f);
        Days++;
        playerHP = playerHPmax;

        // 온갖 플래그들 원상 복귀
        //Days = 1;
        //ZombieKill = 0;
        playerHP = playerHPmax;
        FlagSpawner = false;
        FlagisIngame = false;
        FlagcheckingAlive = false;
        FlagWaveSuccess = false;
        numZombies = 0;
        nummageZombies = 0;
        numBigZombies = 0;
        // 조명도 원래대로
        dirlight.GetComponent<IngameLightSetting>().mainlight.intensity = 1.0f;
        playUITMP.text = "";
        // 가젯 무기 전부 초기화
        RemoveWeaponsandStartGun();
        Table.GetComponent<GadgetSettings>().InitSettingwithStartGun();
    }


    private int existingZombieNum()
    {
        // 태그를 통해서 필드에 존재하는 좀비 수 카운트
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Zombies");
        int zombieCounts = 0;
        for (int i = 0; i < objs.Length; i++)
        {
            zombieCounts++;
        }
        //Debug.Log("ZombieCount : " + zombieCounts);
        // 카운트 된 좀비 수를 리턴
        return zombieCounts;
    }


    public void addZombieKill()
    {
        // 좀비 죽을 때마다 콜해주기
        numKill++;
    }


    // Update is called once per frame
    void Update()
    {
        // spawn flag가 켜져있고, 특정수 미만으로 좀비 필드에 있고, 아직 더 생성해야하면
        if (FlagSpawner == true && existingZombieNum() < zombieNumConstraint && numZombies > 0)
        {
            // 개수 줄여주기
            // 랜덤한 위치에 하나 생성
            int randSpawnPos = Random.Range(0, 18);
            numZombies--;
            Instantiate(Zombie, spawnpointsetisthis[randSpawnPos].position, spawnpointsetisthis[randSpawnPos].rotation);
        }

        if (FlagSpawner == true && existingZombieNum() < zombieNumConstraint && nummageZombies > 0)
        {
            // 개수 줄여주기
            // 랜덤한 위치에 하나 생성
            int randSpawnPos = Random.Range(0, 2);
            nummageZombies--;
            Instantiate(MageZombie, spawnpointsetisthis[randSpawnPos].position, spawnpointsetisthis[randSpawnPos].rotation);
        }

        if (FlagSpawner == true && existingZombieNum() < zombieNumConstraint && numBigZombies > 0)
        {
            // 개수 줄여주기
            // 랜덤한 위치에 하나 생성
            int randSpawnPos = Random.Range(0, 18);
            numBigZombies--;
            Instantiate(BossZombie, spawnpointsetisthis[randSpawnPos].position, spawnpointsetisthis[randSpawnPos].rotation);
        }


        // 인게임 플래그가 켜진 상태에서, 좀비가 필드에 0마리, 앞으로 생성할 것도 0마리
        if (FlagisIngame == true && existingZombieNum() == 0 && numZombies <= 0 && nummageZombies <= 0 && numBigZombies <= 0 && playerHP > 0)
        {
            // 인게임 플래그는 꺼주기
            FlagisIngame = false;
            FlagSpawner = false;
            FlagcheckingAlive = false;
            // 웨이브 성공 플래그 켜주기
            FlagWaveSuccess = true;
            // 성공적으로 끝냈다고 볼 수 있다.
            WaveEnd();
        }

        // 플레이어의 사망상태 확인
        isKilled();

        if (playerHP <= 0)
            playTMP_infos.text = "Day " + Days + "\nHP : 0";
        else
            playTMP_infos.text = "Day " + Days + "\nHP : " + playerHP;

        playRecordTMP.text = "Trial : "+ numTrial + "\nTotal Kill : " + numKill;

    }
}