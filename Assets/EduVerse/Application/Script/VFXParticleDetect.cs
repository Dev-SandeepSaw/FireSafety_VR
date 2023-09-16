using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace OperationAutoManagerVer3
{
    public class VFXParticleDetect : MonoBehaviour
    {
        public VisualEffect vfx;
        [Header("Tag var Particle")]
        public string varRate;
        public float currentRate;
        private float tempCurrentRate;
        public string particleTagName;
        [Header("Rate of particle")]
        public float minRate = 0;
        public float maxRate = 12;
        public float speed = 0.8f;
        [Header("Collision Condition")]
        public bool increaseRateOnCollision;
        public bool decreaseRateOnCollision = true;
        public bool autoRateIncrease = true, autoRateDecrease;
        public float autoIncDecSpeed = 3;
        private bool particleCollision;
        private bool outCollision;
        private Coroutine coutDelayCollision;
        private Coroutine coutDelayOutCollision;
        public UnityEvent eventParticleMin;
        public UnityEvent eventParticleMax;
        [HideInInspector] public float delayCollision = 4;
        [HideInInspector] public float delayOutCollision = 2;
        private void Start()
        {
            if (vfx == null)
                vfx = GetComponent<VisualEffect>();
            if (decreaseRateOnCollision)
                currentRate = maxRate;
            else if (increaseRateOnCollision)
                currentRate = 0;
            vfx.SetFloat(varRate, currentRate);
        }
        private void FixedUpdate()
        {
            if (autoRateIncrease && !particleCollision)
                if (currentRate <= maxRate)
                    currentRate += Time.deltaTime * autoIncDecSpeed;
            if (autoRateDecrease && !particleCollision)
                if (currentRate >= minRate)
                    currentRate -= Time.deltaTime * autoIncDecSpeed;

            vfx.SetFloat(varRate, currentRate);
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.gameObject.CompareTag(particleTagName))
            {
                if (!outCollision)
                {
                    coutDelayCollision = StartCoroutine(delay(delayCollision));
                    particleCollision = true;
                    outCollision = true;
                }
                if (decreaseRateOnCollision && particleCollision && (currentRate >= minRate))
                    RateCollisionSpeed(-speed, true, false);
                else if (increaseRateOnCollision && particleCollision && (currentRate <= maxRate))
                    RateCollisionSpeed(speed, false, true);
            }
        }
        IEnumerator delay(float time)
        {
            yield return new WaitForSeconds(time);
            outCollision = false;
            coutDelayOutCollision = StartCoroutine(delayOut(delayOutCollision));
            StopCoroutine(coutDelayCollision);
        }

        IEnumerator delayOut(float time)
        {
            yield return new WaitForSeconds(time);
            if (!outCollision)
            {
                particleCollision = false;
                StopCoroutine(coutDelayOutCollision);
            }
            else
                StopCoroutine(coutDelayOutCollision);

        }

        private void RateCollisionSpeed(float speed, bool min, bool max)
        {
            currentRate += Time.deltaTime + speed;
            if (min)
            {
                if (currentRate <= minRate)
                    eventParticleMin?.Invoke();
            }
            else if (max)
            {
                if (currentRate >= maxRate)
                    eventParticleMax?.Invoke();
            }
        }

    }
}