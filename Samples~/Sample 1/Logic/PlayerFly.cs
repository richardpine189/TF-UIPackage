using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFly : MonoBehaviour
{
    [SerializeField] private int fullFly = 10;
    [SerializeField] private int currentFly = 2;
    private FlyDataSender _flyDataSender;

    private void Awake()
    {
        _flyDataSender = gameObject.GetComponent<FlyDataSender>();
    }

    void Start()
    {
        _flyDataSender.NotifyData(currentFly,fullFly);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            _flyDataSender.NotifyData(++currentFly,fullFly);
        if (Input.GetKeyDown(KeyCode.W))
            _flyDataSender.NotifyData(--currentFly,fullFly);
    }
}
