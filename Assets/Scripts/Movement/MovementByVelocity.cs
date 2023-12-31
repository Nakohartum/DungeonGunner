﻿using System;
using UnityEngine;

namespace Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(MovementByVelocityEvent))]
    [DisallowMultipleComponent]
    public class MovementByVelocity : MonoBehaviour
    {
        private Rigidbody2D rigidbody2D;
        private MovementByVelocityEvent movementByVelocityEvent;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        }

        private void OnEnable()
        {
            movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        }

        private void OnDisable()
        {
            movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        }

        private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
        {
            MoveRigidbody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
        }

        private void MoveRigidbody(Vector2 moveDirection, float moveSpeed)
        {
            rigidbody2D.velocity = moveDirection * moveSpeed;
        }
    }
}