using System;
using UnityEngine;

public class ArcticGameConroller : MonoBehaviour
{
    void Start()
    {
        RegisterInput();
    }

    InputSystem_Actions InputActions { get; set; }

    private void RegisterInput()
    {
        InputActions = new InputSystem_Actions();
        InputActions.UI.Click.performed += Click_performed;
        InputActions.Enable();
    }

    private void Click_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.phase != UnityEngine.InputSystem.InputActionPhase.Performed) return;

        HandleClick();
    }

    private void HandleClick()
    {
        GameObject hitItem = ScanForClickedItem(GeneralUtilities.PointerAsWorldPosition());
        if (hitItem == null) return;

        if (hitItem.TryGetComponent(out PersonController pc))
            HandlePlayerClick(pc);
    }

    PersonController CurrentlySelectedPerson { get; set; }

    private void HandlePlayerClick(PersonController pc)
    {
        if (pc == null) return;

        pc.ToggleSelection();
        CurrentlySelectedPerson = pc.IsSelected ? pc : null;
    }

    private GameObject ScanForClickedItem(Vector3 wp)
    {
        Collider2D hitItem = Physics2D.OverlapPoint(wp);

        if (hitItem == null) return null;
        return hitItem.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
