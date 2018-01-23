using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;
using LostGen.Util;
using ByteSheep.Events;

namespace LostGen.Display {
	public class TimelineSync : MonoBehaviour {
		public int Step {
			get { return _step; }
			set { SetStep(value); }
		}
		public int Count {
			get { return _max; }
		}
		public QuickIntEvent StepChanged;
		public QuickIntEvent CountChanged;
		public QuickEvent PointsNotSpent;
		public QuickEvent AllPointsSpent;
		private List<Timeline> _timelines = new List<Timeline>();
		private int _step = 0;
		private int _max = 0;
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
			step = Math.Min(Math.Max(0, step), Count);
			_step = step;
			_timelines.ForEach(t => { t.Step = _step; });
			StepChanged.Invoke(_step);
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
				return acc && actionPoints.Max == t.Cost;
			});
			if (ready) {AllPointsSpent.Invoke(); }
			else { PointsNotSpent.Invoke(); }

			// Update the maximum count
			int newMax = _timelines.Select(t => t.Count).Max();
			if (_max != newMax) {
				_max = newMax;
				CountChanged.Invoke(_max);
			}
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