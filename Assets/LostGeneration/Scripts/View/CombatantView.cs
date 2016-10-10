﻿using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class CombatantView : MonoBehaviour {
    public bool AreMessagesFinished {
        get { return _messages.Count > 0; }
    }
    
    private Combatant _combatant;
    private BoardTheme _boardTheme;

    private Queue<MessageArgs> _messages = new Queue<MessageArgs>();
    private MessageArgs _currentMessage;

    private enum State {
        Idle, Moving, Hurt, Dead
    }
    private State _state;
    private Vector3 _moveStart, _moveEnd;
    private float _hurtTimer = 0f;
    
    public void Initialize(Combatant combatant, BoardTheme theme) {
        _combatant = combatant;
        _boardTheme = theme;
    }

    public void OnMessage(MessageArgs message) {
        if (message.Source == _combatant || message.Target == _combatant) {
            _messages.Enqueue(message);
        }
    }

    #region MonoBehaviour
    private void Update() {
        CheckState();
        switch (_state) {
            case State.Moving:
                MoveUpdate();
                break;
            case State.Hurt:
                HurtUpdate();
                break;
        }
    }
    #endregion MonoBehaviour

    #region PrivateMethods
    private bool CheckState() {
        bool stateChanged = false;
        if (_currentMessage == null && _messages.Count > 0) {
            State oldState = _state;
            _currentMessage = _messages.Dequeue();
            _state = State.Idle;

            if (_currentMessage.Source == _combatant) {
                MoveAction.Message move = _currentMessage as MoveAction.Message;
                DamageMessage damage = _currentMessage as DamageMessage;
                if (move != null) {
                    OnMove(move);
                } else if (damage != null) {
                    OnDamage(damage);
                }
            }

            stateChanged = oldState != _state;
        }

        return stateChanged;
    }

    private void OnMove(MoveAction.Message move) {
        _state = State.Moving;
        _moveStart = _boardTheme.PointToVector3(move.From);
        _moveEnd = _boardTheme.PointToVector3(move.To);

        transform.position = _moveStart;
    }

    private void MoveUpdate() {
        transform.position = Vector3.MoveTowards(transform.position, _moveEnd, 8f * Time.deltaTime);
        if (transform.position == _moveEnd) {
            _currentMessage = null;
        }
    }

    private void OnDamage(DamageMessage damage) {
        _state = State.Hurt;
        _hurtTimer = 1f;
    }

    private void HurtUpdate() {
        _hurtTimer -= Time.deltaTime;
        if (_hurtTimer <= 0f) {
            _currentMessage = null;
        }
    }
    #endregion PrivateMethods
}
