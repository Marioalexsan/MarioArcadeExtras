using Microsoft.Xna.Framework;
using System;
using SoG;
using SoG.Modding.Core;
using SoG.Modding.ModUtils;
using SoG.Modding.API;

namespace Murio
{
    public class ChaosAbilitySpell : EnemyAbilitySpell
    {
        public int ChaosDuration { get; set; } = 360;

        public float SpawnCost { get; set; } = 40f;

        public int CalmDuration { get; set; } = 120;

        public override void OnDestroy()
        {
            AbilityIcon.bToBeDestroyed = true;
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

        public override void ClientUpdate()
        {
            Update();
        }

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
                if (_durationLeft <= 0)
                {
                    _inChaos = !_inChaos;

                    _durationLeft = _inChaos ? ChaosDuration : CalmDuration;
                }
                else
                {
                    if (_inChaos)
                    {
                        _points += Vector2.Distance(_lastPosition, Owner.xTransform.v2Pos) * 0.5f;
                        _points += 0.4f;
                    }
                    else
                    {
                        _points -= 0.08f;
                        _points = Math.Max(_points, 0);
                    }

                    _lastPosition = Owner.xTransform.v2Pos;
                    _durationLeft--;

                    if (_points >= SpawnCost)
                    {
                        _points -= SpawnCost;

                        if (NetUtils.IsLocalOrServer)
                        {
                            SpawnChaos(_lastPosition, Globals.Game.randomInLogic.Next(0, 2));
                        }
                    }

                    UpdateEffects();
                }
            }
        }

        public override void InstructionsFromServer(byte byInstructionID, params float[] afParams)
        {
            Vector2 position;

            switch (byInstructionID)
            {
                case 0:
                    position = new Vector2(afParams[0], afParams[1]);

                    SlowSlime slime = Globals.Game._EntityMaster_AddDynamicEnvironment(DynamicEnvironmentCodex.ObjectTypes.CloudEffect_BossPapaSlime_SlowSlimeSmall_Red, position) as SlowSlime;

                    slime.fMoveSpeedDebuff = afParams[2];
                    slime.iDeath = (int)afParams[3];
                    break;
                case 1:
                    position = new Vector2(afParams[0], afParams[1]);

                    SlipperySurface ice = Globals.Game._EntityMaster_AddDynamicEnvironment(DynamicEnvironmentCodex.ObjectTypes.CloudEffect_Ice, position) as SlipperySurface;

                    ice.iDeath = (int)afParams[2];
                    ice.colDebuffCollider = new SphereCollider(20f, Vector2.Zero, ice.xTransform, 0f, ice);
                    ice.colDebuffCollider.ibitLayers = Owner.xCollisionComponent.ibitCurrentColliderLayer;

                    Globals.Game._EntityMaster_AddWatcher(new Watchers.PeriodicalAnimationSwitch(Globals.Game._EffectMaster_AddEffect(new SortedAnimated(position, SortedAnimated.SortedAnimatedEffects._EnemyEffects_Winter_GroundFrostAbove_Medium)).xRenderComponent.AsAnimated(), 2, ice.iDeath));

                    break;
            }
        }

        private bool _foundEnemy = false;

        private ushort _enemyID = 0;

        private float _points = 0f;

        private Vector2 _lastPosition;

        private bool _inChaos = false;

        private int _durationLeft = 60;

        private void StartSpell()
        {
            CreateAbilityIcon(Color.Yellow);

            _lastPosition = Owner.xTransform.v2Pos;
        }

        private void UpdateEffects()
        {
            Color toUse = Color.Yellow;

            if (!_inChaos)
            {
                toUse.R = (byte)(toUse.R * 0.1f);
                toUse.G = (byte)(toUse.G * 0.1f);
                toUse.B = (byte)(toUse.B * 0.1f);
            }

            Color current = AbilityIcon.xRenderComponent.cColor;

            Color newColor = new Color()
            {
                R = (byte)(current.R + (toUse.R - current.R) * 0.1f),
                G = (byte)(current.G + (toUse.G - current.G) * 0.1f),
                B = (byte)(current.B + (toUse.B - current.B) * 0.1f),
                A = 255
            };

            AbilityIcon.xRenderComponent.cColor = newColor;
        }

        private void SpawnChaos(Vector2 position, int option)
        {
            switch (option)
            {
                case 0:
                    InstructionsFromServer(0, position.X, position.Y, 0.6f, 180);
                    SendClientInstruction(0, position.X, position.Y, 0.6f, 180);
                    break;
                case 1:
                    InstructionsFromServer(1, position.X, position.Y, 180);
                    SendClientInstruction(1, position.X, position.Y, 180);
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        public static ChaosAbilitySpell SpellBuilder(int powerLevel, Level.WorldRegion worldRegion)
        {
            ChaosAbilitySpell spell = new ChaosAbilitySpell();

            spell.bSendOwnerAsWorldActor = true;
            spell.bSurviveCutscenes = true;

            return spell;
        }
    }
}
