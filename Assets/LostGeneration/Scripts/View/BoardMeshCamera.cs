using UnityEngine;

/// <summary>
/// Triggers Cell builds and enables/disables BlockMeshes based on Camera visibility
/// </summary>
public class BoardMeshCamera : MonoBehaviour
{
    public Camera Camera = Camera.main;
    public BoardMesh BoardMesh;

    #region MonoBehaviour
    private void LateUpdate()
    {
        
    }
    #endregion MonoBehaviour
}