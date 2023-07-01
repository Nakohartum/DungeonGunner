using System;
using System.Collections;
using Health;
using Misc;
using Sounds;
using UnityEngine;

namespace Environment
{
    [DisallowMultipleComponent]
    public class DestroyableItem : MonoBehaviour
    {
        #region Header HEALTH

        [Space(10)]
        [Header("HEALTH")]

        #endregion

        #region Tooltip

        [Tooltip("What the starting health for this destroyable item should be")]

        #endregion

        [SerializeField]
        private int startingHealthAmount = 1;

        #region Header SOUND EFFECT

        [Space(10)]
        [Header("SOUND EFFECT")]

        #endregion

        #region Tooltip

        [Tooltip("The sound effect when this item is destroyed")]

        #endregion

        [SerializeField]
        private SoundEffectSO destroySoundEffect;

        private Animator animator;
        private BoxCollider2D boxCollider2D;
        private HealthEvent healthEvent;
        private Health.Health health;
        private ReceiveContactDamage receiveContactDamage;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            healthEvent = GetComponent<HealthEvent>();
            health = GetComponent<Health.Health>();
            health.SetStartingHealth(startingHealthAmount);
            receiveContactDamage = GetComponent<ReceiveContactDamage>();
        }

        private void OnEnable()
        {
            healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        }

        private void OnDisable()
        {
            healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
        }

        private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
        {
            if (healthEventArgs.healthAmount <= 0f)
            {
                StartCoroutine(PlayAnimation());
            }
        }

        private IEnumerator PlayAnimation()
        {
            Destroy(boxCollider2D);
            if (destroySoundEffect != null)
            {
                SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
            }

            animator.SetBool(Settings.destroy, true);

            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
            {
                yield return null;
            }
            
            Destroy(animator);
            Destroy(receiveContactDamage);
            Destroy(health);
            Destroy(healthEvent);
            Destroy(this);
        }
    }
}