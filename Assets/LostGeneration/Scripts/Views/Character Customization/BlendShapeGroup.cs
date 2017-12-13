using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MarkLight;
using MarkLight.Views.UI;

public class BlendShapeGroup : UIView {
    public _string Path;
    public _string Name;
    public _bool IsWeight;
    public _float Weight;
    public ObservableList<BlendShapeGroup> Sliders = new ObservableList<BlendShapeGroup>();
    public ObservableList<BlendShapeGroup> Buttons = new ObservableList<BlendShapeGroup>();
    
    public BlendShapeGroup(string path, string name, bool isWeight, float weight = 0f) {
        Path = new _string() { Value = path };
        Name = new _string() { Value = name };
        IsWeight = new _bool() { Value = isWeight };
    }
}