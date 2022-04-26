using UnityEngine;
using System;

namespace Avrahamy.Messages {
    public interface IMessagePredicate {
        Type GetMessageType();
    }

    public abstract class MessagePredicateBase<T> : ScriptableObject, IMessagePredicate where T : class {
        public Type GetMessageType() {
            return typeof(T);
        }
    }

    public abstract class MessagePredicateComponent<T> : MonoBehaviour, IMessagePredicate where T : class {
        public Type GetMessageType() {
            return typeof(T);
        }
    }
}
