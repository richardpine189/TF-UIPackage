using UnityEngine;

public class TimeLogic : MonoBehaviour
{
    [SerializeField] private float time = 90;

    private float _currentTime;
    private TimeFillDataSender _timeFillDataSender;
    private TimeTextDataSender _timeTextDataSender;

    private void Awake()
    {
        _timeFillDataSender = gameObject.GetComponent<TimeFillDataSender>();
        _timeTextDataSender = gameObject.GetComponent<TimeTextDataSender>();
    }

    void Start()
    {
        _currentTime = time;
        
        _timeFillDataSender.NotifyData(_currentTime,time);
        _timeTextDataSender.NotifyData(_currentTime,time, true);
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime = time - Time.time;
        _timeFillDataSender.NotifyData(_currentTime,time);
        _timeTextDataSender.NotifyData(_currentTime,time, true);
    }
}
