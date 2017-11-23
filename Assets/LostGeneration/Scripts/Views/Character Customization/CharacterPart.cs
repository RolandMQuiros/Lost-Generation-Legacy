using System;
using System.Collections.Generic;
using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterPart : UIView {
    public _string Name;
    public struct BlendWeight {
        public const int MIN_WEIGHT = 0;
        public const int MAX_WEIGHT = 100;
        public string Name;
        private int _weight;
        public int Weight {
            get { return _weight; }
            set {
                int newWeight = Math.Min(Math.Max(value, MIN_WEIGHT), MAX_WEIGHT);
                if (newWeight != _weight) {
                    int oldWeight = _weight;
                    _weight = newWeight;
                    if (WeightChanged != null) {
                        WeightChanged(oldWeight, newWeight);
                    }
                }
            }
        }
        public event Action<int, int> WeightChanged;
    }

    public ObservableList<BlendWeight> Weights;
}