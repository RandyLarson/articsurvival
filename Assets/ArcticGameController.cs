using System;
using UnityEngine;

public class ArcticGameController : MonoBehaviour
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
        InputActions.UI.RightClick.performed += RightClick_performed;
        InputActions.UI.Cancel.performed += Cancel_performed;
        InputActions.Enable();
    }

    private void Cancel_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ClearSelection();
    }

    private void RightClick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ClearSelection();
    }

    private void Click_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.phase != UnityEngine.InputSystem.InputActionPhase.Performed) return;

        HandleClick();
    }

    private void HandleClick()
    {
        var clickedPos = GeneralUtilities.PointerAsWorldPosition();
        GameObject hitItem = ScanForClickedItem(clickedPos);
        if (hitItem == null)
        {
            if (CurrentPerson != null)
            {
                CurrentPerson.SetCurrentMovementDestination(clickedPos);
            }
            else
            {
                ClearSelection();
            }
        }
        else
        {
            if (hitItem.TryGetComponent(out PersonController pc))
            {
                if (CurrentSelection != null && CurrentSelection != pc)
                    CurrentSelection.SetSelected(false);

                HandlePlayerClick(pc);
            }
            else if (hitItem.TryGetComponent(out GamePiece generalGamePiece))
            {
                HandleNonPlayerClick(generalGamePiece, clickedPos);
            }

        }
    }

    private void HandleNonPlayerClick(GamePiece generalGamePiece, Vector3 clickedPos)
    {
        if (CurrentSelection == generalGamePiece)
        {
            ClearSelection();
            return;
        }

        if (CurrentPerson != null)
        {
            CurrentPerson.SetCurrentMovementDestination(clickedPos);
            return;
        }

        ClearSelection();
        CurrentSelection = generalGamePiece;
        CurrentSelection.SetSelected(true);
    }

    void ClearSelection()
    {
        if (CurrentSelection != null)
            CurrentSelection.SetSelected(false);
        CurrentSelection = null;
        CurrentPerson = null;
    }

    GamePiece CurrentSelection { get; set; }
    PersonController CurrentPerson { get; set; }

    private void HandlePlayerClick(PersonController pc)
    {
        if (pc == null) return;

        pc.ToggleSelection();
        CurrentSelection = pc.IsSelected ? pc : null;
        CurrentPerson = pc.IsSelected ? pc : null;
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
