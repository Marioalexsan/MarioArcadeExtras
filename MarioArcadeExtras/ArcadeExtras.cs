using System;
using System.Collections.Generic;
using Bagmen;
using Microsoft.Xna.Framework;
using SoG;
using SoG.Modding;
using SoG.Modding.Configs;
using SoG.Modding.Utils;

namespace Murio
{
    public class ArcadeExtras : Mod
    {
        internal static ArcadeExtras TheMod { get; private set; }

        public static readonly ushort ShieldedEnemyPacket = 0;

        public static readonly ushort SpecialistEnemyPacket = 1;

        public static readonly ushort ChaosSpawnPacket = 2;

        private readonly List<EnemyCodex.EnemyTypes> shieldCurseBans;

        private readonly List<EnemyCodex.EnemyTypes> abilityCurseBans;

        public override string NameID => "MarioArcadeExtras";

        public override Version ModVersion => new Version("0.12");

        public ArcadeExtras()
        {
            shieldCurseBans = new List<EnemyCodex.EnemyTypes>()
            {
                EnemyCodex.EnemyTypes.Lood_Gold,
                EnemyCodex.EnemyTypes.Lood_Item,
                EnemyCodex.EnemyTypes.Lood_Health,
                EnemyCodex.EnemyTypes.Lood_TalentOrb,
                EnemyCodex.EnemyTypes.Lood_Pin,
                EnemyCodex.EnemyTypes.BeeHive,
                EnemyCodex.EnemyTypes.RiftCrystal,
                EnemyCodex.EnemyTypes.Special_VoodooDoll,
                EnemyCodex.EnemyTypes.MtBloom_CrystalBeetleRed,
                EnemyCodex.EnemyTypes.Chicken
            };

            abilityCurseBans = new List<EnemyCodex.EnemyTypes>()
            {
                EnemyCodex.EnemyTypes.Lood_Gold,
                EnemyCodex.EnemyTypes.Lood_Item,
                EnemyCodex.EnemyTypes.Lood_Health,
                EnemyCodex.EnemyTypes.Lood_TalentOrb,
                EnemyCodex.EnemyTypes.Lood_Pin,
                EnemyCodex.EnemyTypes.BeeHive,
                EnemyCodex.EnemyTypes.RiftCrystal,
                EnemyCodex.EnemyTypes.Special_VoodooDoll,
                EnemyCodex.EnemyTypes.MtBloom_CrystalBeetleRed,
                EnemyCodex.EnemyTypes.Chicken
            };
        }

        private Dictionary<string, RogueLikeMode.Perks> Perks = new Dictionary<string, RogueLikeMode.Perks>();

        private Dictionary<string, RogueLikeMode.TreatsCurses> Curses = new Dictionary<string, RogueLikeMode.TreatsCurses>();

        private Dictionary<string, SpellCodex.SpellTypes> Spells = new Dictionary<string, SpellCodex.SpellTypes>();

        private Dictionary<string, BaseStats.StatusEffectSource> StatusEffects = new Dictionary<string, BaseStats.StatusEffectSource>();

        private Dictionary<string, PinCodex.PinType> Pins = new Dictionary<string, PinCodex.PinType>();

        public RogueLikeMode.Perks GetPerkID(string modID)
        {
            if (Perks.ContainsKey(modID))
                return Perks[modID];

            return RogueLikeMode.Perks.None;
        }

        public RogueLikeMode.TreatsCurses GetCurseID(string modID)
        {
            if (Curses.ContainsKey(modID))
                return Curses[modID];

            return RogueLikeMode.TreatsCurses.None;
        }
        
        public SpellCodex.SpellTypes GetSpellID(string modID)
        {
            if (Spells.ContainsKey(modID))
                return Spells[modID];

            return SpellCodex.SpellTypes.NULL;
        }
        
        public BaseStats.StatusEffectSource GetStatusEffectID(string modID)
        {
            if (StatusEffects.ContainsKey(modID))
                return StatusEffects[modID];

            return BaseStats.StatusEffectSource.SlowLv1;
        }

        public PinCodex.PinType GetPinID(string modID)
        {
            if (Pins.ContainsKey(modID))
                return Pins[modID];

            return PinCodex.PinType.EmptySlot;
        }

        public override void Load()
        {
            TheMod = this;

            Logger.LogLevel = LogLevels.Debug;

            Perks["Perk01"] = CreatePerk(new PerkConfig("Perk01")
            {
                Name = "Soul Booster",
                Description = "Start the run with 10 more MaxEP. Cast again, and again, and again!",
                EssenceCost = 25,
                RunStartActivator = (player) =>
                {
                    player.xEntity.xBaseStats.iMaxEP += 10;
                    player.xEntity.xBaseStats._ichkBaseMaxEP += 10 * 2;
                },
                TexturePath = AssetPath + "RogueLike/Perks/SoulBooster"
            });

            Perks["Perk02"] = CreatePerk(new PerkConfig("Perk02")
            {
                Name = "Talented Start",
                Description = "Start the run with a pair of bal- I mean 2 bonus Talent Points.",
                EssenceCost = 50,
                RunStartActivator = (player) =>
                {
                    player.xViewStats.iTalentPoints += 2;
                },
                TexturePath = AssetPath + "RogueLike/Perks/TalentedStart"
            });

            Perks["Perk03"] = CreatePerk(new PerkConfig("Perk03")
            {
                Name = "Speedy Start",
                Description = "Start the run with a Swift Ring. Wait, why do mages get the most stats?!",
                EssenceCost = 30,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Ring01Yellow, 1);
                    Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Ring01Yellow, player, bSend: true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/SpeedyStart"
            });

            Perks["Perk04"] = CreatePerk(new PerkConfig("Perk04")
            {
                Name = "Arcane Start",
                Description = "Start the run with a Smart Ring. Or was it called Nuke 'n' Destroy Ring MK1?",
                EssenceCost = 30,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Ring01Blue, 1);
                    Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Ring01Blue, player, bSend: true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/ArcaneStart"
            });

            Perks["Perk05"] = CreatePerk(new PerkConfig("Perk05")
            {
                Name = "Brawny Start",
                Description = "Start the run with a Strong Ring. Also known as Big Bamage Ring.",
                EssenceCost = 30,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Ring01Red, 1);
                    Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Ring01Red, player, bSend: true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/BrawnyStart"
            });

            Perks["Perk06"] = CreatePerk(new PerkConfig("Perk06")
            {
                Name = "Lucky Start",
                Description = "Start the run with a Rabbit's Foot. Rabby not included!",
                EssenceCost = 40,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_RabbitsFoot, 1);
                    Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_RabbitsFoot, player, bSend: true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/LuckyStart"
            });

            Perks["Perk07"] = CreatePerk(new PerkConfig("Perk07")
            {
                Name = "Slimy Start",
                Description = "Start the run with a Slimy Ring. Still a better love story than Twilight...",
                EssenceCost = 40,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_SlimeRing, 1);
                    Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_SlimeRing, player, bSend: true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/SlimyStart"
            });

            Perks["Perk08"] = CreatePerk(new PerkConfig("Perk08")
            {
                Name = "Charged Start",
                Description = "Start the run with a Soul Amulet. You won't need Focus this time around!",
                EssenceCost = 40,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Amulet01Blue, 1);
                    Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Amulet01Blue, player, bSend: true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/ChargedStart"
            });

            Perks["Perk09"] = CreatePerk(new PerkConfig("Perk09")
            {
                Name = "Extra Medicine",
                Description = "Start the run with two Health Potions. Use them wisely!",
                EssenceCost = 50,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Consumable_HealthPotion, 2);
                    Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Consumable_HealthPotion, player, true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/ExtraMedicine"
            });

            Perks["Perk10"] = CreatePerk(new PerkConfig("Perk10")
            {
                Name = "Sniper Elite",
                Description = "Start the run with the Compound Bow. Snipe away!",
                EssenceCost = 55,
                RunStartActivator = (player) =>
                {
                    Globals.Game._Player_SwitchBow(player, 2, true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/SniperElite"
            });

            Perks["Perk11"] = CreatePerk(new PerkConfig("Perk11")
            {
                Name = "Rubick's Curse",
                Description = "Start the run with 40 more MaxEP, but your MaxHP is reduced to 50%, and you lose 30 EP Regen. Ready for a challenge?",
                EssenceCost = 50,
                RunStartActivator = (player) =>
                {
                    player.xEntity.xBaseStats.iEPRegenMultiplierInPCT -= 30;
                    player.xEntity.xBaseStats.iMaxEP += 40;
                    player.xEntity.xBaseStats._ichkBaseEP += 40 * 2;

                    int previous = player.xEntity.xBaseStats.iHP;
                    player.xEntity.xBaseStats.fMaxHPMultiplier -= 0.5f;
                    player.xEntity.xBaseStats.iHP = player.xEntity.xBaseStats.iMaxHP;
                    player.xEntity.xBaseStats._ichkHPBalance += player.xEntity.xBaseStats.iHP - previous;

                },
                TexturePath = AssetPath + "RogueLike/Perks/RubickCurse"
            });

            Perks["Perk12"] = CreatePerk(new PerkConfig("Perk12")
            {
                Name = "Pandemic Kid",
                Description = "Start the run with the Gas Mask, which is totally not going to make Mt. Bloom a cakewalk.",
                EssenceCost = 50,
                RunStartActivator = (player) =>
                {
                    player.xInventory.AddItem(ItemCodex.ItemTypes._Facegear_GasMask, 1);
                    Globals.Game._Item_Equipped(ItemCodex.ItemTypes._Facegear_GasMask, player, true);
                },
                TexturePath = AssetPath + "RogueLike/Perks/PandemicKid"
            });

            Curses["Curse01"] = CreateTreatOrCurse(new TreatCurseConfig("Curse01")
            {
                Name = "START Treaty",
                Description = "Most enemies spawn with a shield. Shields deactivate after a delay.",
                IsCurse = true,
                ScoreModifier = 0.25f,
                TexturePath = AssetPath + "RogueLike/Curses/STARTTreaty"
            });

            Curses["Curse02"] = CreateTreatOrCurse(new TreatCurseConfig("Curse02")
            {
                Name = "Specialists",
                Description = "Some enemies gain special abilities.",
                IsCurse = true,
                ScoreModifier = 0.20f,
                TexturePath = AssetPath + "RogueLike/Curses/Specialists"
            });
            
            Curses["Curse03"] = CreateTreatOrCurse(new TreatCurseConfig("Curse03")
            {
                Name = "Game Over Speedrun",
                Description = "Most enemies move and attack faster.",
                IsCurse = true,
                ScoreModifier = 0.30f,
                TexturePath = AssetPath + "RogueLike/Curses/HasteCurse"
            });

            Spells["Spell001"] = CreateSpell(new SpellConfig("Spell001")
            {
                Builder = ShieldedEnemySpell.SpellBuilder
            });

            Spells["Spell002"] = CreateSpell(new SpellConfig("Spell002")
            {
                Builder = HealAbilitySpell.SpellBuilder
            });

            Spells["Spell003"] = CreateSpell(new SpellConfig("Spell003")
            {
                Builder = DrainAbilitySpell.SpellBuilder
            });

            Spells["Spell004"] = CreateSpell(new SpellConfig("Spell004")
            {
                Builder = ChaosAbilitySpell.SpellBuilder
            });

            StatusEffects["DrainDebuff_ASPD"] = CreateStatusEffect(new StatusEffectConfig("DrainDebuff_ASPD"));

            StatusEffects["DrainDebuff_CSPD"] = CreateStatusEffect(new StatusEffectConfig("DrainDebuff_CSPD"));

            StatusEffects["DrainDebuff_EPReg"] = CreateStatusEffect(new StatusEffectConfig("DrainDebuff_EPReg"));

            Pins["Pin001"] = CreatePin(new PinConfig("Pin001")
            {
                Description = "Gain 150 bonus Shield HP with all shields.",
                PinSymbol = PinConfig.Symbol.Exclamation,
                PinShape = PinConfig.Shape.Circle,
                PinColor = PinConfig.Color.BilobaFlower,
                EquipAction = (PlayerView view) =>
                {
                    view.xEntity.xBaseStats.iShieldMaxHP += 150;
                },
                UnequipAction = (PlayerView view) =>
                {
                    view.xEntity.xBaseStats.iShieldMaxHP -= 150;
                }
            });

            CreateCommand("DropCustomPin", (_1, _2) =>
            {
                var pos = Globals.Game.xLocalPlayer.xEntity.xTransform.v2Pos;
                Globals.Game._EntityMaster_AddWatcher(new Watchers.PinSpawned(GetPinID("Pin001"), pos, pos));
            });
        }

        public override void Unload()
        {
            TheMod = null;
        }

        public override void PostArcadeRoomStart()
        {
            if (CanDoShieldCurseLogic())
            {
                DoShieldCurseForRoomStart();
            }

            if (CanDoSpecialistCurseLogic())
            {
                DoSpecialistCurseForRoomStart();
            }

            if (CanDoHasteCurseLogic())
            {
                foreach (var enemy in Globals.Game.dixEnemyList.Values)
                {
                    DoHasteCurseForEnemy(enemy);
                }
            }
        }

        public override void PostArcadeGauntletEnemySpawned(Enemy enemy)
        {
            if (CanDoShieldCurseLogic())
            {
                DoShieldCurseForGauntletEnemy(enemy);
            }

            if (CanDoSpecialistCurseLogic())
            {
                DoSpecialistCurseForGauntletEnemy(enemy);
            }

            if (CanDoHasteCurseLogic())
            {
                DoHasteCurseForEnemy(enemy);
            }
        }

        public override void PostEnemyKilled(Enemy enemy, AttackPhase killer)
        {
            if (CanDoShieldCurseLogic())
            {
                foreach (var spell in Globals.Game.xEntityMaster.dixActiveSpellInstances.Values)
                {
                    if (spell is ShieldedEnemySpell shield)
                    {
                        shield.OnKill();
                    }
                }
            }
        }

        private void DoShieldCurseForRoomStart()
        {
            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            List<Enemy> candidates = new List<Enemy>(Globals.Game.xEntityMaster.lxActiveEnemies);

            candidates.RemoveAll(x => shieldCurseBans.Contains(x.enType));

            bool inBossFight = session.xCurrentRoom.enRoomType == RogueLikeMode.Room.RoomTypes.Boss;

            int startDelay = inBossFight ? 480 : 180;
            int delayIncrease = inBossFight ? 150 : 120;
            int delayTier = 0;

            int shieldSkips = inBossFight ? 0 : Math.Max((int)(candidates.Count * 0.40f), 1);

            OpenGatesAtEnemyClear bagman = Globals.Game._EntityMaster_GetBagmanOfType<OpenGatesAtEnemyClear>();

            if (candidates.Count - shieldSkips > 0)
            {
                bagman.iParTime += (int)(startDelay * 0.65f);
            }

            foreach (Enemy enemy in candidates)
            {
                if (shieldSkips > 0)
                {
                    shieldSkips--;
                    continue;
                }

                int duration = startDelay + delayIncrease * delayTier;
                int killTimeReduction = 20;

                ShieldedEnemySpell spell = (ShieldedEnemySpell)Globals.Game._EntityMaster_AddSpellInstance(GetSpellID("Spell001"), enemy, Vector2.Zero, false, duration, killTimeReduction);
                
                spell.Init(null, duration, killTimeReduction);

                ///*
                Globals.Game._EntityMaster_AddWatcher(new DelayedCallbackWatcher(4, () =>
                {
                    Globals.Game._EntityMaster_InstantiateSpellOnClients(spell, true, spell.DurationLeft, spell.EnemyKillTimeReduction);
                }));
                //*/

                bagman.iParTime += (int)(delayIncrease * 0.5f + delayTier * 5);

                delayTier++;
            }
        }

        private void DoShieldCurseForGauntletEnemy(Enemy enemy)
        {
            int duration = 90 + (Globals.Game.randomInLogic.Next() % 45);
            int killTimeReduction = 0;

            ShieldedEnemySpell spell = (ShieldedEnemySpell)Globals.Game._EntityMaster_AddSpellInstance(GetSpellID("Spell001"), enemy, Vector2.Zero, false, duration, killTimeReduction);

            spell.Init(null, duration, killTimeReduction);

            ///*
            Globals.Game._EntityMaster_AddWatcher(new DelayedCallbackWatcher(4, () =>
            {
                Globals.Game._EntityMaster_InstantiateSpellOnClients(spell, true, spell.DurationLeft, spell.EnemyKillTimeReduction);
            }));
            //*/
        }

        private void DoSpecialistCurseForRoomStart()
        {
            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            List<Enemy> candidates = new List<Enemy>(Globals.Game.xEntityMaster.lxActiveEnemies);

            candidates.RemoveAll(x => abilityCurseBans.Contains(x.enType));

            int healSlots = 1 + session.iCurrentFloor / 4;
            int drainSlots = 1 + session.iCurrentFloor / 3;
            int chaosSlots = 1 + (session.iCurrentFloor + 2) / 4;

            int abilitySlots = 2 + (session.iCurrentFloor + 1) / 4;

            foreach (Enemy enemy in candidates)
            {
                int totalWeight = healSlots + drainSlots + chaosSlots;

                if (abilitySlots <= 0 || totalWeight <= 0)
                {
                    break;
                }

                abilitySlots--;

                if (Globals.Game.randomInLogic.NextDouble() < 0.15)
                {
                    continue; // 15% chance to consume a slot without granting ability
                }

                RollForSpecialistAndSpawn(enemy, ref healSlots, ref drainSlots, ref chaosSlots);
            }
        }

        private void DoSpecialistCurseForGauntletEnemy(Enemy enemy)
        {
            if (Globals.Game.randomInLogic.NextDouble() < 0.73 - 0.03 * Globals.Game.xGameSessionData.xRogueLikeSession.iCurrentFloor)
            {
                return; // 30% of enemies will have abilities. Increases by 3% per floor.
            }

            // Enemy will be spawned with an ability

            int healSlots = 10;
            int drainSlots = 12;
            int chaosSlots = 8;

            RollForSpecialistAndSpawn(enemy, ref healSlots, ref drainSlots, ref chaosSlots);
        }

        private void DoHasteCurseForEnemy(Enemy enemy)
        {
            List<EnemyCodex.EnemyTypes> moveSpeedBans = new List<EnemyCodex.EnemyTypes>()
            {
                EnemyCodex.EnemyTypes.Vilya
            };

            if (!moveSpeedBans.Contains(enemy.enType))
            {
                enemy.xBaseStats.AddPercentageMoveSpeedBuff(new BaseStats.BuffFloat(int.MaxValue, 1.25f));
            }

            foreach (var animation in enemy.xRenderComponent.dixAnimations.Values)
            {
                animation.fInnateTimeWarp *= 1.25f;
            }
        }

        private bool CanDoShieldCurseLogic()
        {
            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            var rogueData = Globals.Game.xGlobalData.xLocalRoguelikeData;

            bool coolRoom = session.xCurrentRoom.enRoomType == RogueLikeMode.Room.RoomTypes.Normal || session.xCurrentRoom.enRoomType == RogueLikeMode.Room.RoomTypes.Boss;

            return rogueData.IsTreatCurseEquipped(Curses["Curse01"]) &&
                NetUtils.IsLocalOrServer &&
                coolRoom &&
                !session.xCurrentRoom.bCompleted;
        }

        private bool CanDoSpecialistCurseLogic()
        {
            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            var rogueData = Globals.Game.xGlobalData.xLocalRoguelikeData;

            bool coolRoom = session.xCurrentRoom.enRoomType == RogueLikeMode.Room.RoomTypes.Normal;

            return rogueData.IsTreatCurseEquipped(Curses["Curse02"]) &&
                NetUtils.IsLocalOrServer &&
                coolRoom &&
                !session.xCurrentRoom.bCompleted;
        }
        
        private bool CanDoHasteCurseLogic()
        {
            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            var rogueData = Globals.Game.xGlobalData.xLocalRoguelikeData;

            // Non-player buffs have to be done on the client too

            return rogueData.IsTreatCurseEquipped(Curses["Curse03"]) &&
                !session.xCurrentRoom.bCompleted;
        }

        private void RollForSpecialistAndSpawn(Enemy enemy, ref int healSlots, ref int drainSlots, ref int chaosSlots)
        {
            OpenGatesAtEnemyClear bagman = Globals.Game._EntityMaster_GetBagmanOfType<OpenGatesAtEnemyClear>();

            int totalWeight = healSlots + drainSlots + chaosSlots;

            int next = Globals.Game.randomInLogic.Next(0, totalWeight);
            int edge = 0;

            edge += healSlots;

            if (healSlots > 0 && next < edge)
            {
                var spell = Globals.Game._EntityMaster_AddSpellInstance(GetSpellID("Spell002"), enemy, Vector2.Zero, false);

                spell.Init(null);

                ///*
                Globals.Game._EntityMaster_AddWatcher(new DelayedCallbackWatcher(4, () =>
                {
                    Globals.Game._EntityMaster_InstantiateSpellOnClients(spell, true);
                }));
                //*/

                bagman.iParTime += enemy.xBehaviour.bIsElite ? 420 : 200;

                healSlots--;

                return;
            }

            edge += drainSlots;

            if (drainSlots > 0 && next < edge)
            {
                var spell = Globals.Game._EntityMaster_AddSpellInstance(GetSpellID("Spell003"), enemy, Vector2.Zero, false);

                spell.Init(null);

                ///*
                Globals.Game._EntityMaster_AddWatcher(new DelayedCallbackWatcher(5, () =>
                {
                    Globals.Game._EntityMaster_InstantiateSpellOnClients(spell, true);
                }));
                //*/

                bagman.iParTime += enemy.xBehaviour.bIsElite ? 200 : 60;

                drainSlots--;

                return;
            }

            edge += chaosSlots;

            if (chaosSlots > 0 && next < edge)
            {
                var spell = Globals.Game._EntityMaster_AddSpellInstance(GetSpellID("Spell004"), enemy, Vector2.Zero, false);
                
                spell.Init(null);

                ///*
                Globals.Game._EntityMaster_AddWatcher(new DelayedCallbackWatcher(4, () =>
                {
                    Globals.Game._EntityMaster_InstantiateSpellOnClients(spell, true);
                }));
                //*/

                bagman.iParTime += enemy.xBehaviour.bIsElite ? 700 : 320;

                chaosSlots--;

                return;
            }
        }
    }
}
