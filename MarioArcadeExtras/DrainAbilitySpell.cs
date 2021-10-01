using SoG;
using Microsoft.Xna.Framework;
using SoG.Modding;
using SoG.Modding.Utils;

namespace Murio
{
    public class DrainAbilitySpell : EnemyAbilitySpell
    {
        public int FlatEPDrain { get; } = 10;

        public float RelativeEPDrain { get; } = 0.5f;

        public float DebuffRadius { get; } = 45;

        public override void OnDestroy()
        {
            AbilityIcon.bToBeDestroyed = true;

            RadiusIndicator.bToBeDestroyed = true;

            if (NetUtils.IsLocalOrServer)
            {
                foreach (var player in Globals.Game.dixPlayers.Values)
                {
                    BaseStats stats = player.xEntity.xBaseStats;

                    stats.RemoveEP(FlatEPDrain + (int)(RelativeEPDrain * stats.iEP));

                    _Spells_ChainLightningInstance.SimulateEffect(Owner.xTransform.v2Pos, player.xEntity.xTransform.v2Pos, true);
                }
            }
        }

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
                foreach (PlayerView player in Globals.Game.dixPlayers.Values)
                {
                    if (Vector2.Distance(player.xEntity.xTransform.v2Pos, Owner.xTransform.v2Pos) <= DebuffRadius)
                    {
                        BaseStats stats = player.xEntity.xBaseStats;

                        stats.AddStatusEffect(ArcadeExtras.TheMod.GetStatusEffectID("DrainDebuff_ASPD"), new BaseStats.EBuffFloat(10, -12, EquipmentInfo.StatEnum.ASPD, true));
                        stats.AddStatusEffect(ArcadeExtras.TheMod.GetStatusEffectID("DrainDebuff_CSPD"), new BaseStats.EBuffFloat(10, -24, EquipmentInfo.StatEnum.CSPD, true));
                        stats.AddStatusEffect(ArcadeExtras.TheMod.GetStatusEffectID("DrainDebuff_EPReg"), new BaseStats.EBuffFloat(10, -65, EquipmentInfo.StatEnum.EPRegen, true));
                        stats.AddPercentageMoveSpeedDeBuff(new BaseStats.BuffFloat(10, 0.7f));
                    }
                } 

                UpdateEffects();
            }
        }

        private bool _foundEnemy = false;

        private ushort _enemyID = 0;

        private void StartSpell()
        {
            CreateAbilityIcon();

            CreateRadiusIndicator();
        }

        private void UpdateEffects()
        {
            EditAbilityIcon(Color.Blue, 1f);

            EditRadiusIndicator(Color.Blue, 0.15f, DebuffRadius);
        }

        public static DrainAbilitySpell SpellBuilder(int powerLevel, Level.WorldRegion worldRegion)
        {
            DrainAbilitySpell spell = new DrainAbilitySpell();

            spell.bSendOwnerAsWorldActor = true;
            spell.bSurviveCutscenes = true;

            return spell;
        }
    }
}
