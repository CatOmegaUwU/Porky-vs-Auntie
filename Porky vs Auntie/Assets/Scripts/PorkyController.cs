using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PorkyController : MonoBehaviour
{
    public SkeletonAnimation porkyAnimation;

    public int porkHp = 100;

    void Update()
    {
        CapHp();
    }

    // สำหรับใช้ไอเท็ม heal
    public void Heal(int heal)
    {
        porkHp += heal;
    }

    // จำกัด Hp มากที่สุดที่ 100 
    void CapHp()
    {
        if(porkHp > 100)
        {
            porkHp = 100;
        }
    }

    // สำหรับคำนวณดาเมจถ้าโดนปาใส่ที่ตัว โดยจะให้เล่นอนิเมชั่น ตอนโดนตีแล้วกลับไป loop อนิเมชั่น Idle
    public void normalHit()
    {
        porkHp -= 10;
        porkyAnimation.AnimationState.SetAnimation(0, "Moody Friendly", false);
        porkyAnimation.AnimationState.AddAnimation(0, "Idle UnFriendly 1", true, 0f);

    }

    public void PorkyWin()
    {
        porkyAnimation.AnimationState.SetAnimation(0, "Cheer Friendly", true); // เล่นอนิเมชั่นตอนชนะ
    }

    public void StartIdle()
    {
        porkyAnimation.AnimationState.SetAnimation(0, "Idle UnFriendly 1", true); // ใช้สำหรับเริ่มเกมใหม่ให้เล่นอนิเมชั่น idle
    }

    public void PorkyLose()
    {
        porkyAnimation.AnimationState.SetAnimation(0, "Idle UnFriendly 2", true); // เล่นอนิเมชั่นตอนแพ้

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ThrowObject")
        {
            normalHit();
        }
        else if (collision.tag == "PowerThrowObject")
        {

        }
    }
}
