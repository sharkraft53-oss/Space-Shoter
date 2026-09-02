using UnityEngine;

public class ListenEvent : MonoBehaviour
{
    public InitEvent initEvent;

    private void Start()
    {
        initEvent.OnClick.AddListener(OnClickEvent);
    }

    private void OnClickEvent()
    {
        Debug.Log("Нажали");
    }
}
