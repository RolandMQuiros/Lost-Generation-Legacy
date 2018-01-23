using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;
using LostGen.Util;

namespace LostGen.Display {
    /// <summary>
    /// A camera constrained to discrete <see cref="Point"/> on a <see cref="Board"/>
    /// </summary>
    /// <remarks>
    /// A <see cref="GameObject"/> with a BoardCamera attached will pan, zoom, and rotate in the world space along a
    /// discrete grid, as defined by <see cref="PointVector"/>. Each motion is performed across a period of time, and can be
    /// queued so that the motions happen one after another.
    /// </remarks>
    public class BoardCamera : MonoBehaviour {
        #region UnityFields
        [TooltipAttribute("Transform of the camera object")]
        [SerializeField]private Camera _camera;
        #endregion UnityFields
        private enum MotionType {
            Pan, Zoom, Rotate
        };
        private MotionType _motionType;
        private IEnumerator _motion;
        private Queue<IEnumerator> _motions = new Queue<IEnumerator>();
        private Vector3 _originalOffset;

        #region MotionDefs
        /// <summary>
        /// Creates a coroutine enumerator that translates this camera over a period of time.
        /// </summary>
        /// <param name="target">The destination position</param>
        /// <param name="duration">Time, in seconds, to move from current position to the target</param>
        /// <param name="after">Function called at the end of the motion</param>
        /// <returns>A coroutine enumerator that finishes after the given duration has elapsed</returns>
        private IEnumerator Pan(Vector3 target, float duration, Action after = null) {
            _motionType = MotionType.Pan;
            float time = 0f;
            while (time < duration) {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, target, time / duration);
                yield return null;
            }
            transform.position = target;

            if (after != null) { after(); }
        }

        /// <summary>
        /// Creates a coroutine iterator that scales this object's local position relative to its
        /// starting one, over a period of time.
        /// </summary>
        /// <param name="scale">The zoom factor. Applied to this camera's local position, moving it further or closer to the
        /// parent object's origin.</param>
        /// <param name="duration">Time, in seconds, to move from current zoom level to new zoom level</param>
        /// <param name="after">Function called at the end of the motion</param>
        /// <returns>A coroutine enumerator that finishes after the given duration has elapsed</returns>
        private IEnumerator Zoom(float scale, float duration, Action after) {
            _motionType = MotionType.Zoom;
            float time = 0f;
            
            while (time < duration) {
                time += Time.deltaTime;
                _camera.transform.localPosition =
                    Vector3.Lerp(
                        _camera.transform.localPosition,
                        _originalOffset / scale,
                        time / duration
                    );
                yield return null;
            }
            _camera.transform.localPosition = _originalOffset / scale;

            if (after != null) { after(); }
        }

        /// <summary>
        /// Creates a coroutine iterator rotates the camera around its parent transform, over a period of time.  
        /// </summary>
        /// <param name="angle">Relative angle, in degrees, to rotate the camera</param>
        /// <param name="duration">Time, in seconds, to rotate by the given angle</param>
        /// <param name="after">Function called at the end of the motion</param>
        /// <returns>A coroutine enumerator that finishes after the given duration has elapsed</returns>
        private IEnumerator Rotate(float angle, float duration, Action after = null) {
            _motionType = MotionType.Rotate;
            Quaternion target = Quaternion.Euler(0f, angle, 0f);
            float time = 0f;

            while (time < duration) {
                transform.rotation = Quaternion.Lerp(transform.rotation, target, time / duration);
                yield return null;
            }
            transform.rotation = target;
            if (after != null) { after(); }
        }
        #endregion MotionDefs

        #region MonoBehaviour
        private void Start() {
            _originalOffset = _camera.transform.localPosition;
        }

        #endregion MonoBehaviour

        #region MotionMethods
        /// <summary>
        /// Immediately stops a panning motion if one is currently in progress. Does not clear the motion queue.
        /// </summary>
        public void CancelPan() {
            if (_motion != null && _motionType == MotionType.Pan) {
                StopCoroutine(_motion);
            }
        }

        /// <summary>
        /// Immediately instructs the camera to translate to the given Point on the board. Overrides any currently
        /// running pans, and clears the motion queue. 
        /// </summary>
        /// <param name="point">Where to move the camera</param>
        /// <param name="duration">Time, in seconds, to move from current position to the target</param>
        public void Pan(Point point, float duration) {
            Vector3 end = PointVector.ToVector(point);

            if (_motion != null && !(_motionType == MotionType.Pan)) {
                StopCoroutine(_motion);
            }
            _motion = Pan(end, duration, null);
            StartCoroutine(_motion);
            _motions.Clear();
        }

        /// <summary>
        /// Enqueues a translation motion. Enqueued motions run one after another.
        /// </summary>
        /// <param name="point">Where to move the camera</param>
        /// <param name="duration">Time, in seconds, to move from current position to the target</param>
        public void AddPan(Point point, float duration) {
            Vector3 end = PointVector.ToVector(point);
            _motions.Enqueue(Pan(end, duration, PopMotion));
            if (_motion == null) { PopMotion(); }
        }

        /// <summary>
        /// Immediately instructs the camera to zoom by the given scale. Overrides any currently running motions, and clears
        /// the motion queue. 
        /// </summary>
        /// <param name="scale">Zoom scale, relative to the camera's initial local position</param>
        /// <param name="duration">Time, in seconds, to zoom from current scale to the given one</param>
        public void Zoom(float scale, float duration) {
            if (_motion != null) {
                StopCoroutine(_motion);
            }
            _motion = Zoom(scale, duration, null);
            StartCoroutine(_motion);
            _motions.Clear();
        }

        /// <summary>
        /// Enqueues a zooming motion. Queued motions run one after another.
        /// </summary>
        /// <param name="scale">Zoom scale, relative to the camera's initial local position</param>
        /// <param name="duration">Time, in seconds, to zoom from current scale to the given one</param>
        public void AddZoom(float scale, float duration) {
            _motions.Enqueue(Zoom(scale, duration, PopMotion));
            if (_motion == null) { PopMotion(); }
        }

        /// <summary>
        /// Immediately instructs the camera to rotate by the given angle. Overrides any currently running rotations, and
        /// clears the motion queue. 
        /// </summary>
        /// <param name="angle">Number of degrees to rotate, relative to the current rotation</param>
        /// <param name="duration">Time, in seconds, to rotate from current angle to the given one</param>
        public void Rotate(float angle, float duration) {
            if (_motion != null && !(_motionType == MotionType.Rotate)) {
                StopCoroutine(_motion);
            }
            _motion = Rotate(angle, duration, null);
            StartCoroutine(_motion);
            _motions.Clear();
        }

        /// <summary>
        /// Enqueues a zooming motion. Queued motions run one after another.
        /// </summary>
        /// <param name="scale">Zoom scale, relative to the camera's initial local position</param>
        /// <param name="duration">Time, in seconds, to zoom from current scale to the given one</param>
        public void AddRotate(float angle, float duration) {
            _motions.Enqueue(Rotate(angle, duration, PopMotion));
            if (_motion == null) { PopMotion(); }
        }

        /// <summary>
        /// If more motions are queued, this dequeues one and runs it.
        /// </summary>
        private void PopMotion() {
            if (_motions.Count > 0) {
                _motion = _motions.Dequeue();
                StartCoroutine(_motion);
            } else {
                _motion = null;
            }
        }
        #endregion MotionMethods
    }
}