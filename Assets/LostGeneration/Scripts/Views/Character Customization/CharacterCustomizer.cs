using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MarkLight;
using MarkLight.Views.UI;

public class CharacterCustomizer : UIView {
    public BlendShapeGroup ActiveGroup;
    public CharacterBody Body;

    private Dictionary<string, BlendShapeGroup> _groups = new Dictionary<string, BlendShapeGroup>();
    
    public void ButtonClick() {
        Debug.Log("Clicked");
    }

    #region MonoBehaviour
    private void Awake() {
        NameTree nameTree = new NameTree('.', Body.BlendShapes.Keys);

        Stack<BlendShapeGroup> parentStack = new Stack<BlendShapeGroup>();
        ActiveGroup = new BlendShapeGroup("_", "", false);
        parentStack.Push(ActiveGroup);

        foreach (string path in nameTree) {
            string parent = nameTree.GetParent(path);
            string name = nameTree.GetName(path);
            bool isWeight = !nameTree.IsLeaf(path);
            
            BlendShapeGroup group = new BlendShapeGroup(path, name, isWeight);

            if (parentStack.Count > 0 && parent == nameTree.GetParent(parentStack.Peek().Path)) {
                parentStack.Pop();
            }

            if (isWeight) {
                parentStack.Peek().Sliders.Add(group);
            } else {
                parentStack.Peek().Buttons.Add(group);
                parentStack.Push(group);
            }
            _groups[path] = group;
        }
    }
    #endregion
}