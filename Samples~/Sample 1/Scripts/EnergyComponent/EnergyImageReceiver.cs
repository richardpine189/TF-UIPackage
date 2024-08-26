
using UnityEngine;
using UnityEngine.UI;

public class EnergyDataReceiverImage : HUDElementReceiver
{
    [SerializeField] private Image fillImage;
    void Awake()
    {
        EnergyDataSender.OnDataNotify += UpdateHUD;
    }
    private void OnDestroy()
    {
        EnergyDataSender.OnDataNotify -= UpdateHUD;
    }
    private void UpdateHUD(int energy, int Full)
    {
        float factor = energy / (float)Full;
        fillImage.fillAmount = factor;
    }
}
