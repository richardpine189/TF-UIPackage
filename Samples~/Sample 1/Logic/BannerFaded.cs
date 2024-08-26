using UnityEngine;

public class BannerFaded : MonoBehaviour
{
    [SerializeField] private int _duration = 2;
    void Start()
    {
        gameObject.GetComponent<BannerTextDataSender>().NotifyData(_duration,"Hola estoy en el nivel 1");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            gameObject.GetComponent<BannerTextDataSender>().NotifyData(_duration,"Hola estoy en el nivel 2?");
        }
    }
}
