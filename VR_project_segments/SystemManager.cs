using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
using TMPro;

public class SystemManager : MonoBehaviour
{
    // ���ӿ��� ����ȭ�� �ʱ�ȭ, ���̺� Ŭ���� ���� �� ��ü ���� ��ũ��Ʈ

    // �Ķ���� : �帥 ��¥(��������), �� ����ų��, �÷��̾� ü��
    private int Days = 1;
    private int ZombieKill = 0;
    private int playerHP;
    private int playerHPmax = 20;

    // stateNum�� ������ � ��������
    // 1 : �ʱ� ����
    // 2 : ���̺� ��ŸƮ�� ���� �ΰ���
    // 3 : ���̺� Ŭ����
    private int stateNum = 0;

    // ���� ���� �÷���
    private bool FlagSpawner = false;
    // �ΰ��� �÷���
    private bool FlagisIngame = false;
    // �÷��̾� ����Ȯ�� �÷��� :: ���� ������ Ȯ���ϰ� ���������� Ȯ��x
    private bool FlagcheckingAlive = false;
    // ���̺꼺�� �÷��� :: ���� ������ Ȯ���ϰ� ���������� Ȯ��x
    private bool FlagWaveSuccess = false;
    // ������ ���� ��
    private int numZombies = 0;
    // ������ ���������� ��
    private int nummageZombies = 0;
    // ������ ���� ��
    private int numBigZombies = 0;
    // ��� ���� ���� ��
    private int numKill = 0;
    // �ʵ� �� ���� ���� ���Ѽ�
    private int zombieNumConstraint = 10;
    // ��ü ��� ����Ƚ��
    private int numTrial = 1;

    // ����
    public GameObject dirlight;


    // ���� ���� ���̺� ������Ʈ
    public GameObject Table;

    // ���� ������Ʈ
    public GameObject Zombie;
    public GameObject MageZombie;
    public GameObject BossZombie;

    // ������ ������Ʈ
    public GameObject[] TobeRemovedsets;
    public GameObject TobeRemoved;


    // ���� ���� ���
    public Transform spawnpoint1;
    public Transform spawnpoint2;
    public Transform spawnpoint3;

    public Transform[] spawnpointsetisthis;

    // �÷��̾� ui tmp
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
        // �ý��� �ʱ���� ����
        // ù°��, 0ų, Ǯü��, ��Ÿ�� �� ���� �ֱ�
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
        // ���� �������
        dirlight.GetComponent<IngameLightSetting>().mainlight.intensity = 1.0f;
        playUITMP.text = "";
        // ���� ���� ���� �ʱ�ȭ
        RemoveWeaponsandStartGun();
        Table.GetComponent<GadgetSettings>().InitSettingwithStartGun();
    }

    private void isKilled()
    {
        // �÷��̾� ü���� ���� ������κ��� �߻�
        if (playerHP <= 0 && FlagcheckingAlive == true)
        {
            // ����Ȯ�� �÷��� ����
            FlagcheckingAlive = false;
            // ���ð� ������ �ڷ�ƾ ���
            StartCoroutine(iskilledScenario());
            Debug.Log("sysman :: is killed");
        }
    }

    private IEnumerator iskilledScenario()
    {
        // ui �ؽ�Ʈ ���
        playUITMP.text = "GAME OVER\nYOU DIE";
        playerSound.PlayOneShot(deathScream);
        // 5�� ���� ��� 
        // �ʵ� �� ���� ġ���, ���� ���� ���� 
        // ���⵵ �� ġ���
        // 1�� �� �ʱ� ��������
        yield return new WaitForSeconds(5.0f);
        RemoveZombies();
        RemoveWeaponsandStartGun();
        Init();
        numTrial++;
    }

    public void OnPlayerHit()
    {
        // ���� ��ũ��Ʈ��κ��� �ǰ� �� �� �ȴ�
        playerHP -= 1;
        Debug.Log("sysman :: onplayer hit call :: HP : " + playerHP);
        playerSound.PlayOneShot(dohClip);
    }

    private void RemoveZombies()
    {
        // �ʵ� �� ���� �� ����
        TobeRemovedsets = GameObject.FindGameObjectsWithTag("Zombies");

        foreach (GameObject toberemoved in TobeRemovedsets)
        {
            // ������ ����ź �ǰ��� ���԰� ���ÿ� ����ź ���� ��ġ �����ֱ�
            Destroy(toberemoved);
        }
    }

    private void RemoveWeaponsandStartGun()
    {
        // �ʵ� �� ��� ����� ��ŸƮ �� ����
        
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
        // ���̺� ��Ÿ�� �����κ��� �߻�
        StartCoroutine(WavestartAction());
        /*
        // ���� ���̺� ���� ��ƾ ����
        // ���� �÷��� �ø���
        FlagSpawner = true;
        FlagisIngame = true;
        FlagcheckingAlive = true;
        FlagWaveSuccess = false;
        // �� ������ ���� �� ������Ʈ
        // numZombies = ������Ʈ;
        numZombies = 15;
        // ��ŸƮ���� ������ ���� �������� ����
        Debug.Log("sysman :: wave start");
        */
    }

    private IEnumerator WavestartAction()
    {
        yield return new WaitForSeconds(5.0f);
        // ���� ���̺� ���� ��ƾ ����
        // ���� �÷��� �ø���
        FlagSpawner = true;
        FlagisIngame = true;
        FlagcheckingAlive = true;
        FlagWaveSuccess = false;
        // �� ������ ���� �� ������Ʈ
        // numZombies = ������Ʈ;
        numZombies = 10 + 2 * Days;
        if (Days % 2 == 0)
            nummageZombies = 1;
        else
            nummageZombies = 0;
        if (Days % 3 == 0)
            numBigZombies = 1;
        else
            numBigZombies = 0;


        // ��ŸƮ���� ������ ���� �������� ����
        Debug.Log("sysman :: wave start");
    }


    private void WaveEnd()
    {
        // ���� ���̺� ��Ʈ�ѷ��κ��� �߻� :: ���������� ���̺긦 ������
        // �߻�����? ���̺� ��ŸƮ ����, ��� ���� ���������ư�, �ʵ忡 ���� 0�����̰� �÷��̾� hp >0�̸�
        
        if (FlagWaveSuccess == true)
        {
            // ���̺� ���� �÷��� ���ֱ�
            FlagWaveSuccess = false;
            StartCoroutine(goNextDay());
            Debug.Log("sysman :: Wave END");
        }

    }

    private IEnumerator goNextDay()
    {
        // ���̺� Ŭ���� ���� �� ������ �ڷ�ƾ


        // playerUI�� �������� Ŭ���� �޽��� ��� :: ��� ų��, ��¥ ���
        // 10�� ���
        // days ī���� + 1 ���ֱ�
        // �÷��̾� ü�� Ǯü������ �ٽ�
        // �ٸ� ���� + ��Ÿ�Ͱ� �����ؼ� ���� ġ���
        // ���̺� ��Ÿ�� �� �ٽ� ����
        playUITMP.text = "STAGE CLEAR";
        yield return new WaitForSeconds(5.0f);
        Days++;
        playerHP = playerHPmax;

        // �°� �÷��׵� ���� ����
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
        // ���� �������
        dirlight.GetComponent<IngameLightSetting>().mainlight.intensity = 1.0f;
        playUITMP.text = "";
        // ���� ���� ���� �ʱ�ȭ
        RemoveWeaponsandStartGun();
        Table.GetComponent<GadgetSettings>().InitSettingwithStartGun();
    }


    private int existingZombieNum()
    {
        // �±׸� ���ؼ� �ʵ忡 �����ϴ� ���� �� ī��Ʈ
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Zombies");
        int zombieCounts = 0;
        for (int i = 0; i < objs.Length; i++)
        {
            zombieCounts++;
        }
        //Debug.Log("ZombieCount : " + zombieCounts);
        // ī��Ʈ �� ���� ���� ����
        return zombieCounts;
    }


    public void addZombieKill()
    {
        // ���� ���� ������ �����ֱ�
        numKill++;
    }


    // Update is called once per frame
    void Update()
    {
        // spawn flag�� �����ְ�, Ư���� �̸����� ���� �ʵ忡 �ְ�, ���� �� �����ؾ��ϸ�
        if (FlagSpawner == true && existingZombieNum() < zombieNumConstraint && numZombies > 0)
        {
            // ���� �ٿ��ֱ�
            // ������ ��ġ�� �ϳ� ����
            int randSpawnPos = Random.Range(0, 18);
            numZombies--;
            Instantiate(Zombie, spawnpointsetisthis[randSpawnPos].position, spawnpointsetisthis[randSpawnPos].rotation);
        }

        if (FlagSpawner == true && existingZombieNum() < zombieNumConstraint && nummageZombies > 0)
        {
            // ���� �ٿ��ֱ�
            // ������ ��ġ�� �ϳ� ����
            int randSpawnPos = Random.Range(0, 2);
            nummageZombies--;
            Instantiate(MageZombie, spawnpointsetisthis[randSpawnPos].position, spawnpointsetisthis[randSpawnPos].rotation);
        }

        if (FlagSpawner == true && existingZombieNum() < zombieNumConstraint && numBigZombies > 0)
        {
            // ���� �ٿ��ֱ�
            // ������ ��ġ�� �ϳ� ����
            int randSpawnPos = Random.Range(0, 18);
            numBigZombies--;
            Instantiate(BossZombie, spawnpointsetisthis[randSpawnPos].position, spawnpointsetisthis[randSpawnPos].rotation);
        }


        // �ΰ��� �÷��װ� ���� ���¿���, ���� �ʵ忡 0����, ������ ������ �͵� 0����
        if (FlagisIngame == true && existingZombieNum() == 0 && numZombies <= 0 && nummageZombies <= 0 && numBigZombies <= 0 && playerHP > 0)
        {
            // �ΰ��� �÷��״� ���ֱ�
            FlagisIngame = false;
            FlagSpawner = false;
            FlagcheckingAlive = false;
            // ���̺� ���� �÷��� ���ֱ�
            FlagWaveSuccess = true;
            // ���������� ���´ٰ� �� �� �ִ�.
            WaveEnd();
        }

        // �÷��̾��� ������� Ȯ��
        isKilled();

        if (playerHP <= 0)
            playTMP_infos.text = "Day " + Days + "\nHP : 0";
        else
            playTMP_infos.text = "Day " + Days + "\nHP : " + playerHP;

        playRecordTMP.text = "Trial : "+ numTrial + "\nTotal Kill : " + numKill;

    }
}