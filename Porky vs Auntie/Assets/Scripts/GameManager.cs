using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;


public class GameManager : MonoBehaviour
{
    // enum สำหรับโหมดเกมและระดับของ AI 
    public enum GameMode { PlayerVsPlayer, PlayerVsAI }
    public enum AiDifficulty { Easy, Normal, Hard }

    [Header("Player and Manager")]
    public ThrowManager throwManager;
    public PorkyController porkyCharacter;
    public AuntieController auntieCharacter;

    [Header("Game Setting")]
    public GameMode selectedGameMode = GameMode.PlayerVsAI;
    public AiDifficulty selectedDifficulty = AiDifficulty.Easy;

    [Header("Ui Element")]
    public TMP_Text timerText;
    public Slider windSlider;
    public Slider porkyThrowSlider;
    public Slider auntieThorwSlider;
    public Slider porkyHpBar;
    public Slider auntieHpBar;
    public Button porkyPowerThrowButton;
    public Button porkyDoubleAttackButton;
    public Button porkyHealButton;
    public Button auntiePowerThrowButton;
    public Button auntieDoubleAttackButton;
    public Button auntieHealButton;

    [Header("Game Object")]
    public GameObject porkyThrowablePrefab;
    public GameObject auntieThrowablePrefab;
    public GameObject powerThrowPrefab;
    public GameObject porkyWinpanel;
    public GameObject auntieWinpanel;

    public bool isPlayer1Turn = true; // เช็คว่าตอนนี้ใช่ตาของ Player1 หรือ Porky ไหม
    public bool isGameStart = false; // เช็คว่าตอนนี้เกมเริ่มขึ้นแล้วหรือยัง

    void Start()
    {
        //กำหนดค่า Slider ใน UI 
        porkyThrowSlider.minValue = throwManager.minThrowForce;
        porkyThrowSlider.maxValue = throwManager.maxThrowForce;
        porkyThrowSlider.value = throwManager.minThrowForce;

        auntieThorwSlider.minValue = throwManager.minThrowForce;
        auntieThorwSlider.maxValue = throwManager.maxThrowForce;
        auntieThorwSlider.value = throwManager.minThrowForce;

        porkyHpBar.maxValue = porkyCharacter.porkHp;
        auntieHpBar.maxValue = auntieCharacter.auntieHp;

        // กำหนดให้ค่า force ในการโยน เท่ากับ ค่า Force ตำสุดที่จะโยนเพื่อให้เริ่มต้นโยนจากเบาสุด
        throwManager.currentThrowForce = throwManager.minThrowForce;
    }
    void Update()
    {
        UpdateSliderHP();

        //เช็คว่าเกมเริ่มหรือยัง ถ้าเริ่มแล้วถึงจะคุมตัวละครได้
        if (isGameStart)
        {

            if (selectedGameMode == GameMode.PlayerVsPlayer || (selectedGameMode == GameMode.PlayerVsAI && isPlayer1Turn))
            {
                HandlePlayerInput();
            }

            GameEnd();
        }
    }
    
    // function ในการควบคุม Input 
    void HandlePlayerInput()
    {
        if (throwManager.isUsingItem) return;

        // ตรวจสอบการกดปุ่มเพื่อเริ่มโยน โดยเช็คจาก mouse และ กดหน้าจอค้าง
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            throwManager.isCharging = true;
        }

        // ตรวจสอบว่ามีการกดค้างหรือไม่ เพื่อชาร์จพลังในการโยน
        if (Input.GetMouseButtonUp(0) && (throwManager.isCharging || Input.touchCount > 0))
        {
            throwManager.ThrowObject();
            throwManager.isCharging = false;
            throwManager.ResetThrowForce();
            if (throwManager.doubleAttackActive)
            {
                throwManager.Invoke("ThrowObject", 0.5f); // โยนอีกครั้งหลังจาก 0.5 วินาที
                throwManager.doubleAttackActive = false; // ปิดสถานะการใช้ Double Attack
            }
            EndTurn();
        }

        // เพิ่มความแรงเการโยนมื่อกดค้าง
        if (throwManager.isCharging)
        {
            throwManager.ChargeThrowForce();
        }
    }

    // สำหรับจบ turn โดยเช็คจาก Enum ว่าจะเป็นผู้เล่นหรือ Ai ที่จะเริ่ม turn
    public void EndTurn()
    {
        isPlayer1Turn = !isPlayer1Turn;

        if (!isPlayer1Turn && selectedGameMode == GameMode.PlayerVsAI)
        {
            throwManager.Invoke("PerformAITurn", 1f);
        }

        UpdateWind();
    }

    // function สำหรับเช็คว่าเกมจบแล้วหรือไม่ โดยเช็คจาก HP ของทั้งสองตัวละคร
    void GameEnd()
    {
        if (porkyCharacter.porkHp <= 0)
        {
            isGameStart = false;
            auntieCharacter.AuntieWin();
            porkyCharacter.PorkyLose();
            auntieWinpanel.SetActive(true);

        }
        else if (auntieCharacter.auntieHp <= 0)
        {
            isGameStart = false;
            porkyCharacter.PorkyWin();
            auntieCharacter.AuntieLose();
            porkyWinpanel.SetActive(true);
        }
    }

    // function สำหรับ reset เกม โดยจะเรียกจากปุ่ม เพื่อให้เกมถูก reset สำหรับเล่นได้อีกครั้ง
    public void ResetGame()
    {
        porkyCharacter.porkHp = 100;
        auntieCharacter.auntieHp = 100;
        throwManager.windForce = 0f;
        throwManager.porkyUsedPowerThrow = false;
        throwManager.porkyUsedDoubleAttack = false;
        throwManager.porkyUsedHeal = false;
        throwManager.auntieUsedPowerThrow = false;
        throwManager.auntieUsedDoubleAttack = false;
        throwManager.auntieUsedHeal = false;
    }
    
    // function สำหรับสุ่มค่าแรงลม
    void UpdateWind()
    {
        throwManager.windForce = Random.Range(0f, 1.5f);
        windSlider.value = throwManager.windForce;
        //Debug.Log($"Wind Value: {throwManager.windForce}");
    }

    void UpdateSliderHP()
    {
        porkyHpBar.value = porkyCharacter.porkHp;
        auntieHpBar.value = auntieCharacter.auntieHp;
    }

    // Functions ปุ่มสำหรับเลือก Mode
    public void SetModeToPlayerVsPlayer()
    {
        selectedGameMode = GameMode.PlayerVsPlayer;
        isGameStart = true;
    }

    public void SetModeToPlayerVsAI()
    {
        selectedGameMode = GameMode.PlayerVsAI;
    }

    // Functions ปุ่มสำหรับเลือกระดับ Ai
    public void SetDifficultyToEasy()
    {
        selectedDifficulty = AiDifficulty.Easy;
        isGameStart = true;
    }

    public void SetDifficultyToNormal()
    {
        selectedDifficulty = AiDifficulty.Normal;
        isGameStart = true;
    }

    public void SetDifficultyToHard()
    {
        selectedDifficulty = AiDifficulty.Hard;
        isGameStart = true;
    }

    // Functions ปุ่มสำหรับไอเท็ม
    public void PorkyUsePowerThrow()
    {
        if (!isPlayer1Turn) { return; }
        throwManager.UseItem("PowerThrow");
        porkyPowerThrowButton.gameObject.SetActive(false);
    }

    public void PorkyUseDoubleAttack()
    {
        if(!isPlayer1Turn) { return; }
        throwManager.UseItem("DoubleAttack");
        porkyDoubleAttackButton.gameObject.SetActive(false);
    }

    public void PorkyUseHeal()
    {
        if (!isPlayer1Turn) { return; }
        throwManager.UseItem("Heal");
        porkyHealButton.gameObject.SetActive(false);
    }

    public void AuntieUsePowerThrow()
    {
        if (isPlayer1Turn) { return; }
        throwManager.UseItem("PowerThrow");
        auntiePowerThrowButton.gameObject.SetActive(false);
    }

    public void AuntieUseDoubleAttack()
    {
        if (isPlayer1Turn) { return; }
        throwManager.UseItem("DoubleAttack");
        auntieDoubleAttackButton.gameObject.SetActive(false);
    }

    public void AuntieUseHeal()
    {
        if (isPlayer1Turn) { return; }
        throwManager.UseItem("Heal");
        auntieHealButton.gameObject.SetActive(false);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
