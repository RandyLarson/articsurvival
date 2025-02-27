using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class CameraPanningController : MonoBehaviour
{
    [SerializeField] private CameraSceneExtentMarker SceneBounds;
    [SerializeField] private CameraSceneExtentMarker SceneMargins;

    private bool HasFocus = true;

    public (Vector3 bottomLeft, Vector3 topRight) SceneExtentBoundingPoints => (SceneBounds.BottomLeft, SceneBounds.TopRight);
    public (Vector3 bottomLeft, Vector3 topRight) SceneMarginBoundingPoints => (SceneMargins.BottomLeft, SceneMargins.TopRight);

    public float SceneExtentMinX => SceneBounds.MinXWp;
    public float SceneExtentMaxX => SceneBounds.MaxXWp;
    public float SceneExtentMinY => SceneBounds.MinYWp;
    public float SceneExtentMaxY => SceneBounds.MaxYWp;
    public float SceneMarginMinX => SceneMargins.MinXWp;
    public float SceneMarginMaxX => SceneMargins.MaxXWp;
    public float SceneMarginMinY => SceneMargins.MinYWp;
    public float SceneMarginMaxY => SceneMargins.MaxYWp;

    public Vector3 ClampToSceneExtents(Vector3 position)
    {
        Vector3 clamped = new Vector3(
            Mathf.Clamp(position.x, SceneExtentMinX, SceneExtentMaxX),
            Mathf.Clamp(position.y, SceneExtentMinY, SceneExtentMaxY),
            0);
        return clamped;
    }
    public Vector3 ClampToSceneMargins(Vector3 position)
    {
        Vector3 clamped = new Vector3(
            Mathf.Clamp(position.x, SceneMarginMinX, SceneMarginMaxX),
            Mathf.Clamp(position.y, SceneMarginMinY, SceneExtentMaxY),
            0);
        return clamped;
    }

    public float CameraMinX { get; private set; }
    public float CameraMaxX { get; private set; }
    public float CameraMinY { get; private set; }
    public float CameraMaxY { get; private set; }
    public Camera Camera { get; private set; }


    void Start()
    {
        Initialize();
    }

    InputSystem_Actions PlayerInputHandler { get; set; }
    void RegisterInput()
    {
        PlayerInputHandler = new InputSystem_Actions();
        PlayerInputHandler.Enable();
        var playerMap = PlayerInputHandler.asset.FindActionMap("Player");
        playerMap.Disable();

        PlayerInputHandler.UI.ScrollDown.performed += ScrollDown_performed;
        PlayerInputHandler.UI.ScrollDown.canceled += ScrollDown_performed;
        PlayerInputHandler.UI.ScrollUp.performed += ScrollUp_performed;
        PlayerInputHandler.UI.ScrollUp.canceled += ScrollUp_performed;
        PlayerInputHandler.UI.ScrollLeft.performed += ScrollLeft_performed;
        PlayerInputHandler.UI.ScrollLeft.canceled += ScrollLeft_performed;
        PlayerInputHandler.UI.ScrollRight.performed += ScrollRight_performed;
        PlayerInputHandler.UI.ScrollRight.canceled += ScrollRight_performed;
        PlayerInputHandler.UI.Point.performed += Point_performed;
    }

    private void Point_performed(InputAction.CallbackContext obj)
    {
        var corners = GeneralUtilities.ViewPortCornersScreen;
        var pointerPos = obj.ReadValue<Vector2>();

        var thresholdX = (corners.tr.x - corners.bl.x) * GameConfig.Current.MapPanningThresholdPercentage;
        var thresholdY = (corners.tr.y - corners.bl.y) * GameConfig.Current.MapPanningThresholdPercentage;

        if (pointerPos.x > corners.tr.x - thresholdX)
            MousePanDelta.x = GameConfig.Current.MapPanningSpeed;
        else if (pointerPos.x < corners.bl.x + thresholdX)
            MousePanDelta.x = -GameConfig.Current.MapPanningSpeed;
        else
            MousePanDelta.x = 0;

        if (pointerPos.y > corners.tr.y - thresholdY/2 && pointerPos.y < corners.tr.y + thresholdY)
            MousePanDelta.y = GameConfig.Current.MapPanningSpeed;
        else if (pointerPos.y < corners.bl.y + thresholdY && pointerPos.y > corners.bl.y - thresholdY)
            MousePanDelta.y = -GameConfig.Current.MapPanningSpeed;
        else
            MousePanDelta.y = 0;




    }

    private void ScrollRight_performed(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
            PanDelta.x = 0;
        else
            PanDelta.x = GameConfig.Current.MapPanningSpeed;
    }

    private void ScrollLeft_performed(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
            PanDelta.x = 0;
        else
            PanDelta.x = -GameConfig.Current.MapPanningSpeed;
    }

    private void ScrollUp_performed(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
            PanDelta.y = 0;
        else
            PanDelta.y = GameConfig.Current.MapPanningSpeed;
    }

    private void ScrollDown_performed(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
            PanDelta.y = 0;
        else
            PanDelta.y = -GameConfig.Current.MapPanningSpeed;
    }

    Vector3 PanDelta;
    Vector3 MousePanDelta;

    private void Update()
    {
        if ( PanDelta != Vector3.zero)
        {
            Camera.transform.position = Camera.transform.position + (PanDelta * Time.deltaTime);
            SnapCameraIntoBounds();
        }
        else if (MousePanDelta != Vector3.zero)
        {
            Camera.transform.position = Camera.transform.position + (MousePanDelta * Time.deltaTime);
            SnapCameraIntoBounds();
        }
    }

    void UnRegisterInput()
    {
        if (PlayerInputHandler != null)
        {
            PlayerInputHandler.Disable();
            PlayerInputHandler.Player.Disable();
            PlayerInputHandler = null;
        }
    }

    bool IsInitialized { get; set; }
    void Initialize()
    {
        if (IsInitialized) return;

        IsInitialized = true;
        Camera = Camera.main;
        RegisterInput();
        RecalculateExtents();
    }

    void UnInitialize()
    {
        if (!IsInitialized) return;
        UnRegisterInput();
        IsInitialized = false;
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        UnInitialize();
    }

    private Vector2 ViewPortDimensions
    {
        get
        {
            Vector2 res = GeneralUtilities.ViewPortDimensions;
            // DiagnosticController.Current.Add("Panning", $"WPs {ord % 5}", $"({Time.frameCount}) - {res}");
            return res;
        }
    }

    Vector2 LastViewPortDimensions { get; set; }

    private void RecalculateExtents()
    {
        if (!IsInitialized)
            return;

        DetermineExtents();
        SnapCameraIntoBounds();
    }

    private void DetermineExtents()
    {
        LastViewPortDimensions = ViewPortDimensions;

        // DiagnosticController.Current.Add("Panning", $"Viewport {ord % 5}", $"({Time.frameCount}) - {LastViewPortDimensions}");

        int levelWidth = (int)SceneBounds.Width;

        // When the entire level fits within the current viewport, the camera should be fixed in the center of the level
        if (levelWidth < LastViewPortDimensions.x)
        {
            CameraMinX = SceneBounds.MinXWp + ViewPortDimensions.x / 2;
            CameraMaxX = CameraMinX;
        }
        else
        {
            CameraMinX = SceneBounds.MinXWp + (LastViewPortDimensions.x / 2);
            CameraMaxX = SceneBounds.MaxXWp - (LastViewPortDimensions.x / 2);
        }

        int levelHeight = (int)SceneBounds.Height;
        if (levelHeight < LastViewPortDimensions.y)
        {
            CameraMinY = SceneBounds.MinYWp + ViewPortDimensions.y / 2;
            CameraMaxY = CameraMinY;
        }
        else
        {
            CameraMinY = SceneBounds.MinYWp + (LastViewPortDimensions.y / 2);
            CameraMaxY = SceneBounds.MaxYWp - (LastViewPortDimensions.y / 2);
        }

        // DiagnosticController.Current.Add("Panning", $"Setting-CameraMinMax {ord++ % 5}", $"({Time.frameCount}) - Min: {CameraMinX}, Max: {CameraMaxX}");
    }

    public void SnapCameraIntoBounds()
    {
        Camera.transform.position = new Vector3(
            Mathf.Clamp(Camera.transform.position.x, CameraMinX, CameraMaxX),
            Mathf.Clamp(Camera.transform.position.y, CameraMinY, CameraMaxY),
            Camera.transform.position.z);
    }

}
