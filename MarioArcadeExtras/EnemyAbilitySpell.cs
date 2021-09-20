using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SoG;
using SoG.Modding.Core;
using SoG.Modding.ModUtils;
using Watchers;

namespace Murio
{
    public abstract class EnemyAbilitySpell : ISpellInstance
    {
        public Enemy Owner => xSpellOwner as Enemy;

        public SortedAnimated AbilityIcon { get; protected set; }

        protected void CreateAbilityIcon(Color iconColor)
        {
            AbilityIcon = new SortedAnimated(Vector2.Zero, SortedAnimated.SortedAnimatedEffects._EnemyEffects_DebuffEffect_Poison);

            Globals.Game._EffectMaster_AddEffect(AbilityIcon);

            AbilityIcon.xRenderComponent.cColor = iconColor;

            AbilityIcon.xRenderComponent.v2OffsetRenderPos = Owner.xEnemyDescription.v2ApproximateOffsetToMid;
            AbilityIcon.xRenderComponent.v2OffsetRenderPos.Y -= Owner.xEnemyDescription.v2ApproximateSize.Y;

            AbilityIcon.xRenderComponent.fVirtualHeight += Owner.xRenderComponent.fVirtualHeight;
            AbilityIcon.xRenderComponent.xTransform = Owner.xTransform;
            AbilityIcon.xTransform = Owner.xTransform;
        }
    }
}
