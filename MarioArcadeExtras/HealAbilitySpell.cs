using Microsoft.Xna.Framework;
using SoG;
using SoG.Modding.Core;
using SoG.Modding.ModUtils;
using System;
using System.Collections.Generic;
using Watchers;
using SoG.Modding.API;

namespace Murio
{
    public class HealAbilitySpell : EnemyAbilitySpell
    {
        public int HealInterval { get; set; } = 180;

        // How much the owner gets healed by
        public float SelfHealStrength { get; set; } = 0.14f;

        public float BossHealMultiplier { get; set; } = 0.1f;

        public float AllyHealStrength { get; set; } = 0.18f;

        public float AllyHealRadius { get; set; } = 60;

        public SortedAnimated RadiusIndicator { get; private set; }

        public override void Init(InMessage msg, params float[] p_afInitFloats)
        {
            if (msg != null)
            {
                xSpellOwner = Globals.Game.EXT_ReadWorldActor(msg, out _, out var ID);
                _enemyID = (ushort)ID;
            }

            _foundEnemy = xSpellOwner != null;

            if (_foundEnemy)
            {
                StartSpell();
            }
        }

        public override void ClientUpdate() => Update();

        public override void Update()
        {
            if (!_foundEnemy && Globals.Game.dixEnemyList.ContainsKey(_enemyID))
            {
                xSpellOwner = Globals.Game.dixEnemyList[_enemyID];
                _foundEnemy = true;

                StartSpell();
            }

            if (!_foundEnemy)
            {
                return;
            }

            if (Owner.bToBeDestroyed || Owner.bDefeated)
            {
                bToBeDestroyed = true;
            }
            else
            {
                if (_nextHealIn <= 0)
                {
                    List<Enemy> nearbyEnemies = new List<Enemy>();

                    foreach (Enemy enemy in Globals.Game.xEntityMaster.lxActiveEnemies)
                    {
                        if (Vector2.Distance(enemy.xTransform.v2Pos, Owner.xTransform.v2Pos) <= AllyHealRadius)
                        {
                            nearbyEnemies.Add(enemy);
                        }
                    }

                    _nextHealIn = HealInterval;

                    foreach (Enemy enemy in nearbyEnemies)
                    {
                        float usedRatio = HealEnemy(enemy, enemy == Owner ? SelfHealStrength : AllyHealStrength);

                        _nextHealIn += (int)((enemy == Owner ? 240 : 40) * usedRatio) + 5;
                    }

                    _flashTotal = _nextHealIn;

                    _flashState = _flashTotal;
                }
                else
                {
                    _nextHealIn--;
                }

                _flashState = Math.Max(_flashState - 1, 0);

                UpdateEffects();
            }
        }

        public override void OnDestroy()
        {
            AbilityIcon.bToBeDestroyed = true;
            RadiusIndicator.bToBeDestroyed = true;
        }

        private bool _foundEnemy = false;

        private ushort _enemyID = 0;

        private int _nextHealIn = 120;

        private int _flashState = 0;

        private int _flashTotal = 90;

        private int _flashTransitionTotal => (int)(_flashTotal * 0.75f);

        private float HealEnemy(Enemy enemy, float healStrength)
        {
            if (enemy.xEnemyDescription.enCategory != EnemyDescription.Category.Regular)
                healStrength *= BossHealMultiplier;

            int wounds = enemy.xBaseStats.iMaxHP - enemy.xBaseStats.iHP;
            int heal = (int)(enemy.xBaseStats.iMaxHP * healStrength);

            int whatCanBeHealed = Math.Min(heal, wounds);

            if (NetUtils.IsLocalOrServer)
            {
                Globals.Game._Enemy_Heal(enemy, whatCanBeHealed);
            }

            float healRatioUsed = whatCanBeHealed / (float)heal;

            return healRatioUsed;
        }

        private void StartSpell()
        {
            CreateAbilityIcon(Color.Red);

            RadiusIndicator = new SortedAnimated(Vector2.Zero, SortedAnimated.SortedAnimatedEffects._SkillEffects_OneHand_SpiritSlash_AOE_Lv1);

            Globals.Game._EffectMaster_AddEffect(RadiusIndicator);

            RadiusIndicator.xRenderComponent.fVirtualHeight += Owner.xRenderComponent.fVirtualHeight;
            RadiusIndicator.xRenderComponent.xTransform = Owner.xTransform;
            RadiusIndicator.xTransform = Owner.xTransform;
        }

        private void UpdateEffects()
        {
            Color toUse = Color.Red;

            if (_flashState >= _flashTransitionTotal)
            {
                toUse.R = 0;
            }
            else if (_flashTransitionTotal != 0)
            {
                toUse.R = (byte)(toUse.R * (_flashTransitionTotal - _flashState) / _flashTransitionTotal);
            }

            AbilityIcon.xRenderComponent.cColor = toUse;

            RadiusIndicator.xRenderComponent.fAlpha = 0.18f;

            RadiusIndicator.xRenderComponent.fScale = (float)AllyHealRadius / 55f; // 55 is the Spirit Slash AOE texture's radius
        }

        public static HealAbilitySpell SpellBuilder(int powerLevel, Level.WorldRegion worldRegion)
        {
            HealAbilitySpell spell = new HealAbilitySpell();

            spell.bSendOwnerAsWorldActor = true;
            spell.bSurviveCutscenes = true;

            return spell;
        }
    }
}
