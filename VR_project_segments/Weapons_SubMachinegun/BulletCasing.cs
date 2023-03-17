using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasing : MonoBehaviour
{
    // bullet casing only exists for CasingMaxLifetime
    private float CasingMaxLifetime = 3.0f;
    private float elapsedtime = 0.0f;
    // audio effects :: for gun shot
    private AudioSource casingSound;
    public AudioClip casingSoundclip;

    // Start is called before the first frame update
    void Start()
    {
        casingSound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // casing collision sounds
        casingSound.PlayOneShot(casingSoundclip);

    }

    // Update is called once per frame
    void Update()
    {
        // time updates
        elapsedtime += Time.deltaTime;
        if (CasingMaxLifetime <= elapsedtime)
        {
            Destroy(gameObject);
        }
    }
}
