using UnityEngine;
using System;

public class EnergyDataSender : MonoBehaviour
{
    //Recomended Use: first variable for current amount, second variable for full amount
    public static Action<int,int> OnDataNotify;

    public void NotifyData(int energy, int Full)
    {
        OnDataNotify?.Invoke(energy,Full);
    }
}
