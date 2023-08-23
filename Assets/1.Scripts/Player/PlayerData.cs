using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("-----기본정보-----")]
    [Header("체력")]
    public float health;
    [Header("속도")]
    public float speed;
    [Header("회전속도")]
    public float rotateSpeed;
    [Header("점프파워")]
    public float jumpPower;
    [Header("점프버튼유지시간")]
    public float jumpFlagTime;
    [Header("중력")]
    public float gravity;
    [Header("최대하강속도")]
    public float maxFallSpeed;
    [Header("비행 힘")]
    public float flyPower;
    [Header("비행 최대 높이")]
    public float flyMaxHeight;
    [Header("비행가능 시간")]
    public float flyTime;
    [Header("날개짓 딜레이(키다운)")]
    public float flyActionDelay;
    [Header("비행최대하강속도")]
    public float maxFlyFallSpeed;

    [Header("-----빨아들이기-----")]
    [Header("빨아들이는 힘")]
    public float suctionPower;
    [Header("빨아들이기 준비시간")]
    public float suctionDelay;
    [Header("뱉은 물건의 속도")]
    public float spitItemSpeed;

    [Header("-----피격-----")]
    [Header("밀려나는 힘")]
    public float hitPower;
    [Header("밀려나는 시간")]
    public float hitTime;
    [Header("무적시간")]
    public float hitDelay;
    [Header("피격무적깜빡임시간")]
    public float hitBlinkDelay;
    [Header("체력경고비율")]
    public float healthWarningRatio;
    [Header("체력경고깜빡임시간")]
    public float healthWarningBlinkDelay;
}