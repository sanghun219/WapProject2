﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.GetInst().InitSoundinAwake();
        SoundManager.GetInst().PlayMusic("Title");
    }

   
}
