using Microsoft.Xna.Framework;
using SoG;
using SoG.Modding;

namespace Murio
{
    public abstract class EnemyAbilitySpell : ISpellInstance
    {
        public Enemy Owner => xSpellOwner as Enemy;

        public SortedAnimated AbilityIcon { get; protected set; }

        public SortedAnimated RadiusIndicator { get; private set; }

        protected void CreateAbilityIcon()
        {
            if (AbilityIcon != null)
            {
                AbilityIcon.bToBeDestroyed = true;
            }

            AbilityIcon = new SortedAnimated(Vector2.Zero, SortedAnimated.SortedAnimatedEffects._EnemyEffects_DebuffEffect_Poison);

            Globals.Game._EffectMaster_AddEffect(AbilityIcon);

            AbilityIcon.xRenderComponent.v2OffsetRenderPos = Owner.xEnemyDescription.v2ApproximateOffsetToMid;
            AbilityIcon.xRenderComponent.v2OffsetRenderPos.Y -= Owner.xEnemyDescription.v2ApproximateSize.Y;

            AbilityIcon.xRenderComponent.fVirtualHeight += Owner.xRenderComponent.fVirtualHeight;
            AbilityIcon.xRenderComponent.xTransform = Owner.xTransform;
            AbilityIcon.xTransform = Owner.xTransform;
        }

        protected void CreateRadiusIndicator()
        {
            if (RadiusIndicator != null)
            {
                RadiusIndicator.bToBeDestroyed = true;
            }

            RadiusIndicator = new SortedAnimated(Vector2.Zero, SortedAnimated.SortedAnimatedEffects._SkillEffects_OneHand_SpiritSlash_AOE_Lv1);

            Globals.Game._EffectMaster_AddEffect(RadiusIndicator);

            RadiusIndicator.xRenderComponent.fVirtualHeight += Owner.xRenderComponent.fVirtualHeight;
            RadiusIndicator.xRenderComponent.xTransform = Owner.xTransform;
            RadiusIndicator.xTransform = Owner.xTransform;
        }

        protected void EditAbilityIcon(Color baseColor, float lightLevel)
        {
            if (AbilityIcon == null)
            {
                return;
            }

            float light = MathHelper.Clamp(lightLevel, 0f, 1f);

            Color newColor = baseColor;

            newColor.R = (byte)(newColor.R * light);
            newColor.G = (byte)(newColor.G * light);
            newColor.B = (byte)(newColor.B * light);

            AbilityIcon.xRenderComponent.cColor = newColor;
        }

        protected void EditRadiusIndicator(Color baseColor, float alpha, float radius)
        {
            if (RadiusIndicator == null)
            {
                return;
            }

            RadiusIndicator.xRenderComponent.cColor = baseColor;
            RadiusIndicator.xRenderComponent.fAlpha = alpha;
            RadiusIndicator.xRenderComponent.fScale = radius / 55f; // 55 is the Spirit Slash AOE texture's radius
        }
    }
}
