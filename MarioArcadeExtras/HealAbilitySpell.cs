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
        public int HealInterval { get; } = 150;

        public float WoundHeal { get; } = 0.05f;

        public float BaseFlatHeal { get; } = 23;

        public float FlatHealPerEnemyLevel { get; } = 6;

        public float AllyWoundHealPenalty { get; } = 0.75f;

        public float BossWoundHealPenalty { get; } = 0.15f;

        public float AllyHealRadius { get; } = 60;

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
                        HealEnemy(enemy);
                    }
                }
                else
                {
                    _nextHealIn--;
                }

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

        private void HealEnemy(Enemy enemy)
        {
            bool isSelf = enemy == Owner;

            float woundRatio = WoundHeal * (isSelf ? 1f : AllyWoundHealPenalty);

            float woundHeal = (int)((enemy.xBaseStats.iMaxHP - enemy.xBaseStats.iHP) * woundRatio);
            float flatHeal = BaseFlatHeal + FlatHealPerEnemyLevel * enemy.xEnemyDescription.iLevel * (isSelf ? 1f : AllyWoundHealPenalty);

            if (enemy.xEnemyDescription.enCategory != EnemyDescription.Category.Regular)
            {
                woundHeal *= BossWoundHealPenalty;
                flatHeal *= BossWoundHealPenalty;
            }

            if (NetUtils.IsLocalOrServer)
            {
                Globals.Game._Enemy_Heal(enemy, (int)(woundHeal + flatHeal));
            }
        }

        private void StartSpell()
        {
            CreateAbilityIcon();

            CreateRadiusIndicator();

            Owner.xBaseStats.bStunImmune = true;
            Owner.xBaseStats.bSlowImmunity = true;
            Owner.xBaseStats.enSize = BaseStats.BodySize.Titan;

            Owner.xBaseStats.SetDefaulKnockbackResistance(5);
        }

        private void UpdateEffects()
        {
            EditAbilityIcon(Color.Red, (20f + HealInterval - _nextHealIn) / HealInterval);

            EditRadiusIndicator(Color.Red, 0.15f, 55f);
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
