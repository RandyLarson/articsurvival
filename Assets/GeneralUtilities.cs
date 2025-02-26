using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class GeneralUtilities
{
    private static PointerEventData UiPointerEventData = null;
    private static List<RaycastResult> UiDetectionRaycastResults = new List<RaycastResult>();

    private static (int, bool) IsOverUiElement_FrameResult { get; set; }
    //public static bool IsOverUIElement()
    //{
    //    if (Time.frameCount == IsOverUiElement_FrameResult.Item1)
    //        return IsOverUiElement_FrameResult.Item2;

    //    var res = IsOverUIElement(PointerAsScreenPosition());
    //    IsOverUiElement_FrameResult = (Time.frameCount, res);
    //    return res;
    //}

    //public static bool IsOverUIElement(Vector3 screenPosition)
    //{
    //    bool res = false;
    //    try
    //    {
    //        if (UiPointerEventData == null)
    //            UiPointerEventData = new PointerEventData(EventSystem.current);

    //        UiPointerEventData.position = screenPosition;

    //        EventSystem.current.RaycastAll(UiPointerEventData, UiDetectionRaycastResults);

    //        for (int index = 0; index < UiDetectionRaycastResults.Count; index++)
    //        {
    //            RaycastResult curRaysastResult = UiDetectionRaycastResults[index];
    //            if (curRaysastResult.gameObject.layer == GameConstants.LayerIdUi)
    //            {
    //                DiagnosticController.Current.Add("Ui Detection", "Current Object", curRaysastResult.gameObject);
    //                res = true;
    //                break;
    //            }
    //        }
    //    }
    //    catch (Exception)
    //    {

    //    }

    //    if (!res) DiagnosticController.Current.Add("Ui Detection", "Current Object", "None");
    //    DiagnosticController.Current.Add("Ui Detection", "IsOverUi", res);
    //    return res;
    //}

    public static bool ProtectedCall(Action action)
    {
        return ProtectedCall(action, null);
    }

    public static bool ProtectedCall(Action action, Action onException)
    {
        try
        {
            action?.Invoke();
            return true;
        }
        catch (Exception caught)
        {
            onException?.Invoke();
            return false;
        }
    }


    public static int CalculateAbsoluteSortOrder(float worldPosY)
    {
        int res = (int)(SortOrderMultiplier * (BaseAbsoluteSortOrder - worldPosY));
        if (res > short.MaxValue)
            res = short.MaxValue;
        return res;
    }

    public static int SortOrderMultiplier => 20;
    public static int BaseAbsoluteSortOrder => 1000;

    public static Vector3 PointerAsWorldPosition()
    {
        if (Pointer.current != null)
        {
            var pointerPos = Pointer.current.position.ReadValue();
            return Camera.main.ScreenToWorldPoint(pointerPos);
        }
        return Vector3.zero;
    }

    public static Vector3 PointerAsScreenPosition()
    {
        if (Pointer.current != null)
        {
            var pointerPos = Pointer.current.position.ReadValue();
            return pointerPos;
        }
        return Vector3.zero;
    }

    //public static bool IsPointWithinMainCameraViewport(Vector3 wp)
    //{
    //    Vector3 spLeft = GameController.MainCamera.ViewportToWorldPoint(Vector3.zero);
    //    Vector3 spRight = GameController.MainCamera.ViewportToWorldPoint(Vector3.one);

    //    return wp.x >= spLeft.x && wp.x <= spRight.x && wp.y >= spLeft.y && wp.y < spRight.y;
    //}

    public static bool IsPointWithinRect(RectTransform rectTransform, Vector3 worldPos)
    {
        if (rectTransform == null) return false;

        Vector3[] worldCorner = new Vector3[4];

        rectTransform.GetWorldCorners(worldCorner);
        Rect asRect = new Rect(worldCorner[0].x, worldCorner[0].y, worldCorner[2].x - worldCorner[0].x, worldCorner[2].y - worldCorner[0].y);
        return asRect.Contains(worldPos);
    }

    public static T ChooseOne<T>(this IEnumerable<T> fromCollection) where T : class
    {
        if (fromCollection == null) return null;

        T item = null;
        if (fromCollection.Any())
        {
            item = fromCollection.Skip(UnityEngine.Random.Range(0, fromCollection.Count())).Take(1).First();
        }

        return item;
    }

    public static Color ChooseOne(this IEnumerable<Color> fromCollection)
    {
        if (fromCollection == null) return Color.red;

        Color item = Color.red;
        if (fromCollection.Any())
        {
            item = fromCollection.Skip(UnityEngine.Random.Range(0, fromCollection.Count())).Take(1).First();
        }

        return item;
    }

    public static bool IsPointInRect(Vector3 pt, Transform t)
    {
        if (t == null) return false;

        if (t.TryGetComponent(out RectTransform ourRc))
        {
            Vector3[] worldCorner = new Vector3[4];

            ourRc.GetWorldCorners(worldCorner);
            Rect asRect = new Rect(worldCorner[0].x, worldCorner[0].y, worldCorner[2].x - worldCorner[0].x, worldCorner[2].y - worldCorner[0].y);
            return asRect.Contains(pt);
        }
        return false;
    }

    public static bool IsPointInRect(Vector3 pt, GameObject go) => go != null && IsPointInRect(pt, go.transform);

    /// <summary>
    /// Get, in world coordinates, dimensions of the main camera's viewport.
    /// </summary>
    public static Vector2 ViewPortDimensions
    {
        get
        {
            var wpLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
            var wpRight = Camera.main.ViewportToWorldPoint(Vector3.one);

            Vector2 res = new Vector2(wpRight.x - wpLeft.x, wpRight.y - wpLeft.y);
            return res;
        }
    }

    /// <summary>
    /// Get, in world coordinates, the bottom-left and top-right corners of the main camera's viewport.
    /// </summary>
    public static (Vector2 bl, Vector2 tr) ViewPortCornersWorld
    {
        get
        {
            var bl = Camera.main.ViewportToWorldPoint(Vector3.zero);
            var tr = Camera.main.ViewportToWorldPoint(Vector3.one);

            return (bl, tr);
        }
    }

    /// <summary>
    /// Get, in screen coordinates, the bottom-left and top-right corners of the main camera's viewport.
    /// </summary>
    public static (Vector2 bl, Vector2 tr) ViewPortCornersScreen
    {
        get
        {
            if (Camera.main == null)
                return (Vector2.zero, Vector2.zero);

            var bl = Camera.main.ViewportToScreenPoint(Vector3.zero);
            var tr = Camera.main.ViewportToScreenPoint(Vector3.one);

            return (bl, tr);
        }
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform, Canvas canvas)
    {
        if (canvas == null)
            return transform.rect;

        //Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect pixelAdj = RectTransformUtility.PixelAdjustRect(transform, canvas);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        Rect rect = new Rect(screenPos.x, screenPos.y, pixelAdj.width * canvas.scaleFactor, pixelAdj.height * canvas.scaleFactor);

        rect.x -= (transform.pivot.x * rect.width);
        rect.y -= (transform.pivot.y * rect.height);
        return rect;
    }


    public static Vector3 ClampToApartment(Vector3 pt)
    {
        try
        {
            //var cameraController = GameController.Current.CurrentLevelController.CameraController;
            //pt.x = cameraController.ClampToSceneExtentX(pt.x);
            //pt.x = Mathf.Clamp(pt.x, cameraController.SceneExtentMinX + 50, cameraController.SceneExtentMaxX - 50);
            //pt.y = Mathf.Clamp(pt.y, 200, 900);

            //// Avoid Clock
            //Vector3 vp = cameraController.Camera.WorldToViewportPoint(pt);
            //if (vp.x > .7f && vp.y > .7f)
            //{
            //    vp.x = Mathf.Clamp(vp.x, 0, .7f);
            //    vp.y = Mathf.Clamp(vp.y, 0, .7f);
            //    pt = cameraController.Camera.ViewportToWorldPoint(vp);
            //}

            return pt;
        }
        catch (Exception)
        {
            return pt;
        }
    }

    public static Vector3 PickPointInApartment()
    {
        try
        {
            //var cameraController = GameController.Current.CurrentLevelController.CameraController;
            //float pickedX = UnityEngine.Random.Range(cameraController.SceneExtentMinX, cameraController.SceneExtentMaxX);
            //float pickedY = UnityEngine.Random.Range(200, 900);

            //var res = ClampToApartment(new Vector3(pickedX, pickedY));
            //return res;
            return Vector3.zero;
        }
        catch (Exception)
        {
            return new Vector3(900, 500, 0);
        }
    }

    public static bool IsPointInRect(GameObject go, Vector3 pt)
    {
        if (go == null) return false;

        if (go.TryGetComponent(out RectTransform rt))
        {
            Vector3[] worldCorner = new Vector3[4];

            rt.GetWorldCorners(worldCorner);
            Rect asRect = new Rect(worldCorner[0].x, worldCorner[0].y, worldCorner[2].x - worldCorner[0].x, worldCorner[2].y - worldCorner[0].y);
            return asRect.Contains(pt);
        }
        return false;
    }

    public static Color Desaturate(Color value)
    {
        Color.RGBToHSV(value, out float H, out float S, out float V);
        S = 0;
        var rgbColor = Color.HSVToRGB(H, S, V);
        return rgbColor;
    }


}
