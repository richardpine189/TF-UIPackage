
using UnityEngine;
using UnityEngine.UI;

public class FlyyDataReceiverImage : HUDElementReceiver
{
    [SerializeField] private Image fillImage;
    void Awake()
    {
        FlyDataSender.OnDataNotify += UpdateHUD;
    }
    private void OnDestroy()
    {
        FlyDataSender.OnDataNotify -= UpdateHUD;
    }
    private void UpdateHUD(int fly, int Full)
    {
        float factor = fly / (float)Full;
        fillImage.fillAmount = factor;
    }
}
