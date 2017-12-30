using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour {
    public static PlayerAnimations instance;
    [Tooltip("Sprites are in a prefixed order. Do not change it if you are not sure about it")]
    public SpriteRenderer SpriteRend;
    public Sprite[] PlayerSprites;
    public Vector2[] Proportions;
    public Vector2[] Offsets;
    Vector2 StandingProportions;
    Vector2 StandingPosition;

    bool damaged;
    public float MaxTimeDamagedAnim;
    private float TimeDamagedAnim;

    protected PlayerSlash.SlashState slashSt;
    private void Awake()
    {
        instance = this;
        PAnim = PlayerAnims.Standing;
        StandingProportions = SpriteRend.transform.localScale;
        StandingPosition = SpriteRend.transform.localPosition;
        damaged = false;
    }

    [HideInInspector]
    public PlayerAnims PAnim;
    public enum PlayerAnims
    {
        Standing=0,
        Runing=1,
        Jumping=2,
        Slashing=3,
        Damaged=4,
        Sliding=5,

    }
    private void Update()
    {
        Damaged();
        slashSt = PlayerSlash.instance.slashSt;
        if (slashSt == PlayerSlash.SlashState.slashing)
        {
            PAnim = PlayerAnims.Slashing;
        }
        else if (damaged)
        {
            PAnim = PlayerAnims.Damaged;
        }
        else if (PlayerMovement.instance.bouncing)
        {
            PAnim = PlayerAnims.Jumping;
        }
        else if (PlayerMovement.instance.bouncing)
        {
            PAnim = PlayerAnims.Jumping;
        }
        else if (slashSt == PlayerSlash.SlashState.crystal)
        {
            PAnim = PlayerAnims.Jumping;
        }
        else if (!PlayerMovement.instance.IsGrounded)
        {
            PAnim = PlayerAnims.Jumping;
        }
        else if (PlayerMovement.instance.stopPlayer)
        {
            PAnim = PlayerAnims.Standing;
        }
        else if (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.wLeft || PlayerMovement.instance.pmState == PlayerMovement.pmoveState.wRight)
        {
            PAnim = PlayerAnims.Runing;
        }
        else
        {
            PAnim = PlayerAnims.Standing;
        }

        if (PAnim != PlayerAnims.Slashing)
        {
            ResetSpriteRotation();
        }

        switch (PAnim)
        {
            case PlayerAnims.Standing:
                SpriteRend.sprite = PlayerSprites[0];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[0].x, StandingProportions.y * Proportions[0].y);
                break;
            case PlayerAnims.Runing:
                SpriteRend.sprite = PlayerSprites[1];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[1].x, StandingProportions.y * Proportions[1].y);
                break;
            case PlayerAnims.Jumping:
                SpriteRend.sprite = PlayerSprites[2];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[2].x, StandingProportions.y * Proportions[2].y);
                break;
            case PlayerAnims.Slashing:
                SpriteRend.sprite = PlayerSprites[3];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[3].x, StandingProportions.y * Proportions[3].y);
                float angle=AngleBetweenVectors(Vector2.left, PlayerSlash.instance.lastSlashDir);
                if (PlayerSlash.instance.lastSlashDir.x >= 0)
                {
                    SpriteRend.transform.localRotation = Quaternion.Euler(180, 0, -angle);
                }
                else
                {
                    SpriteRend.transform.localRotation = Quaternion.Euler(0, 0, angle);
                }

                break;
            case PlayerAnims.Damaged:
                SpriteRend.sprite = PlayerSprites[4];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[4].x, StandingProportions.y * Proportions[4].y);
                break;
            case PlayerAnims.Sliding:
                SpriteRend.sprite = PlayerSprites[5];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[5].x, StandingProportions.y * Proportions[5].y);
                break;
        }
    }

    public void StartDamaged()
    {
        damaged = true;
        TimeDamagedAnim = 0;
    }

    public void Damaged()
    {
        if (damaged)
        {
            TimeDamagedAnim += Time.deltaTime;
            if (TimeDamagedAnim >= MaxTimeDamagedAnim)
            {
                damaged = false;
            }
        }
    }

    public void SetPose()
    {
        switch (PAnim)
        {
            case PlayerAnims.Standing:
                SpriteRend.sprite = PlayerSprites[0];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[0].x, StandingProportions.y * Proportions[0].y);
                break;
            case PlayerAnims.Runing:
                SpriteRend.sprite = PlayerSprites[1];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[1].x, StandingProportions.y * Proportions[1].y);
                break;
            case PlayerAnims.Jumping:
                SpriteRend.sprite = PlayerSprites[2];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[2].x, StandingProportions.y * Proportions[2].y);
                break;
            case PlayerAnims.Slashing:
                SpriteRend.sprite = PlayerSprites[3];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[3].x, StandingProportions.y * Proportions[3].y);
                float angle = AngleBetweenVectors(Vector2.left, PlayerSlash.instance.lastSlashDir);
                if (PlayerSlash.instance.lastSlashDir.x >= 0)
                {
                    SpriteRend.transform.localRotation = Quaternion.Euler(180, 0, -angle);
                }
                else
                {
                    SpriteRend.transform.localRotation = Quaternion.Euler(0, 0, angle);
                }

                break;
            case PlayerAnims.Damaged:
                SpriteRend.sprite = PlayerSprites[4];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[4].x, StandingProportions.y * Proportions[4].y);
                break;
            case PlayerAnims.Sliding:
                SpriteRend.sprite = PlayerSprites[5];
                SpriteRend.transform.localScale = new Vector2(StandingProportions.x * Proportions[5].x, StandingProportions.y * Proportions[5].y);
                break;
        }
    }

    float AngleBetweenVectors(Vector2 A, Vector2 B)
    {
        float dotProduct = Vector2.Dot(B.normalized, A.normalized);
        float magnitudesProduct = B.normalized.magnitude * A.normalized.magnitude;
        float cos = dotProduct / magnitudesProduct;
        float angleDif = Mathf.Acos(cos) * Mathf.Rad2Deg;

        if (A.x<0 && B.y >= 0)
        {
            angleDif = -angleDif;
        }else if (A.x > 0 && B.y <= 0)
        {
            angleDif = -angleDif;
        }
        return angleDif;
    }

    public void ResetSpriteRotation()
    {
        if(PlayerMovement.instance.pmState==PlayerMovement.pmoveState.stopLeft || PlayerMovement.instance.pmState == PlayerMovement.pmoveState.wLeft)
        {
            SpriteRend.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            SpriteRend.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
}

