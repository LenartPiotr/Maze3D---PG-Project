using UnityEngine;

public class DesignCube : MonoBehaviour
{
    public Vector3Int FitToGrid()
    {
        Vector3 pos = transform.position;
        Vector3Int newPosition = new Vector3Int(
                Mathf.RoundToInt(pos.x),
                Mathf.RoundToInt(pos.y),
                Mathf.RoundToInt(pos.z)
            );
        transform.position = newPosition;
        return newPosition;
    }
    public void FitScaleAndRotation()
    {
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
    }
}
