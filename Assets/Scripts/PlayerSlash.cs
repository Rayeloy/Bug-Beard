using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlash : MonoBehaviour
{

    public bool stopOnGround;
    public bool gravityOn;
    public bool stopOnMove;

    private Vector3 mousePosition;
    public float InitialSpeed;
    public float cdTime;
    private float cd;
    public float MaxTimeSlashing;
    public new Camera camera;
    public Rigidbody2D myRB;
    Vector2 myPos;//my position
    public PlayerMovement playerM;

    [HideInInspector]
    public float timeSlashing;
    [HideInInspector]
    public SlashState slashSt;

    public enum SlashState
    {
        ready,
        slashing,
        cooldown,
        crystal
    }

    private void Awake()
    {
        slashSt = SlashState.ready;
        timeSlashing = 0;
    }

    private void FixedUpdate()
    {
        switch (slashSt)
        {
            case SlashState.slashing:
                timeSlashing += Time.deltaTime;
                if (timeSlashing >= MaxTimeSlashing)
                {
                    StopSlash();
                }
                break;
            case SlashState.cooldown:
                cd += Time.deltaTime;
                if (cd >= cdTime)
                {
                    slashSt = SlashState.ready;
                }
                break;


        }
    }


void Update()
{
    mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - camera.transform.position.z));
    if (Input.GetButtonDown("Slash") && (slashSt==SlashState.ready||slashSt==SlashState.crystal))
    {
            Debug.Log("SLASH!");
        slash();
    }

}

void slash()
{
    if (gravityOn)
        playerM.phase = PlayerMovement.jumpphase.normal;
    else
        playerM.phase = PlayerMovement.jumpphase.none;
    slashSt = SlashState.slashing;
    timeSlashing = 0;
    myPos = transform.position;
        Debug.Log("SLASHING STATE= "+slashSt);
    Debug.Log("myPos=" + myPos.x + ", " + myPos.y);
    //Vector2 vel = DecomposeSpeed(InitialSpeed, mousePosition, myPos);
    Vector2 vel = new Vector2((mousePosition.x - myPos.x), (mousePosition.y - myPos.y)).normalized;
    myRB.velocity = new Vector2(vel.x * InitialSpeed, vel.y * InitialSpeed); ;
    //myRB.velocity = new Vector2(80f, 20f);
    Debug.Log("mouse pos=" + mousePosition.x + ", " + mousePosition.y + "; vx=" + vel.x + ";vy=" + vel.y);
}

public void StopSlash()
{
    slashSt = SlashState.cooldown;
    timeSlashing = 0;
    playerM.phase = PlayerMovement.jumpphase.normal;
    cd = 0;
    Debug.Log("STOP SLASH");
}

private Vector2 DecomposeSpeed(float speed, Vector3 posB, Vector3 posA)
{
    float angle = Mathf.Atan2((posB.x - posA.x), (posB.y - posA.y)) * Mathf.Rad2Deg;
    return new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
}
}
