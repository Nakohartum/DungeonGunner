using UnityEngine;

namespace Weapons.Ammo
{
    [DisallowMultipleComponent]
    public class AmmoHitEffect : MonoBehaviour
    {
        private ParticleSystem ammoHitParticleSystem;

        private void Awake()
        {
            ammoHitParticleSystem = GetComponent<ParticleSystem>();
        }

        public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
        {
            SetHitEffectColorGradient(ammoHitEffect.colorGradient);
            SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize,
                ammoHitEffect.startParticleSpeed, ammoHitEffect.startLifetime, ammoHitEffect.effectGravity,
                ammoHitEffect.maxParticleNumber);
            SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);
            SetHitEffectParticleSprite(ammoHitEffect.sprite);
            SetHitEffectVelocityOverLifetime(ammoHitEffect.velocityOverLifetimeMin,
                ammoHitEffect.velocityOverLifetimeMax);
        }
        
        private void SetHitEffectColorGradient(Gradient gradient)
        {
            ParticleSystem.ColorOverLifetimeModule
                colorOverLifetimeModule = ammoHitParticleSystem.colorOverLifetime;
            colorOverLifetimeModule.color = gradient;
        }
        
        private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, 
            float startParticleSpeed, float startLifetime, 
            float effectGravity, int maxParticles)
        {
            ParticleSystem.MainModule mainModule = ammoHitParticleSystem.main;
            mainModule.duration = duration;
            mainModule.startSize = startParticleSize;
            mainModule.startSpeed = startParticleSpeed;
            mainModule.startLifetime = startLifetime;
            mainModule.gravityModifier = effectGravity;
            mainModule.maxParticles = maxParticles;
        }

        private void SetHitEffectParticleEmission(int emissionRate, int burstParticleNumber)
        {
            ParticleSystem.EmissionModule emissionModule = ammoHitParticleSystem.emission;
            ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
            emissionModule.SetBurst(0, burst);
            emissionModule.rateOverTime = emissionRate;
        }
        
        private void SetHitEffectParticleSprite(Sprite sprite)
        {
            ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule =
                ammoHitParticleSystem.textureSheetAnimation;
            textureSheetAnimationModule.SetSprite(0, sprite);
        }

        private void SetEmitterRotation(float aimAngle)
        {
            transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
        }

        private void SetHitEffectVelocityOverLifetime(Vector3 minVelocity, Vector3 maxVelocity)
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule =
                ammoHitParticleSystem.velocityOverLifetime;
            ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
            minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
            minMaxCurveX.constantMin = minVelocity.x;
            minMaxCurveX.constantMax = maxVelocity.x;
            velocityOverLifetimeModule.x = minMaxCurveX;
            
            ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
            minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
            minMaxCurveY.constantMin = minVelocity.y;
            minMaxCurveY.constantMax = maxVelocity.y;
            velocityOverLifetimeModule.y = minMaxCurveY;
            
            ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
            minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
            minMaxCurveZ.constantMin = minVelocity.z;
            minMaxCurveZ.constantMax = maxVelocity.z;
            velocityOverLifetimeModule.z = minMaxCurveZ;
        }
    }
}