using UnityEngine;
using System;
using System.Collections;

namespace Avrahamy.Utils {
    public static class CoroutineUtils {
        public static readonly WaitForFixedUpdate WAIT_FOR_FIXED_UPDATE = new WaitForFixedUpdate();
        public static readonly WaitForEndOfFrame WAIT_FOR_END_OF_FRAME = new WaitForEndOfFrame();

        /// <summary>
        /// Use this inside a coroutine.
        /// </summary>
        public static IEnumerator WaitForRealSeconds(float time) {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time) {
                yield return null;
            }
        }

        public static Coroutine WaitForCondition(this MonoBehaviour context, Func<bool> condition, Action action = null, float timeToWait = -1f) {
            return context.StartCoroutine(WaitForCondition(condition, action, timeToWait));
        }

        /// <summary>
        /// Usage: StartCoroutine(CoroutineUtils.WaitForCondition(IsSomething, action))
        /// </summary>
        public static IEnumerator WaitForCondition(Func<bool> condition, Action action = null, float timeToWait = -1f) {
            var wait = timeToWait > 0f ? new WaitForSeconds(timeToWait) : null;
            while (!condition()) {
                yield return wait;
            }

            action?.Invoke();
        }

        public static Coroutine DoWhileCondition(this MonoBehaviour context, Func<bool> condition, Action action, Action endAction = null) {
            return context.StartCoroutine(DoWhileCondition(condition, action, endAction));
        }

        /// <summary>
        /// Usage: StartCoroutine(CoroutineUtils.DoWhileCondition(IsSomething, action))
        /// </summary>
        public static IEnumerator DoWhileCondition(Func<bool> condition, Action action, Action endAction = null) {
            while (condition()) {
                action();
                yield return null;
            }

            endAction?.Invoke();
        }

        public static Coroutine Repeat(this MonoBehaviour context, Action action, float interval, float delay = 0f) {
            return context.StartCoroutine(Repeat(action, interval, delay));
        }

        /// <summary>
        /// Usage: StartCoroutine(CoroutineUtils.Repeat(action, interval))
        /// </summary>
        public static IEnumerator Repeat(Action action, float interval, float delay = 0f) {
            var wait = new WaitForSeconds(interval);
            if (delay > 0f) {
                if (Mathf.Approximately(delay, interval)) {
                    yield return wait;
                } else {
                    yield return new WaitForSeconds(delay);
                }
            }
            while (true) {
                action();
                yield return wait;
            }
        }

        /// <summary>
        /// Usage: this.DelaySeconds(action, delay)
        ///
        /// WARNING: It is inefficient to call this a lot. If you want something to
        /// run every X seconds, it is best to use Repeat.
        /// StartCoroutine is heavy as well as new WaitForSeconds() - those can be
        /// cached and reused.
        /// </summary>
        public static Coroutine DelaySeconds(this MonoBehaviour context, Action action, float delay) {
            return context.StartCoroutine(DelaySeconds(action, delay));
        }

        /// <summary>
        /// Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
        /// </summary>
        public static IEnumerator DelaySeconds(Action action, float delay) {
            yield return new WaitForSeconds(delay);
            action();
        }

        public static Coroutine DelayRealSeconds(this MonoBehaviour context, Action action, float delay) {
            return context.StartCoroutine(_DelayRealSeconds(context, action, delay));
        }

        private static IEnumerator _DelayRealSeconds(MonoBehaviour context, Action action, float delay) {
            yield return context.StartCoroutine(WaitForRealSeconds(delay));
            action();
        }

        public static Coroutine DelayForFixedUpdate(this MonoBehaviour context, Action action) {
            return context.StartCoroutine(DelayForFixedUpdate(action));
        }

        /// <summary>
        /// Usage: StartCoroutine(CoroutineUtils.DelayForFixedUpdate(action))
        /// </summary>
        public static IEnumerator DelayForFixedUpdate(Action action) {
            yield return WAIT_FOR_FIXED_UPDATE;
            action();
        }

        public static Coroutine DelayForEndOfFrame(this MonoBehaviour context, Action action) {
            return context.StartCoroutine(DelayForEndOfFrame(action));
        }

        /// <summary>
        /// Usage: StartCoroutine(CoroutineUtils.DelayForEndOfFrame(action))
        /// </summary>
        public static IEnumerator DelayForEndOfFrame(Action action) {
            yield return WAIT_FOR_END_OF_FRAME;
            action();
        }

        /// <summary>
        /// Usage: this.Chain(context, ...))
        /// For example:
        ///     this.Chain(context,
        ///         this.Do(() => DebugLog.Log("A")),
        ///         this.WaitForRealSeconds(2),
        ///         this.Do(() => DebugLog.Log("B"))));
        /// </summary>
        public static Coroutine Chain(this MonoBehaviour context, params IEnumerator[] actions) {
            return context.StartCoroutine(_Chain(context, actions));
        }

        private static IEnumerator _Chain(MonoBehaviour context, params IEnumerator[] actions) {
            foreach (var action in actions) {
                yield return context.StartCoroutine(action);
            }
        }

        /// <summary>
        /// A simple wrapper for StartCoroutine.
        /// </summary>
        public static Coroutine Do(this MonoBehaviour context, Action action) {
            return context.StartCoroutine(Do(action));
        }

        public static IEnumerator Do(Action action) {
            action();
            yield break;
        }
    }
}
