using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AuntieController : MonoBehaviour
{
    public SkeletonAnimation auntieAnimation;

    public int auntieHp = 100;

    void Update()
    {
        CapHp();
    }

    // จำกัด Hp มากที่สุดที่ 100 
    void CapHp()
    {
        if(auntieHp > 100)
        {
            auntieHp = 100;
        }
    }

    // สำหรับใช้ไอเท็ม heal
    public void Heal(int heal)
    {
        auntieHp += heal; 
    }

    // สำหรับคำนวณดาเมจถ้าโดนปาใส่ที่ตัว โดยจะให้เล่นอนิเมชั่น ตอนโดนตีแล้วกลับไป loop อนิเมชั่น Idle
    public void normalHit()
    {
        auntieHp -= 10;
        auntieAnimation.AnimationState.SetAnimation(0,"Moody Friendly",false);
        auntieAnimation.AnimationState.AddAnimation(0, "Idle UnFriendly 1", true,0f);

    }

    public void AuntieWin()
    {
        auntieAnimation.AnimationState.SetAnimation(0, "Cheer Friendly", true); // เล่นอนิเมชั่นตอนชนะ
    }

    public void StartIdle()
    {
        auntieAnimation.AnimationState.SetAnimation(0, "Idle UnFriendly 1", true); // ใช้สำหรับเริ่มเกมใหม่ให้เล่นอนิเมชั่น idle
    }

    public void AuntieLose()
    {
        auntieAnimation.AnimationState.SetAnimation(0, "Idle UnFriendly 2", true); // เล่นอนิเมชั่นตอนแพ้
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
