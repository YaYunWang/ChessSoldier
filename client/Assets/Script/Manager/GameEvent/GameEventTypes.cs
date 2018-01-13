using UnityEngine;
using System.Collections;

public enum GameEventTypes
{
    // 无效
    Inavlid = 0,

    ChangeScene,        //切换场景
    ExitScene,          // 离开场景
	EnterScene,         // 进入场景

    BeginSkill,         //技能开始
    EndSkill,           //技能结束
}