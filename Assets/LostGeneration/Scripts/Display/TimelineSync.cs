using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
public class TimelineSync : MonoBehaviour {
		public int Step {
			get { return _step; }
			set { SetStep(value); }
		}
		public int Count {
			get { return (_timelines.Count == 0) ? 0 : _timelines.Select(t => t.Count).Max(); }
		}
		[Serializable]public class StepChangeEvent : UnityEvent<int, int> { }
		public StepChangeEvent StepChanged;
		public UnityEvent AllPointsSpent;
		private List<Timeline> _timelines = new List<Timeline>();
		private int _step = 0;
		private PlayerSkillController _skillController;
		[SerializeField]private int _debugStep;
		public void AddTimeline(Timeline timeline) {
			if (timeline != null) {
				_timelines.Add(timeline);
				
				timeline.ActionDone += OnStepChanged;
				timeline.ActionUndone += OnStepChanged;

				timeline.SetStep(_step);
				SetStep(_step);
			}
		}
		public bool RemoveTimeline(Timeline timeline) {
			timeline.ActionDone -= OnStepChanged;
			timeline.ActionUndone -= OnStepChanged;
			return _timelines.Remove(timeline);
		}
		public void SetStep(int step) {
			int from = _step;
			int to = Math.Min(Math.Max(0, step), Count);
			_step = to;
			_timelines.ForEach(t => { t.Step = _step; });
			StepChanged.Invoke(to, from);
		}
		public void ApplyTimelines() {
			_timelines.ForEach(t => {
				t.Apply();
				t.Clear();
			});
		}

		private void OnStepChanged(PawnAction action) {
			// Synchronize the steps
			Timeline timeline = action.Owner.RequireComponent<Timeline>();
			SetStep(timeline.Step);

			// Check each timeline
			bool ready = _timelines.Aggregate(true, (acc, t) => {
				ActionPoints actionPoints = t.Pawn.GetComponent<ActionPoints>();
				int timelinePoints = t.Actions.Sum(a => a.Cost);
				return acc || actionPoints.Max == timelinePoints;
			});
		}

		#region MonoBehaviour
		private void Awake() {
			_skillController = GetComponent<PlayerSkillController>();
		}
		private void Update() {
			_debugStep = _step;
		}
		#endregion MonoBehaviour
	}
}