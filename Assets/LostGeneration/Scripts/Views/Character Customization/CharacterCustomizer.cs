using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MarkLight;
using MarkLight.Views.UI;

public class CharacterCustomizer : View {
    public BlendShapeGroup ActiveGroup;
    [SerializeField]private CharacterBody _body;
    private Dictionary<string, BlendShapeData> _groups = new Dictionary<string, BlendShapeData>();
    
    public void ButtonClick() {
        Debug.Log("Clicked");
    }

    public override void LayoutChanged() {
        Dictionary<string, BlendShapeData> data = new Dictionary<string, BlendShapeData>();
        NameTree nameTree = new NameTree('.', _body.BlendShapes.Keys);

        ActiveGroup.Data = new BlendShapeData("_", "_", false);
        data["_"] = ActiveGroup.Data;

        foreach (string path in nameTree) {
            string parentPath = nameTree.GetParent(path);
            if (parentPath.Length == 0) {
                parentPath = "_";
            }

            string name = nameTree.GetName(path);
            bool isWeight = nameTree.IsLeaf(path);
            BlendShapeData current = new BlendShapeData(path, name, isWeight);
            data[path] = current;

            BlendShapeData parent;
            if (data.TryGetValue(parentPath, out parent)) {
                if (isWeight) {
                    parent.Sliders.Add(current);
                } else {
                    parent.Buttons.Add(current);
                }
            }
        }
    }
}