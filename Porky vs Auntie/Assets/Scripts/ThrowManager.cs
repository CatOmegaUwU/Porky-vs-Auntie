using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static GameManager;

public class ThrowManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Throw Setting")]
    public Transform auntieThrowPosition;
    public Transform porkyThrowPosition;
    public float maxThrowForce = 20f;
    public float minThrowForce = 1f;
    public float chargeSpeed = 8f;
    public float throwAngle = 60f;

    [Header("Game Elements")]
    public float timer = 30f;
    public float windForce;

    [Header("Character")]
    public PorkyController porkyCharacter;
    public AuntieController auntieCharacter;

    // public variable
    public float currentThrowForce;
    public bool isCharging;
    public bool doubleAttackActive = false;
    public bool isUsingItem = false;

    public bool porkyUsedPowerThrow = false;
    public bool porkyUsedDoubleAttack = false;
    public bool porkyUsedHeal = false;

    public bool auntieUsedPowerThrow = false;
    public bool auntieUsedDoubleAttack = false;
    public bool auntieUsedHeal = false;


    public void ThrowObject()
    {
        // กำหนดจุดที่จะโยน และเช็คว่าจะถ้าจะใช้ตำแหน่งโยนของใครจาก โดยใช้ตัวแปร isPlayer1Turn
        Transform throwOrigin = gameManager.isPlayer1Turn ? porkyThrowPosition : auntieThrowPosition;
        GameObject throwable;

        // เช็คสถานะการใช้ Power Thorw
        if (gameManager.isPlayer1Turn ? porkyUsedPowerThrow : auntieUsedPowerThrow)
        {
            throwable = Instantiate(gameManager.powerThrowPrefab, throwOrigin.position, Quaternion.identity);
            if (gameManager.isPlayer1Turn) porkyUsedPowerThrow = false;
            else auntieUsedPowerThrow = false;
        }
        else
        {
            if (gameManager.isPlayer1Turn) throwable = Instantiate(gameManager.porkyThrowablePrefab, throwOrigin.position, Quaternion.identity);
            else throwable = Instantiate(gameManager.auntieThrowablePrefab, throwOrigin.position, Quaternion.identity);
        }

        // คำนวณทิศทางการโยน
        float angle = throwAngle; // มุมโยน
        float radianAngle = angle * Mathf.Deg2Rad;

        // เช็คตำแหน่งที่จะโยนให้ถูกฝั่ง ถ้าค่า x ติดลบจะโยนไปทางซ้าย ถ้าค่า x เป็นบวกจะโยนไปทางขวา
        Vector2 throwDirection = gameManager.isPlayer1Turn
            ? new Vector2(-Mathf.Cos(radianAngle), Mathf.Sin(radianAngle))
            : new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

        // เพิ่มแรงลมให้ทิศทาง
        if (gameManager.isPlayer1Turn)
        {
            throwDirection.x -= windForce;
        }
        else
        {
            throwDirection.x += windForce;
        }

        //Debug.Log($"Throw Direction for {(gameManager.isPlayer1Turn ? "Player 1" : "Player 2")}: {throwDirection}");

        // เพิ่มแรงให้ของที่จะโยนออกไป
        Rigidbody2D rb = throwable.GetComponent<Rigidbody2D>();
        rb.AddForce(throwDirection * currentThrowForce, ForceMode2D.Impulse);
    }

    // function สำหรับเพิ่มแรงให้กับการโยนถ้ากดค้าง
    public void ChargeThrowForce()
    {
        // ค่าแรงเพิ่มตามระยะเวลาที่กด
        currentThrowForce += chargeSpeed * Time.deltaTime;

        if (currentThrowForce > maxThrowForce)
        {
            currentThrowForce = minThrowForce; // เมื่อชาร์จจนเกจเต็มจะวนกลับไปที่ค่าต่ำสุด
        }

        // อัพเดต slide ของค่าความแรงในการโยน
        if (gameManager.isPlayer1Turn) gameManager.porkyThrowSlider.value = currentThrowForce;
        else gameManager.auntieThorwSlider.value = currentThrowForce;
    }

    // สำหรับ reset ค่าที่โยนออกไปใน turn ก่อนหน้า
    public void ResetThrowForce()
    {
        currentThrowForce = minThrowForce;
        if (gameManager.isPlayer1Turn) gameManager.porkyThrowSlider.value = minThrowForce;
        else gameManager.auntieThorwSlider.value = minThrowForce;
    }

    // function การโยนสำหรับ Ai ในส่วนนี้ AI จะโยนสุ่มตำแหน่ง ยังไม่ใช่โดนผู้เล่น 100%
    public void PerformAITurn()
    {
        bool willMiss = ShouldAIMiss();
        float minSafeThrowForce = maxThrowForce * 0.4f; 
        float aiForce;

        if (willMiss)
        {
            aiForce = Random.Range(minSafeThrowForce, minSafeThrowForce + (maxThrowForce * 0.2f)); // ถ้าพลาดจะทำให้ค่าที่สุ่มมาได้น้อยมากจนปาชนเสา
        }
        else
        {
            aiForce = Random.Range(minSafeThrowForce + (maxThrowForce * 0.2f), maxThrowForce); // ถ้าไม่พลาดจะปาแรงขึ้นระดับหนึ่ง มีโอกาสโดนผู้เล่น แต่ยังไม่ 100%
        }

        // AI ใช้ไอเท็มตามความยาก
        UseAIItem();

        GameObject throwable = Instantiate(gameManager.auntieThrowablePrefab, auntieThrowPosition.position, Quaternion.identity);

        float angle = 60f;
        float radianAngle = angle * Mathf.Deg2Rad;
        Vector2 throwDirection = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

        throwDirection.x += windForce;

        Rigidbody2D rb = throwable.GetComponent<Rigidbody2D>();
        rb.AddForce(throwDirection * aiForce, ForceMode2D.Impulse);

        gameManager.EndTurn();
    }

    // เช็คตามระดับความยากว่า AI จะพลาดหรือไม่
    bool ShouldAIMiss()
    {
        float missChance = 0f;
        switch (gameManager.selectedDifficulty)
        {
            case AiDifficulty.Easy:
                missChance = 0.5f;
                break;
            case AiDifficulty.Normal:
                missChance = 0.35f;
                break;
            case AiDifficulty.Hard:
                missChance = 0.15f;
                break;
        }
        return Random.value < missChance;
    }

    // สำหรับ Ai เรียกใช้ item โดยจะเช็คจากระดับความยาก ถ้า easy จะไม่ใช้เลย ถ้า normal จะใช้แค่ Heal ถ้า Hard จะใช้ทุกอย่าง
    void UseAIItem()
    {

        if (gameManager.selectedDifficulty == AiDifficulty.Easy) return;

        if (gameManager.selectedDifficulty == AiDifficulty.Normal && !auntieUsedHeal && auntieCharacter.auntieHp < 50)
        {
            UseItem("Heal");
            auntieUsedHeal = true;
        }
        else if (gameManager.selectedDifficulty == AiDifficulty.Hard)
        {
            if (!auntieUsedHeal && auntieCharacter.auntieHp < 50)
            {
                UseItem("Heal");
                auntieUsedHeal = true;
            }
            else if (!auntieUsedPowerThrow)
            {
                UseItem("PowerThrow");
                auntieUsedPowerThrow = true;
            }
            else if (!auntieUsedDoubleAttack)
            {
                UseItem("DoubleAttack");
                auntieUsedDoubleAttack = true;
            }
        }
    }

    // function สำหรับเรียกใช้ Item ของผู้เล่น
    public void UseItem(string itemName)
    {
        isUsingItem = true;

        if (itemName == "PowerThrow")
        {
            if (gameManager.isPlayer1Turn && !porkyUsedPowerThrow)
            {
                porkyUsedPowerThrow = true;
                Debug.Log("Player 1 used PowerThrow!");
            }
            else if (!gameManager.isPlayer1Turn && !auntieUsedPowerThrow)
            {
                auntieUsedPowerThrow = true;
                Debug.Log("Player 2 used PowerThrow!");
            }
        }
        else if (itemName == "DoubleAttack")
        {
            if (gameManager.isPlayer1Turn && !porkyUsedDoubleAttack)
            {
                doubleAttackActive = true;
                porkyUsedDoubleAttack = true;
                Debug.Log("Player 1 used DoubleAttack!");
            }
            else if (!gameManager.isPlayer1Turn && !auntieUsedDoubleAttack)
            {
                doubleAttackActive = true;
                auntieUsedDoubleAttack = true;
                Debug.Log("Player 2 used DoubleAttack!");
            }
        }
        else if (itemName == "Heal")
        {
            if ((gameManager.isPlayer1Turn && !porkyUsedHeal) || (!gameManager.isPlayer1Turn && !auntieUsedHeal))
            {
                porkyCharacter.Heal(50);
                porkyUsedHeal = true;
            }
            else
            {
                auntieCharacter.Heal(50);
                auntieUsedHeal = true;
            }
            Debug.Log($"{(gameManager.isPlayer1Turn ? "Player 1" : "Player 2")} used Heal!");
        }

        isUsingItem = false;
    }


}
