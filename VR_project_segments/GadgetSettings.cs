using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetSettings : MonoBehaviour
{
    // 이 스크립트는 가젯 테이블의 초기화 및 관리를 위함
    public Transform spawnPoint_startgun;
    public Transform spawnPoint_revolver;
    public Transform spawnPoint_smg;
    public Transform spawnPoint_pointer;
    public Transform spawnPoint_grenadesets;
    public Transform spawnPoint_grenadesets1;
    public Transform spawnPoint_grenadesets2;

    public GameObject startgun;
    public GameObject revolver;
    public GameObject smg;
    public GameObject pointer;
    public GameObject grenadesets;

    public GameObject TobeRemoved;

    // Start is called before the first frame update
    void Start()
    {
        //spawnOnlyStartGun();
        //InitSettingwithStartGun();
    }


    public void InitSetting()
    {
        Instantiate(revolver, spawnPoint_revolver.position, spawnPoint_revolver.rotation);
        Instantiate(smg, spawnPoint_smg.position, spawnPoint_smg.rotation);
        Instantiate(pointer, spawnPoint_pointer.position, spawnPoint_pointer.rotation);
        Instantiate(grenadesets, spawnPoint_grenadesets.position, spawnPoint_grenadesets.rotation);
        Instantiate(grenadesets, spawnPoint_grenadesets1.position, spawnPoint_grenadesets1.rotation);
        Instantiate(grenadesets, spawnPoint_grenadesets2.position, spawnPoint_grenadesets2.rotation);
    }
    public void InitSettingwithStartGun()
    {
        Instantiate(startgun, spawnPoint_startgun.position, spawnPoint_startgun.rotation);
        Instantiate(revolver, spawnPoint_revolver.position, spawnPoint_revolver.rotation);
        Instantiate(smg, spawnPoint_smg.position, spawnPoint_smg.rotation);
        Instantiate(pointer, spawnPoint_pointer.position, spawnPoint_pointer.rotation);
        Instantiate(grenadesets, spawnPoint_grenadesets.position, spawnPoint_grenadesets.rotation);
        Instantiate(grenadesets, spawnPoint_grenadesets1.position, spawnPoint_grenadesets1.rotation);
        Instantiate(grenadesets, spawnPoint_grenadesets2.position, spawnPoint_grenadesets2.rotation);
    }
    public void spawnOnlyStartGun()
    {
        Instantiate(startgun, spawnPoint_startgun.position, spawnPoint_startgun.rotation);
    }

    private void RemoveAll()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        //RemoveAll();
    }
}
