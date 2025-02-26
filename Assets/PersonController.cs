using System;
using UnityEngine;

public class PersonController : MonoBehaviour
{
    public bool IsSelected { get; set; }
    [SerializeField] private GameObject SelectedHighlight;

    void Start()
    {
        RegisterInput();
    }


    private void RegisterInput()
    {
    }

    public void OnClick()
    {
        ToggleSelection();
    }

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
        SelectedHighlight.gameObject.SetActive(IsSelected);
    }
}
