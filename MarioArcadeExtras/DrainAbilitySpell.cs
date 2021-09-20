using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoG;
using SoG.Modding.Core;
using SoG.Modding.ModUtils;
using Microsoft.Xna.Framework;
using SoG.Modding.API;

namespace Murio
{
    public class DrainAbilitySpell : EnemyAbilitySpell
    {
        public int FlatEPDrain { get; } = 25;

        public float RelativeEPDrain { get; } = 0.5f;

        public override void OnDestroy()
        {
            AbilityIcon.bToBeDestroyed = true;

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
        }

        private bool _foundEnemy = false;

        private ushort _enemyID = 0;

        private void StartSpell()
        {
            CreateAbilityIcon(Color.Blue);

            Owner.xBaseStats.AddPercentageMoveSpeedBuff(new BaseStats.BuffFloat(int.MaxValue, 1.15f));

            foreach (var animation in Owner.xRenderComponent.dixAnimations.Values)
            {
                animation.fInnateTimeWarp *= 1.25f;
            }
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
