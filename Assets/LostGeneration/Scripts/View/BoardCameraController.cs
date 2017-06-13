using UnityEngine;

public class BoardCameraController : MonoBehaviour {
    #region EditorFields
    [SerializeField]private float _regularSpeed = 8f;
    [SerializeField]private float _highSpeed = 14f;
    [SerializeField]private float _correctionTime = 0.5f;
    [SerializeField]private Transform _camera;
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
        float dz = Input.GetAxis("Vertical");
        float dy = Input.GetAxis("Shift Plane");
        bool sprint = Input.GetButton("Sprint");

        if (dx != 0 || dz != 0 || dy != 0) {
            _isManuallyPanning = true;
            _boardCamera.CancelPan ();

            Vector3 forward = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 up = Vector3.Cross(forward, right).normalized;

            Vector3 offset = (dx * right + dz * forward + dy * up * 2f) *
                             (sprint ? _highSpeed : _regularSpeed) * Time.deltaTime;
            transform.position += offset;
        } else {
            if (_isManuallyPanning) {
                _boardCamera.Pan(PointVector.ToPoint(transform.position), _correctionTime);
                _isManuallyPanning = false;
            }
        }
	}
    #endregion MonoBehaviour

}
