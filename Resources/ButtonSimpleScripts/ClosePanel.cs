using UnityEngine;
public class ClosePanel : MonoBehaviour
{
    [SerializeField] private GameObject PanelToOpen;

    public void Execute()
    {
        PanelToOpen.SetActive(false);
    }
}
