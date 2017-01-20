using UnityEngine;

public class BoardCameraController : MonoBehaviour {
    #region EditorFields
    public float RegularSpeed = 8f;
    public float HighSpeed = 14f;
    public float CorrectionTime = 0.5f;
    public Transform Camera;
    #endregion EditorFields

    private BoardCamera _boardCamera;

    private bool _isManuallyPanning = false;

    #region MonoBehaviour
    private void Awake() {
        _boardCamera = GetComponent<BoardCamera>();
    }

    public bool DebugSprint;

    private void Update () {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        bool sprint = Input.GetButton("Sprint");

        if (dx != 0 || dy != 0) {
            _isManuallyPanning = true;
            _boardCamera.CancelPan ();

            Vector3 forward = Vector3.ProjectOnPlane(Camera.forward, Vector3.up).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            Vector3 offset = (dx * right + dy * forward) *
                             (sprint ? HighSpeed : RegularSpeed) * Time.deltaTime;
            transform.position += offset;
        } else {
            if (_isManuallyPanning) {
                _boardCamera.Pan(PointVector.ToPoint(transform.position), CorrectionTime);
                _isManuallyPanning = false;
            }
        }
	}
    #endregion MonoBehaviour

}
