using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MarkLight;
using MarkLight.Views.UI;

public class BlendShapeData {
    public string Path;
    public string Name;
    public bool IsWeight;
    public float Weight;
    public List<BlendShapeData> Sliders = new List<BlendShapeData>();
    public List<BlendShapeData> Buttons = new List<BlendShapeData>();
    
    public BlendShapeData(string path, string name, bool isWeight, float weight = 0f) {
        Path = path;
        Name = name;
        IsWeight = isWeight;
        Weight = weight;
    }
}

public class BlendShapeGroup : UIView {
    public BlendShapeData Data;
}