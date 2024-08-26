using UnityEngine;
using System;

public class FlyDataSender : MonoBehaviour
{
    //Recomended Use: first variable for current amount, second variable for full amount
    public static Action<int,int> OnDataNotify;

    public void NotifyData(int fly, int Full)
    {
        OnDataNotify?.Invoke(fly,Full);
    }
}
