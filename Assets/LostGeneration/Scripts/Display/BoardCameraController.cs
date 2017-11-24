using UnityEngine;

public class BoardCameraController : MonoBehaviour {
    #region EditorFields
    [SerializeField]private string _verticalInput;
	[SerializeField]private string _horizontalInput;
    [SerializeField]private string _rotateInput;
    [SerializeField]private string _zoomInput;
    [SerializeField]private string _shiftPlaneInput;
    [SerializeField]private string _sprintInput;

    [SerializeField]private float _regularSpeed = 8f;
    [SerializeField]private float _highSpeed = 14f;
    [SerializeField]private float _correctionTime = 0.5f;
    [SerializeField]private float _rotationSpeed = 10f;
    [SerializeField]private float _zoomSpeed = 1f;
    [SerializeField]private float _maxScale = 5f;
    [SerializeField]private float _minScale = 0.1f;
    [SerializeField]private Transform _camera;
    #endregion EditorFields

    private BoardCamera _boardCamera;

    private bool _isManuallyPanning = false;
    private float _rotation = 0f;
    private float _scale = 1f;

    #region MonoBehaviour
    private void Awake() {
        _boardCamera = GetComponent<BoardCamera>();
        _rotation = transform.rotation.eulerAngles.y;
    }

    public bool DebugSprint;

    private void Update () {
        float dx = Input.GetAxis(_horizontalInput);
        float dz = Input.GetAxis(_verticalInput);
        float dy = Input.GetAxis(_shiftPlaneInput);
        float dr = Input.GetAxis(_rotateInput);
        float ds = Input.GetAxis(_zoomInput);
        
        bool sprint = Input.GetButton(_sprintInput);

        if (dx != 0f || dz != 0f || dy != 0f) {
            _isManuallyPanning = true;
            _boardCamera.CancelPan();

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

        if (dr != 0f) {
            _rotation += dr * _rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(_rotation, Vector3.up);
        }

        if (ds != 0f) {
            _scale = Mathf.Clamp(_scale + (ds * _zoomSpeed * Time.deltaTime), _minScale, _maxScale);
            transform.localScale = _scale * Vector3.one;

            // _camera.transform.rotation = Quaternion.LookRotation(
            //     transform.position - _camera.transform.position
            // );
        }
	}
    #endregion MonoBehaviour

}
