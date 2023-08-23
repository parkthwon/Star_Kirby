using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    PlayerData playerData;

    //현재 플레이어 정보
    float hp;
    public float HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp < 0f) hp = 0f;
            else if (hp > maxHP) hp = maxHP;
        }
    }

    float maxHP;

    Color curColor = Color.black;
    [SerializeField] CanvasGroup playerInfoCG;
    [SerializeField] Renderer[] renderers;
    [SerializeField] Material[] materials;

    //피격
    float hitDelay;
    float hitBlinkTime = 0f;
    Color blinkColor = new Color(0.5f, 0.5f, 0, 1);
    //피격UI
    [SerializeField] Slider mainHPSlider;
    [SerializeField] Slider subHPSlider;
    [SerializeField] Image subHPFillImage;
    float prevSliderValue;
    Color subUICurColor = Color.white;
    //Hit
    float subUIBlinkDelay;
    float subUIBlinkTime;
    Color subUIHitColor = new Color(1, 1, 0.4f, 1);
    //Heal
    float mainUIHealDelay;
    float mainUIHealTime;
    Color subUIHealColor = new Color(1, 0.9f, 0.9f, 1);

    //체력경고
    bool warning = false;
    float warningBlinkTime = 0f;
    Color warningColor = new Color(1, 0, 0, 1);

    void Update()
    {
        if (PlayerManager.Instance.IsDie) return;

        //무적시간
        if (hitDelay > 0f)
        {
            hitDelay -= Time.deltaTime;
            if (hitDelay < 0f)
            {
                hitDelay = 0f;
                subHPSlider.value = hp / maxHP;
                //emission으로 발광효과주기
                //foreach (var renderer in renderers)
                curColor = Color.black;
                foreach (var material in materials)
                    material.SetColor("_EmissionColor", Color.black);
            }
            else
            {
                //발광효과주기
                hitBlinkTime += Time.deltaTime;
                if (hitBlinkTime > playerData.hitBlinkDelay)
                {
                    hitBlinkTime = 0f;
                    if (curColor == Color.black)
                    {
                        curColor = blinkColor;
                        foreach (var material in materials)
                            material.SetColor("_EmissionColor", blinkColor);

                    }
                    else
                    {
                        curColor = Color.black;
                        foreach (var material in materials)
                            material.SetColor("_EmissionColor", Color.black);
                    }
                }

                //UI
                if (hitDelay < subUIBlinkDelay)
                {
                    subHPFillImage.color = subUIHitColor;
                    //게이지 감소
                    subHPSlider.value = Mathf.Lerp(prevSliderValue, hp / maxHP, 1 - (hitDelay / subUIBlinkDelay));
                }
                else
                {
                    subUIBlinkTime += Time.deltaTime;
                    if (subUIBlinkTime > playerData.hitBlinkDelay)
                    {
                        subUIBlinkTime = 0f;
                        if (subUICurColor == Color.white)
                        {
                            subUICurColor = subUIHitColor;
                            subHPFillImage.color = subUICurColor;
                        }
                        else
                        {
                            subUICurColor = Color.white;
                            subHPFillImage.color = subUICurColor;
                        }
                    }
                }
            }
        }
        else
        {
            //체력 경고
            if ((float)hp / maxHP <= playerData.healthWarningRatio)
            {
                warning = true;
                //발광효과주기
                warningBlinkTime += Time.deltaTime;
                if (curColor == Color.black)
                {
                    Color newColor = new Color(warningBlinkTime / playerData.healthWarningBlinkDelay, 0, 0);
                    foreach (var material in materials)
                        material.SetColor("_EmissionColor", newColor);
                }
                else
                {
                    Color newColor = new Color(1 - warningBlinkTime / playerData.healthWarningBlinkDelay, 0, 0);
                    foreach (var material in materials)
                        material.SetColor("_EmissionColor", newColor);
                }

                if (warningBlinkTime > playerData.healthWarningBlinkDelay)
                {
                    warningBlinkTime = 0f;
                    if (curColor == Color.black)
                        curColor = warningColor;
                    else
                        curColor = Color.black;
                }
            }
            else
            {
                if (warning)
                {
                    warning = false;
                    curColor = Color.black;
                    foreach (var material in materials)
                        material.SetColor("_EmissionColor", Color.black);
                }
            }
        }

        if (mainUIHealTime < mainUIHealDelay)
        {
            mainUIHealTime += Time.deltaTime;
            float mainUIWaitDelay = mainUIHealDelay * 0.5f;
            if (mainUIHealTime > mainUIWaitDelay)
                mainHPSlider.value = Mathf.Lerp(prevSliderValue, hp / maxHP, (mainUIHealTime - mainUIWaitDelay) / mainUIWaitDelay);
        }
    }

    public void Set(PlayerData playerData)
    {
        this.playerData = playerData;
        maxHP = playerData.health;
        if (PlayerIngameData.Instance.HP == 0) hp = maxHP;
        else
        {
            hp = PlayerIngameData.Instance.HP;
            mainHPSlider.value = hp / maxHP;
            subHPSlider.value = hp / maxHP;
        }
    }

    public void Heal(float heal)
    {
        mainUIHealDelay = playerData.hitDelay;
        mainUIHealTime = 0;
        prevSliderValue = mainHPSlider.value;
        if (heal == -1) HP += maxHP;
        else HP += heal;
        subHPFillImage.color = subUIHealColor;
        subHPSlider.value = hp / maxHP;
    }

    public void Hit(Vector3 hitDir, float damage, bool drop)
    {
        if (hitDelay > 0f) return; //무적상태
        hitDelay = playerData.hitDelay; //무적시간 설정
        subUIBlinkDelay = playerData.hitDelay * 0.5f;
        subUIBlinkTime = 0;
        prevSliderValue = mainHPSlider.value;
        HP -= damage;
        mainHPSlider.value = hp / maxHP;
        if (hp <= 0f)
        {
            Die();
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlaySFXOnce("KirbyDeath");
        }
        else
        {
            SoundManager.Instance.PlaySFXOnce("Damaged");
            PlayerManager.Instance.Hit(hitDir);
            //아이템 드롭
            if (drop)
                PlayerManager.Instance.UnChange(hitDir, true);
        }
    }

    void Die()
    {
        Time.timeScale = 0f;
        //몸 기본색으로 되돌리기
        foreach (var material in materials)
            material.SetColor("_EmissionColor", Color.black);

        //플레이어 게임오버
        PlayerManager.Instance.PlayerDie();

        //체력 UI 왼쪽으로 움직이면서 감추기
        playerInfoCG.DOFade(0f, 0.5f).SetDelay(0.5f).SetEase(Ease.Linear).SetUpdate(true);
        playerInfoCG.transform.DOLocalMoveX(playerInfoCG.transform.localPosition.x - 60, 0.5f).SetDelay(0.5f).SetEase(Ease.Linear).SetUpdate(true);
    }
}
