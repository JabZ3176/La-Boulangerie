using UnityEngine;
using UnityEngine.EventSystems;

public class MenuStartSelection : MonoBehaviour
{
    public GameObject firstSelectedButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}