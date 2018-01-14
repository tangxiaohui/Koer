using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Event
{
    public class EventType
    {
        /// <summary>
        /// 重置角色类型
        /// </summary>
        public const string ceshiEventType = "ceshiEventType";
        /// <summary>
        /// 技能造成的伤害数字回调
        /// </summary>
        public const string Battle_DamageHp = "Battle_DamageHp";
        /// <summary>
        ///英雄复活
        /// </summary>
        public const string Hero_Revive = "Hero_Revive";
        /// <summary>
        ///英雄血条更改
        /// </summary>
        public const string Hero_Hp= "Hero_Hp";
        /// <summary>
        /// 英雄等级更改
        /// </summary>
        public const string Hero_Level = "Hero_Level";
        /// <summary>
        /// 英雄金币更改
        /// </summary>
        public const string Hero_Gold = "Hero_Gold";
        /// <summary>
        /// 英雄VIP更改
        /// </summary>
        public const string Hero_Vip = "Hero_Vip";
        /// <summary>
        /// 英雄非绑定钻石
        /// </summary>
        public const string Hero_Unbounddiamonds = "Hero_Unbounddiamonds";
        /// <summary>
        /// 英雄非绑定钻石
        /// </summary>
        public const string Hero_Binddiamonds = "Hero_Binddiamonds";
        /// <summary>
        /// 战斗力
        /// </summary>
        public const string Hero_Attackleffect = "Hero_Attackleffect";
        /// <summary>
        ///英雄更换了模型
        /// </summary>
        public const string Hero_ChangeRole = "Hero_ChangeRole";
        /// <summary>
        ///英雄增加怒气值
        /// </summary>
        public const string Hero_AddRage = "Hero_AddRage";
        /// <summary>
        /// 触发新手引导
        /// </summary>
        public const string TriggerGuide = "TriggerGuide";
        /// <summary>
        /// 结束新手引导
        /// </summary>
        public const string ExitGuide = "ExitGuide";
        /// <summary>
        ///触发新手引导界面
        /// </summary>
        public const string GuideUI_Trigger = "GuideUI_Trigger";
        /// <summary>
        ///新手引导结束界面
        /// </summary>
        public const string GuideUI_End = "GuideUI_End";
        /// <summary>
        /// 重置副本
        /// </summary>
        public const string ResetDungeon = "ResetDungeon";
        /// <summary>
        /// 重置角色类型
        /// </summary>
        public const string ResetActorBase = "ResetActorBase";
        /// <summary>
        /// 充值返回
        /// </summary>
        public const string RechargeResult = "RechargeResult";
        /// <summary>
        /// 登录返回
        /// </summary>
        public const string LoginResult = "LoginResult";
        /// <summary>
        /// 使用技能
        /// </summary>
        public const string DoUseSkill = "DoUseSkill";
        /// <summary>
        /// 英雄死亡
        /// </summary>
        public const string HeroDead = "HeroDead";
        /// <summary>
        /// 选择图片成功
        /// </summary>
        public const string ChoosePhoto = "ChoosePhoto";
        /// <summary>
        /// 更改页面基本枚举
        /// </summary>
        public const string UIEnumEvent = "UIEnumEvent";
        /// <summary>
        /// 打开屏蔽页面
        /// </summary>
        public const string OpenShieldUI = "OpenShieldUI";
        /// <summary>
        ///  关闭屏蔽页面
        /// </summary>
        public const string CloseShieldUI = "CloseShieldUI";
        /// <summary>
        /// 触发剧情
        /// </summary>
        public const string StoryTrigger = "StoryTrigger";
        /// <summary>
        /// 结束剧情
        /// </summary>
        public const string StoryEnd = "StoryEnd";
        /// <summary>
        /// 打开剧情UI
        /// </summary>
        public const string OpenStoryUI = "OpenStoryUI";
        /// <summary>
        /// 关闭剧情UI
        /// </summary>
        public const string CloseStoryUI = "CloseStoryUI";
        /// <summary>
        /// 刷新怪物
        /// </summary>
        public const string RefreshMonster = "RefreshMonster";
        /// <summary>
        ///  验证Boss血量
        /// </summary>
        public const string VerificationBoss = "VerificationBoss";
        /// <summary>
        ///  切换副本了
        /// </summary>
        public const string ChangeDungeon = "ChangeDungeon";
        /// <summary>
        ///  PVP或好友切磋结束
        /// </summary>
        public const string ChallengeOpponentFinish = "ChallengeOpponentFinish";
        /// <summary>
        ///  对手玩家死亡
        /// </summary>
        public const string OpponentDie = "OpponentDie";
        #region Buff触发条件
        /// <summary>
        /// 闪避触发
        /// </summary>
        public const string EBuffTriggerType_Dodge_Rating = "EBuffTriggerType_Dodge_Rating";
        /// <summary>
        /// 暴击触发
        /// </summary>
        public const string EBuffTriggerType_Crit = "EBuffTriggerType_Crit";
        /// <summary>
        /// 生命低于万分比触发
        /// </summary>
        public const string EBuffTriggerType_Hp = "EBuffTriggerType_Hp";
        /// <summary>
        /// 受到致命伤触发
        /// </summary>
        public const string EBuffTriggerType_Dead = "EBuffTriggerType_Dead";
        /// <summary>
        /// 被攻击时触发
        /// </summary>
        public const string EBuffTriggerType_Hurt = "EBuffTriggerType_Hurt";
        /// <summary>
        /// 使用技能触发
        /// </summary>
        public const string EBuffTriggerType_UseSkill = "EBuffTriggerType_UseSkill";
        /// <summary>
        /// 攻击不良状态敌人触发
        /// </summary>
        public const string EBuffTriggerType_Atk = "EBuffTriggerType_Atk";
        /// <summary>
        /// 普通攻击时触发
        /// </summary>
        public const string EBuffTriggerType_CommonAkt = "EBuffTriggerType_CommonAkt";
        /// <summary>
        /// 真实伤害触发
        /// </summary>
        public const string EBuffTriggerType_RealDamage = "EBuffTriggerType_RealDamage";
        /// <summary>
        /// 攻击不良状态敌人触发
        /// </summary>
        public const string EBuffTriggerType_DeBuffAtk = "EBuffTriggerType_DeBuffAtk";
        /// <summary>
        /// 被暴击时候触发
        /// </summary>
        public const string EBuffTriggerType_HurtCrit = "EBuffTriggerType_HurtCrit";
        /// <summary>
        /// 重置ResetAnimationCurve
        /// </summary>
        public const string ResetAnimationCurve = "ResetAnimationCurve";
        /// <summary>
        /// 添加血条
        /// </summary>
        public const string AddHpUI = "AddHpUI";
        /// <summary>
        /// 移除血条
        /// </summary>
        public const string RemoveHpUI = "RemoveHpUI";
        #endregion
        public const string ChangeWorld = "ChangeWorld";
    }
}