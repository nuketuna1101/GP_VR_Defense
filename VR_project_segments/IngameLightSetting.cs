using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameLightSetting : MonoBehaviour
{
    public Light mainlight;

    // Start is called before the first frame update
    void Start()
    {
        mainlight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
