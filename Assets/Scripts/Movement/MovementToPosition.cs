using System;
using UnityEngine;

namespace Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(MovementToPositionEvent))]
    [DisallowMultipleComponent]
    public class MovementToPosition : MonoBehaviour
    {
        private Rigidbody2D rigidbody2D;
        private MovementToPositionEvent movementToPositionEvent;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        }

        private void OnEnable()
        {
            movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
        }

        private void OnDisable()
        {
            movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        }

        
        private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
        {
            MoveRigidbody(movementToPositionArgs.movePosition, movementToPositionArgs.currentPosition,
                movementToPositionArgs.moveSpeed);
        }

        private void MoveRigidbody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
        {
            Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);
            rigidbody2D.MovePosition(rigidbody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
        }
    }
}