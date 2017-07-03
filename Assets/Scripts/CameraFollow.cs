using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    private Collider targetCollider;
    private ICharacter targetScript;
    public Vector2 focusAreaSize;
    public float lookAheadDistX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public float verticalOffset;
    

    FocusArea focusArea;
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookaheadStopped;

    private void Start()
    {
        targetCollider = target.GetComponentInChildren<Collider>();
        targetScript = target.GetComponentInChildren<ICharacter>();
        focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);
    }

    private void LateUpdate()
    {
        focusArea.Update(targetCollider.bounds);

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if(focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if(Mathf.Sign(targetScript.getInput().x) == Mathf.Sign(focusArea.velocity.x) && targetScript.getInput().x != 0)
            {
                lookaheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDistX;
            }
            else
            {
                if (!lookaheadStopped)
                {
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistX - currentLookAheadX) / 4;
                    lookaheadStopped = true;
                }
            }
        }

        
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3) focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left+right)/2, (top + bottom) / 2);
        }
        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if(targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if(targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
