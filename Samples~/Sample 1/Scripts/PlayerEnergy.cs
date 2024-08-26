using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [SerializeField] private int fullEnergy = 10;
    [SerializeField] private int currentEnergy = 2;
    private EnergyDataSender _energyDataSender;

    private void Awake()
    {
        _energyDataSender = gameObject.GetComponent<EnergyDataSender>();
    }

    void Start()
    {
        _energyDataSender.NotifyData(currentEnergy,fullEnergy);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            _energyDataSender.NotifyData(++currentEnergy,fullEnergy);
        if (Input.GetKeyDown(KeyCode.S))
            _energyDataSender.NotifyData(--currentEnergy,fullEnergy);
    }
}
