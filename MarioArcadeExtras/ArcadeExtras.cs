using System;
using System.Collections.Generic;
using Bagmen;
using Microsoft.Xna.Framework;
using SoG;
using SoG.Modding;
using SoG.Modding.Addons;
using SoG.Modding.Content;
using SoG.Modding.Utils;
using SoG.Modding.Extensions;

namespace Murio
{
    [ModDependency("Addons.ModGoodies")]
    public class ArcadeExtras : Mod
    {
        internal static ArcadeExtras TheMod { get; private set; }

        public static readonly ushort ShieldedEnemyPacket = 0;

        public static readonly ushort SpecialistEnemyPacket = 1;

        public static readonly ushort ChaosSpawnPacket = 2;

        private readonly List<EnemyCodex.EnemyTypes> shieldCurseBans;

        private readonly List<EnemyCodex.EnemyTypes> abilityCurseBans;

        public override string NameID => "MarioArcadeExtras";

        public override Version ModVersion => new Version("0.16");

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

        public override void Load()
        {
            TheMod = this;

            Logger.LogLevel = LogLevels.Debug;

            SetupPerks();

            SetupCurses();

            SetupPins();

            SpellEntry spell;

            spell = CreateSpell("Spell001");

            spell.Builder = ShieldedEnemySpell.SpellBuilder;

            spell = CreateSpell("Spell002");

            spell.Builder = HealAbilitySpell.SpellBuilder;

            spell = CreateSpell("Spell003");

            spell.Builder = DrainAbilitySpell.SpellBuilder;

            spell = CreateSpell("Spell004");

            spell.Builder = ChaosAbilitySpell.SpellBuilder;

            StatusEffectEntry status;

            status = CreateStatusEffect("DrainDebuff_ASPD");
            status = CreateStatusEffect("DrainDebuff_CSPD");
            status = CreateStatusEffect("DrainDebuff_EPReg");

            CommandEntry commands = CreateCommands();

            commands.SetCommand("DropCustomPin", (_1, _2) =>
            {
                var pos = Globals.Game.xLocalPlayer.xEntity.xTransform.v2Pos;
                Globals.Game._EntityMaster_AddWatcher(new Watchers.PinSpawned(GetPin("Pin001").GameID, pos, pos));
            });
        }

        private void SetupPerks()
        {
            PerkEntry perk;

            perk = CreatePerk("Perk01");

            perk.Name = "Soul Booster";
            perk.Description = "Start the run with 10 more MaxEP. Cast again, and again, and again!";
            perk.EssenceCost = 25;
            perk.RunStartActivator = (player) =>
            {
                player.xEntity.xBaseStats.iMaxEP += 10;
                player.xEntity.xBaseStats._ichkBaseMaxEP += 10 * 2;
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/SoulBooster";

            perk = CreatePerk("Perk02");

            perk.Name = "Talented Start";
            perk.Description = "Start the run with a pair of bal- I mean 2 bonus Talent Points.";
            perk.EssenceCost = 50;
            perk.RunStartActivator = (player) =>
            {
                player.xViewStats.iTalentPoints += 2;
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/TalentedStart";

            perk = CreatePerk("Perk03");

            perk.Name = "Speedy Start";
            perk.Description = "Start the run with a Swift Ring. Wait, why do mages get the most stats?!";
            perk.EssenceCost = 30;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Ring01Yellow, 1);
                Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Ring01Yellow, player, bSend: true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/SpeedyStart";

            perk = CreatePerk("Perk04");

            perk.Name = "Arcane Start";
            perk.Description = "Start the run with a Smart Ring. Or was it called Nuke 'n' Destroy Ring MK1?";
            perk.EssenceCost = 30;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Ring01Blue, 1);
                Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Ring01Blue, player, bSend: true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/ArcaneStart";

            perk = CreatePerk("Perk05");

            perk.Name = "Brawny Start";
            perk.Description = "Start the run with a Strong Ring. Also known as Big Bamage Ring.";
            perk.EssenceCost = 30;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Ring01Red, 1);
                Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Ring01Red, player, bSend: true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/BrawnyStart";

            perk = CreatePerk("Perk06");

            perk.Name = "Lucky Start";
            perk.Description = "Start the run with a Rabbit's Foot. Rabby not included!";
            perk.EssenceCost = 40;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_RabbitsFoot, 1);
                Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_RabbitsFoot, player, bSend: true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/LuckyStart";

            perk = CreatePerk("Perk07");

            perk.Name = "Slimy Start";
            perk.Description = "Start the run with a Slimy Ring. Still a better love story than Twilight...";
            perk.EssenceCost = 40;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_SlimeRing, 1);
                Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_SlimeRing, player, bSend: true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/SlimyStart";

            perk = CreatePerk("Perk08");

            perk.Name = "Charged Start";
            perk.Description = "Start the run with a Soul Amulet. You won't need Focus this time around!";
            perk.EssenceCost = 40;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Accessory_Amulet01Blue, 1);
                Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Accessory_Amulet01Blue, player, bSend: true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/ChargedStart";

            perk = CreatePerk("Perk09");

            perk.Name = "Extra Medicine";
            perk.Description = "Start the run with two Health Potions. Use them wisely!";
            perk.EssenceCost = 50;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Consumable_HealthPotion, 2);
                Globals.Game._Item_AutoEquip(ItemCodex.ItemTypes._Consumable_HealthPotion, player, true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/ExtraMedicine";

            perk = CreatePerk("Perk10");

            perk.Name = "Sniper Elite";
            perk.Description = "Start the run with the Compound Bow. Snipe away!";
            perk.EssenceCost = 55;
            perk.RunStartActivator = (player) =>
            {
                Globals.Game._Player_SwitchBow(player, 2, true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/SniperElite";

            perk = CreatePerk("Perk11");

            perk.Name = "Rubick's Curse";
            perk.Description = "Start the run with 40 more MaxEP, but your MaxHP is reduced to 50%, and you lose 30 EP Regen. Ready for a challenge?";
            perk.EssenceCost = 50;
            perk.RunStartActivator = (player) =>
            {
                player.xEntity.xBaseStats.iEPRegenMultiplierInPCT -= 30;
                player.xEntity.xBaseStats.iMaxEP += 40;
                player.xEntity.xBaseStats._ichkBaseEP += 40 * 2;

                int previous = player.xEntity.xBaseStats.iHP;
                player.xEntity.xBaseStats.fMaxHPMultiplier -= 0.5f;
                player.xEntity.xBaseStats.iHP = player.xEntity.xBaseStats.iMaxHP;
                player.xEntity.xBaseStats._ichkHPBalance += player.xEntity.xBaseStats.iHP - previous;

            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/RubickCurse";

            perk = CreatePerk("Perk12");

            perk.Name = "Pandemic Kid";
            perk.Description = "Start the run with the Gas Mask, which is totally not going to make Mt. Bloom a cakewalk.";
            perk.EssenceCost = 50;
            perk.RunStartActivator = (player) =>
            {
                player.xInventory.AddItem(ItemCodex.ItemTypes._Facegear_GasMask, 1);
                Globals.Game._Item_Equipped(ItemCodex.ItemTypes._Facegear_GasMask, player, true);
            };
            perk.TexturePath = AssetPath + "RogueLike/Perks/PandemicKid";

        }

        private void SetupCurses()
        {
            CurseEntry curse;

            curse = CreateCurse("Curse01");

            curse.Name = "START Treaty";
            curse.Description = "Most enemies spawn with a shield. Shields deactivate after a delay.";
            curse.IsCurse = true;
            curse.ScoreModifier = 0.25f;
            curse.TexturePath = AssetPath + "RogueLike/Curses/STARTTreaty";

            curse = CreateCurse("Curse02");

            curse.Name = "Specialists";
            curse.Description = "Some enemies gain special abilities.";
            curse.IsCurse = true;
            curse.ScoreModifier = 0.20f;
            curse.TexturePath = AssetPath + "RogueLike/Curses/Specialists";

            curse = CreateCurse("Curse03");

            curse.Name = "Game Over Speedrun";
            curse.Description = "Most enemies move and attack faster.";
            curse.IsCurse = true;
            curse.ScoreModifier = 0.30f;
            curse.TexturePath = AssetPath + "RogueLike/Curses/HasteCurse";
        }

        private void SetupPins()
        {
            ModGoodies addon = GetMod("Addons.ModGoodies") as ModGoodies;

            PinEntry pin;

            pin = CreatePin("Pin001");

            pin.Description = "Gain 150 bonus Shield HP with all shields.";
            pin.PinSymbol = PinEntry.Symbol.Shield;
            pin.PinShape = PinEntry.Shape.Circle;
            pin.PinColor = PinEntry.Color.BilobaFlower;
            pin.EquipAction = (PlayerView view) =>
            {
                view.xEntity.xBaseStats.iShieldMaxHP += 150;
            };
            pin.UnequipAction = (PlayerView view) =>
            {
                view.xEntity.xBaseStats.iShieldMaxHP -= 150;
            };

            pin = CreatePin("Pin002");

            pin.Description = "Gain 1 Bonus Level in all General Talents.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Circle;
            pin.PinColor = PinEntry.Color.Coral;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin002").GameID))
                {
                    return;
                }

                if (skill.IsGeneralTalent())
                {
                    modified += 1;
                }
            });

            pin = CreatePin("Pin003");

            pin.Description = "Gain 1 Bonus Level in all Melee Talents.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Diamond;
            pin.PinColor = PinEntry.Color.Coral;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin003").GameID))
                {
                    return;
                }

                if (skill.IsMeleeTalent())
                {
                    modified += 1;
                }
            });

            pin = CreatePin("Pin004");

            pin.Description = "Gain 1 Bonus Level in all Magic Talents.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Plus;
            pin.PinColor = PinEntry.Color.Coral;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin004").GameID))
                {
                    return;
                }

                if (skill.IsMagicTalent())
                {
                    modified += 1;
                }
            });

            pin = CreatePin("Pin005");

            pin.Description = "Gain 2 Bonus Levels in Last Stand, Last Breath and Last Spark.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Square;
            pin.PinColor = PinEntry.Color.Coral;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin005").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_LastStand || skill == SpellCodex.SpellTypes._Talent_LastBreath || skill == SpellCodex.SpellTypes._Talent_LastSpark)
                {
                    modified += 2;
                }
            });

            pin = CreatePin("Pin006");

            pin.Description = "Gain 2 Bonus Levels in Second Wind, Kinetic Energy and Soul Eater.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Tablet;
            pin.PinColor = PinEntry.Color.Coral;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin006").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_SecondWind || skill == SpellCodex.SpellTypes._Talent_General_KineticEnergy || skill == SpellCodex.SpellTypes._Talent_Magic_SoulEater)
                {
                    modified += 2;
                }
            });

            pin = CreatePin("Pin007");

            pin.Description = "Gain 2 Bonus Levels in Burning Weapon, Chilling Touch and Crippling Blast.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Circle;
            pin.PinColor = PinEntry.Color.Conifer;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin007").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_BurningWeapon || skill == SpellCodex.SpellTypes._Talent_ChillyTouch || skill == SpellCodex.SpellTypes._Talent_General_KineticEnergy)
                {
                    modified += 2;
                }
            });

            pin = CreatePin("Pin008");

            pin.Description = "Gain 2 Bonus Levels in Wit, Battle Mage and Knowledge is Power.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Diamond;
            pin.PinColor = PinEntry.Color.Conifer;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin008").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_Wit || skill == SpellCodex.SpellTypes._Talent_Battlemage || skill == SpellCodex.SpellTypes._Talent_Melee_KnowledgeIsPower)
                {
                    modified += 2;
                }
            });

            pin = CreatePin("Pin009");

            pin.Description = "Gain 3 Bonus Levels in Got You Covered, and 2 in Utility Flow.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Plus;
            pin.PinColor = PinEntry.Color.Conifer;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin009").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_General_GotYouCovered)
                {
                    modified += 3;
                }
                else if (skill == SpellCodex.SpellTypes._Talent_General_UtilityFlow)
                {
                    modified += 2;
                }
            });



            pin = CreatePin("Pin010");

            pin.Description = "Gain 3 Bonus Levels in Sudden Strike, and 2 in Combo Starter.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Square;
            pin.PinColor = PinEntry.Color.Conifer;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin010").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_Melee_SuddenStrike)
                {
                    modified += 3;
                }
                else if (skill == SpellCodex.SpellTypes._Talent_Melee_ComboStarter)
                {
                    modified += 2;
                }
            });


            pin = CreatePin("Pin011");

            pin.Description = "Gain 3 Bonus Levels in Backhander, and 2 in Bloodthirst.";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Tablet;
            pin.PinColor = PinEntry.Color.Conifer;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin011").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_Backhander)
                {
                    modified += 3;
                }
                else if (skill == SpellCodex.SpellTypes._Talent_Melee_BloodThirst)
                {
                    modified += 2;
                }
            });

            pin = CreatePin("Pin012");

            pin.Description = "Gain 4 Bonus Levels in Manaburn. (Sticky)";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Circle;
            pin.PinColor = PinEntry.Color.BilobaFlower;
            pin.IsSticky = true;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin012").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_Manaburn)
                {
                    modified += 4;
                }
            });

            pin = CreatePin("Pin013");

            pin.Description = "Gain 4 Bonus Levels in Riposte. (Sticky)";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Diamond;
            pin.PinColor = PinEntry.Color.BilobaFlower;
            pin.IsSticky = true;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin013").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_Melee_Riposte)
                {
                    modified += 4;
                }
            });

            pin = CreatePin("Pin014");

            pin.Description = "Gain 4 Bonus Levels in Fine Taste. (Sticky)";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Plus;
            pin.PinColor = PinEntry.Color.BilobaFlower;
            pin.IsSticky = true;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin014").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_FineTaste)
                {
                    modified += 4;
                }
            });


            pin = CreatePin("Pin015");

            pin.Description = "Gain 6 Bonus Levels in Wand Master. (Sticky)";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Square;
            pin.PinColor = PinEntry.Color.BilobaFlower;
            pin.IsSticky = true;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin015").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_Magic_WandMaster)
                {
                    modified += 6;
                }
            });

            pin = CreatePin("Pin016");

            pin.Description = "Gain 5 Bonus Levels in Lady Luck. All other talents lose a level (but can't go below 0).";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Circle;
            pin.PinColor = PinEntry.Color.Seagull;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin016").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_General_LadyLuck)
                {
                    modified += 5;
                }
                else
                {
                    modified -= 1;
                }
            });

            pin = CreatePin("Pin017");

            pin.Description = "Gain 5 Bonus Levels in Multitasking, and 3 in Shield Bearer. All other talents lose a level (but can't go below 0).";
            pin.PinSymbol = PinEntry.Symbol.Star;
            pin.PinShape = PinEntry.Shape.Diamond;
            pin.PinColor = PinEntry.Color.Seagull;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin017").GameID))
                {
                    return;
                }

                if (skill == SpellCodex.SpellTypes._Talent_Multitasking)
                {
                    modified += 5;
                }
                else if (skill == SpellCodex.SpellTypes._Talent_ShieldBearer)
                {
                    modified += 3;
                }
                else
                {
                    modified -= 1;
                }
            });

            pin = CreatePin("Pin018");

            pin.Description = "Gain 1 Bonus Level for ALL talents. (Sticky)";
            pin.PinSymbol = PinEntry.Symbol.Exclamation;
            pin.PinShape = PinEntry.Shape.Plus;
            pin.PinColor = PinEntry.Color.Seagull;
            pin.IsSticky = true;

            addon.AddSkillEditCallback((SpellCodex.SpellTypes skill, byte original, ref int modified) =>
            {
                if (!skill.IsTalent() || !Globals.Game.xLocalPlayer.xEquipment.HasBadge(GetPin("Pin018").GameID))
                {
                    return;
                }

                modified += 1;
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

                ShieldedEnemySpell spell = (ShieldedEnemySpell)Globals.Game._EntityMaster_AddSpellInstance(GetSpell("Spell001").GameID, enemy, Vector2.Zero, false, duration, killTimeReduction);
                
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

            ShieldedEnemySpell spell = (ShieldedEnemySpell)Globals.Game._EntityMaster_AddSpellInstance(GetSpell("Spell001").GameID, enemy, Vector2.Zero, false, duration, killTimeReduction);

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

        private bool IsInArcadeRun()
        {
            return
                Globals.Game.xStateMaster.enGameMode == StateMaster.GameModes.RogueLike &&
                Globals.Game.xGameSessionData.xRogueLikeSession.bInRun;
        }

        private bool CanDoShieldCurseLogic()
        {
            if (!IsInArcadeRun())
            {
                return false;
            }

            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            var rogueData = Globals.Game.xGlobalData.xLocalRoguelikeData;

            bool coolRoom = session.xCurrentRoom.enRoomType == RogueLikeMode.Room.RoomTypes.Normal || session.xCurrentRoom.enRoomType == RogueLikeMode.Room.RoomTypes.Boss;

            return rogueData.IsTreatCurseEquipped(GetCurse("Curse01").GameID) &&
                NetUtils.IsLocalOrServer &&
                coolRoom &&
                !session.xCurrentRoom.bCompleted;
        }

        private bool CanDoSpecialistCurseLogic()
        {
            if (!IsInArcadeRun())
            {
                return false;
            }

            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            var rogueData = Globals.Game.xGlobalData.xLocalRoguelikeData;

            bool coolRoom = session.xCurrentRoom.enRoomType == RogueLikeMode.Room.RoomTypes.Normal;

            return rogueData.IsTreatCurseEquipped(GetCurse("Curse02").GameID) &&
                NetUtils.IsLocalOrServer &&
                coolRoom &&
                !session.xCurrentRoom.bCompleted;
        }
        
        private bool CanDoHasteCurseLogic()
        {
            if (!IsInArcadeRun())
            {
                return false;
            }

            var session = Globals.Game.xGameSessionData.xRogueLikeSession;

            var rogueData = Globals.Game.xGlobalData.xLocalRoguelikeData;

            // Non-player buffs have to be done on the client too

            return rogueData.IsTreatCurseEquipped(GetCurse("Curse03").GameID) &&
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
                var spell = Globals.Game._EntityMaster_AddSpellInstance(GetSpell("Spell002").GameID, enemy, Vector2.Zero, false);

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
                var spell = Globals.Game._EntityMaster_AddSpellInstance(GetSpell("Spell003").GameID, enemy, Vector2.Zero, false);

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
                var spell = Globals.Game._EntityMaster_AddSpellInstance(GetSpell("Spell004").GameID, enemy, Vector2.Zero, false);
                
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
