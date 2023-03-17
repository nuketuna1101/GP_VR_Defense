using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPointExplosion : MonoBehaviour
{
    private AudioSource bombSound;
    public AudioClip bombSoundClip;
    public ParticleSystem BombBlastEffect;


    // Start is called before the first frame update
    void Start()
    {
        bombSound = GetComponent<AudioSource>();
        bombSound.PlayOneShot(bombSoundClip);
        BombBlastEffect.Play();
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
