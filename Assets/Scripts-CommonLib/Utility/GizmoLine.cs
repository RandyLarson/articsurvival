using UnityEngine;

public class GizmoLine : MonoBehaviour
{
    public Color LineColor = Color.green;
    public float LineLength = 1000;
    public bool Vertical = true;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = LineColor;
        Vector3 startingPoint = Vector3.down;
        Vector3 endPoint = Vector3.up;

        if (!Vertical)
        {
            startingPoint = Vector3.right;
            endPoint = Vector3.left;
        }

        Gizmos.DrawLine(transform.position, transform.position + startingPoint * LineLength);
        Gizmos.DrawLine(transform.position, transform.position + endPoint * LineLength);
    }

}
