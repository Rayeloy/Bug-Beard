using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBox : MonoBehaviour {

    public Collider2D target;
    public static PlayerCameraBox instance;
    Collider2D box;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public Vector2 focusAreaSize;

    FocusArea focusArea;
    [HideInInspector]
    public float currentLookAheadX, smoothLookVelocityX;
    float targetLookAheadX;
    float lookAheadDirX;


    bool lookAheadStopped;

    void Awake()
    {
        instance = this;
        focusArea = new FocusArea(target.bounds, focusAreaSize);

    }

    public void KonoUpdate()
    {
        focusArea.Update(target.bounds);
        CameraMovement.instance.focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if ((PlayerMovement.instance.pmState==PlayerMovement.pmoveState.wLeft && focusArea.velocity.x<0) 
                || (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.wRight && focusArea.velocity.x > 0))
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
        //Debug.Log("DRAW GIZMOS");
    }
    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;
        float boxSmoothX1, boxSmoothX2, boxSmoothY1, boxSmoothY2;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);

            boxSmoothX1= boxSmoothX2= boxSmoothY1= boxSmoothY2 = 0;
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left = Mathf.SmoothDamp(left, left + shiftX,ref boxSmoothX1,0.01f);
            right = Mathf.SmoothDamp(right, right + shiftX, ref boxSmoothX2, 0.01f);
            // left + shiftX;right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top = Mathf.SmoothDamp(top, top + shiftY, ref boxSmoothY1, 0.01f);
            bottom = Mathf.SmoothDamp(bottom, bottom + shiftY, ref boxSmoothY2, 0.01f);
            //top += shiftY;bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

}
