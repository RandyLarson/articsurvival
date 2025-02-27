using UnityEngine;

public class CameraSceneExtentMarker : MonoBehaviour
{
    [SerializeField] private GameObject Lhs;
    [SerializeField] private GameObject Rhs;
    [SerializeField] private GameObject Top;
    [SerializeField] private GameObject Bottom;

    [SerializeField] public Vector3 BottomLeft { get; private set; }
    [SerializeField] public Vector3 TopRight { get; private set; }

    private void Start()
    {
        BottomLeft = new Vector3(MinXWp, MinYWp);
        TopRight = new Vector3(MaxXWp, MaxYWp);
    }

    public float MinXWp { get => Lhs.transform.position.x; }
    public float MaxXWp { get => Rhs.transform.position.x; }
    public float MinYWp { get => Bottom.transform.position.y; }
    public float MaxYWp { get => Top.transform.position.y; }

    public float Width => MaxXWp - MinXWp;
    public float Height => MaxYWp - MinYWp;
}