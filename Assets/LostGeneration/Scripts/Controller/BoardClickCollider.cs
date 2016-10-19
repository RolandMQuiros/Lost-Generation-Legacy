using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using LostGen;

[RequireComponent(typeof(BoxCollider))]
public class BoardClickCollider : MonoBehaviour,
                                  IPointerClickHandler,
                                  IPointerDownHandler,
                                  IPointerUpHandler {
    [Serializable]
    public class ClickEvent : UnityEvent<PointerEventData> { }

    public ClickEvent Clicked;
    public ClickEvent ClickDown;
    public ClickEvent ClickUp;

    private BoardTheme _theme;
    private BoxCollider _box;

    public void Initialize(BoardTheme theme, int boardWidth, int boardHeight) {
        _theme = theme;

        Vector3 bottomRight = theme.PointToVector3(new Point(boardWidth + 1, boardHeight + 1));
        Vector3 center = theme.PointToVector3(new Point(boardWidth, boardHeight));

        transform.position = new Vector3(center.x / 2f, transform.position.y, center.z / 2f);

        _box.size = new Vector3(Mathf.Abs(bottomRight.x),
                                Mathf.Abs(bottomRight.y),
                                Mathf.Abs(bottomRight.z)); 
    }

    #region EventInterfaces

    public void OnPointerClick(PointerEventData eventData) {
        Clicked.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData) {
        ClickDown.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        ClickUp.Invoke(eventData);
    }

    #endregion EventInterfaces

    #region MonoBehaviour

    private void Awake() {
        _box = GetComponent<BoxCollider>();
    }

    #endregion MonoBehaviour
}
