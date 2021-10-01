using Microsoft.Xna.Framework;
using System;
using SoG;
using SoG.Modding;
using SoG.Modding.Utils;

namespace Murio
{
    public class ChaosAbilitySpell : EnemyAbilitySpell
    {
        public int IcePhase { get; } = 240;

        public int SlimePhase { get; } = 240;

        public int BlindingGasPhase { get; } = 120;

        public float IceCost { get; } = 20f;

        public float SlimeCost { get; } = 30f;

        public float BlindingGasCost { get; } = 12f;

        public float SpawnCost { get; private set; } = 20f;

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
                    _chaosMode = (_chaosMode + 1) % 3;

                    switch (_chaosMode)
                    {
                        case 0:
                            _durationLeft = _startingDuration = SlimePhase;
                            SpawnCost = SlimeCost;
                            break;
                        case 1:
                            _durationLeft = _startingDuration = IcePhase;
                            SpawnCost = IceCost;
                            break;
                        case 2:
                            _durationLeft = _startingDuration = BlindingGasPhase;
                            SpawnCost = BlindingGasCost;
                            break;
                    }
                }
                else
                {
                    _points += Vector2.Distance(_lastPosition, Owner.xTransform.v2Pos) * 0.5f;
                    _points += 0.4f;

                    _lastPosition = Owner.xTransform.v2Pos;
                    _durationLeft--;

                    if (_points >= SpawnCost)
                    {
                        _points -= SpawnCost;

                        if (NetUtils.IsLocalOrServer)
                        {
                            SpawnChaos(_chaosMode);
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

        private int _chaosMode = -1;

        private int _durationLeft = 0;

        private int _startingDuration = 360;

        private void StartSpell()
        {
            CreateAbilityIcon();

            _lastPosition = Owner.xTransform.v2Pos;
        }

        private void UpdateEffects()
        {
            EditAbilityIcon(Color.Yellow, (20f + _startingDuration - _durationLeft) / _startingDuration);
        }

        private void SpawnChaos(int option)
        {
            Vector2 position = default;
            Random random = Globals.Game.randomInLogic;

            switch (option)
            {
                case 0:
                    position = Owner.xTransform.v2Pos;

                    InstructionsFromServer(0, position.X, position.Y, 0.6f, 240);
                    SendClientInstruction(0, position.X, position.Y, 0.6f, 240);
                    break;
                case 1:
                    position = Owner.xTransform.v2Pos + Utility.RandomizeVector2Direction(random) * (2f + 4f * (float)random.NextDouble());

                    InstructionsFromServer(1, position.X, position.Y, 90);
                    SendClientInstruction(1, position.X, position.Y, 90);
                    break;
                case 2:
                    Globals.Game._EntityMaster_AddWatcher(new DelayedCallbackWatcher(() =>
                    {
                        position = Owner.xTransform.v2Pos + Utility.RandomizeVector2Direction(random) * (4f + 6f * (float)random.NextDouble());

                        _EnemySpells_GasCloud blind = Globals.Game._EntityMaster_AddSpellInstance(SpellCodex.SpellTypes._EnemySkill_GasCloud_Blind, Owner, position, true, 0f, 0f, 480, position.X, position.Y) as _EnemySpells_GasCloud;

                        blind.iDestroyAt = 480;
                        blind.xAttackPhasePlayer.lxCurrentColliders[0].ibitLayers = Owner.xCollisionComponent.ibitCurrentColliderLayer;
                        blind.xRenderComponent.fVirtualHeight = xRenderComponent.fVirtualHeight;
                        blind.afCloudParameters = new float[2] { 30f, 200f };
                        blind.xAttackPhasePlayer.lxCurrentColliders[0].ibitLayers = Owner.xCollisionComponent.ibitCurrentColliderLayer;
                        blind.xRenderComponent.fVirtualHeight = xRenderComponent.fVirtualHeight + 4f;
                    }));

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
