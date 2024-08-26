using UnityEngine;
public class OpenPanel : MonoBehaviour
{
    [SerializeField] private GameObject PanelToOpen;

    public void Execute()
    {
        PanelToOpen.SetActive(true);
    }
}
