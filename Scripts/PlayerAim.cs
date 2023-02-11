using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public float maxSize = 8f;

    public void RenderArrow(Vector3 startPoint, Vector3 endPoint)
    {
        // draws arrow from start to end point
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        // gets color depending on length of arrow
        Color color = Color.Lerp(Color.green, Color.red, Mathf.Clamp((Vector3.Distance(startPoint, endPoint) / maxSize), 0, 1));

        // sets arrow color
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void EraseArrow()
    {
        // erases arrow
        lineRenderer.positionCount = 0;
    }
}
