using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SoG;
using SoG.Modding.Core;
using SoG.Modding.ModUtils;
using Watchers;
using SoG.Modding.API;

namespace Murio
{
    public class ShieldedEnemySpell : ISpellInstance
    {
        public Enemy Owner => xSpellOwner as Enemy;

        public int EnemyKillTimeReduction { get; set; } = 0;

        public int DurationLeft { get; private set; }

        public void OnKill()
        {
            DurationLeft -= EnemyKillTimeReduction;
        }

        public override void Init(InMessage msg, params float[] p_afInitFloats)
        {
            if (p_afInitFloats.Length > 1)
            {
                DurationLeft = (int)p_afInitFloats[0];
            }
            if (p_afInitFloats.Length > 2)
            {
                EnemyKillTimeReduction = (int)p_afInitFloats[1];
            }

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

        public override void ClientUpdate()
        {
            Update();
        }

        public override void Update()
        {
            if (xSpellOwner != null)
            {
                _foundEnemy = true;
            }

            if (!_foundEnemy && Globals.Game.dixEnemyList.ContainsKey(_enemyID))
            {
                xSpellOwner = Globals.Game.dixEnemyList[_enemyID];
                _foundEnemy = true;

                StartSpell();
            }

            if (DurationLeft-- <= 0)
            {
                bToBeDestroyed = true;
            }
            else if (_foundEnemy && NetUtils.IsLocalOrServer)
            {
                Owner.xBaseStats.bUltimateGuard = true;
            }
        }

        public override void OnDestroy()
        {
            if (NetUtils.IsLocalOrServer)
            {
                Owner.xBaseStats.bUltimateGuard = false;
            }

            if (_shieldEffect != null)
            {
                (_shieldEffect.xRenderComponent as AnimatedRenderComponent).SwitchAnimation(2, Animation.CancelOptions.IgnoreIfPlaying);
            }
        }

        private void StartSpell()
        {
            _shieldEffect = Globals.Game._EffectMaster_AddEffect(new SortedAnimated(Vector2.Zero, SortedAnimated.SortedAnimatedEffects._EnemyEffects_Crystal_ShieldEffect)) as SortedAnimated;

            _shieldEffect.xRenderComponent.v2OffsetRenderPos.Y = 0f - (0f - Owner.xEnemyDescription.v2ApproximateOffsetToMid.Y - 19f);

            Globals.Game._EntityMaster_AddWatcher(new TrackRenderOffsetWithEnemy(_shieldEffect.xRenderComponent, Owner));

            _shieldEffect.xRenderComponent.fVirtualHeight += Owner.xRenderComponent.fVirtualHeight;
            _shieldEffect.xRenderComponent.xTransform = Owner.xTransform;
            _shieldEffect.xTransform = Owner.xTransform;
        }

        private bool _foundEnemy = false;

        private ushort _enemyID = 0;

        private SortedAnimated _shieldEffect;

        public static ShieldedEnemySpell SpellBuilder(int powerLevel, Level.WorldRegion worldRegion)
        {
            ShieldedEnemySpell spell = new ShieldedEnemySpell();

            spell.bSendOwnerAsWorldActor = true;
            spell.bSurviveCutscenes = true;

            return spell;
        }
    }
}
