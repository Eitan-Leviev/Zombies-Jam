using UnityEngine;
using Object = UnityEngine.Object;
using System;
using Avrahamy.Utils;

namespace Avrahamy.Messages {
    [DisallowMultipleComponent]
    public class MessagesHub : MonoBehaviour, IMessagesHub {
        private readonly MessagesHubImpl impl = new MessagesHubImpl();

        public void Subscribe(Type messageType, IMessageHandler subscriber) {
            DebugLog.Log(LogTag.Messages, transform.PathInHierarchy() + ": Subscribe " + subscriber + " to " + messageType, subscriber as Object);
            impl.Subscribe(messageType, subscriber);
        }

        public void Subscribe<T>(IMessageHandler subscriber) where T : class {
            DebugLog.Log(LogTag.Messages, transform.PathInHierarchy() + ": Subscribe " + subscriber + " to " + typeof(T), subscriber as Object);
            impl.Subscribe<T>(subscriber);
        }

        public void Unsubscribe(Type messageType, IMessageHandler subscriber) {
            DebugLog.Log(LogTag.Messages, transform.PathInHierarchy() + ": Unsubscribe " + subscriber + " from " + messageType, subscriber as Object);
            impl.Unsubscribe(messageType, subscriber);
        }

        public void Unsubscribe<T>(IMessageHandler subscriber) where T : class {
            DebugLog.Log(LogTag.Messages, transform.PathInHierarchy() + ": Unsubscribe " + subscriber + " from " + typeof(T), subscriber as Object);
            impl.Unsubscribe<T>(subscriber);
        }

        /// <summary>
        /// Global subscribers receive all messages dispatched to this hub.
        /// </summary>
        public void SubscribeGlobalSubscriber(IMessageHandler subscriber) {
            impl.SubscribeGlobalSubscriber(subscriber);
        }

        public void UnsubscribeGlobalSubscriber(IMessageHandler subscriber) {
            impl.UnsubscribeGlobalSubscriber(subscriber);
        }

        public void Dispatch(object message) {
            DebugLog.Log(LogTag.Messages, transform.PathInHierarchy() + " Dispatch " + message, this);
            impl.Dispatch(message);
        }

        public void OnMessage(object message) {
            impl.OnMessage(message);
        }
    }
}
