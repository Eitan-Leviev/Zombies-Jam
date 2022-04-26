using System;
using System.Collections.Generic;
using Avrahamy.Collections;
using Avrahamy.Utils;

namespace Avrahamy.Messages {
    public class MessagesHubImpl : IMessagesHub {
        private readonly Dictionary<Type, List<IMessageHandler>> subscribers =
            new Dictionary<Type, List<IMessageHandler>>();

        private readonly List<IMessageHandler> globalSubscribers = new List<IMessageHandler>();

        public void Subscribe(Type messageType, IMessageHandler subscriber) {
            var list = subscribers.EnsureListForKey(messageType);
            if (list.Contains(subscriber)) return;

            DebugLog.Log(LogTag.Messages, $"Subscribed {subscriber} to {messageType}");
#if UNITY_EDITOR
            DebugAssert.Assert(messageType.Name.EndsWith("Message"), $"{subscriber} subscribed to {messageType} that does not seem to be a message (name should have a Message suffix)");
#endif
            list.Add(subscriber);
        }

        public void Subscribe<T>(IMessageHandler subscriber) where T : class {
            Subscribe(typeof(T), subscriber);
        }

        public void Unsubscribe(Type messageType, IMessageHandler subscriber) {
            if (!subscribers.ContainsKey(messageType)) return;

            var list = subscribers[messageType];
            var removed = list.Remove(subscriber);
            DebugLog.LogIf(LogTag.Messages, removed, $"Unsubscribed {subscriber} from {messageType}");
        }

        public void Unsubscribe<T>(IMessageHandler subscriber) where T : class {
            Unsubscribe(typeof(T), subscriber);
        }

        /// <summary>
        /// Global subscribers receive all messages dispatched to this hub.
        /// </summary>
        public void SubscribeGlobalSubscriber(IMessageHandler subscriber) {
            globalSubscribers.Add(subscriber);
        }

        public void UnsubscribeGlobalSubscriber(IMessageHandler subscriber) {
            globalSubscribers.Remove(subscriber);
        }

        public void Dispatch(object message) {
            var type = message.GetType();
            if (message is CompoundMessage compoundMessage) {
                foreach (var item in compoundMessage.Messages) {
                    Dispatch(item);
                }
                return;
            }
            if (subscribers.ContainsKey(type)) {
                var list = subscribers[type];
                DebugLog.Log(LogTag.Messages, $"{this} Have {list.Count} subscribers to {message}");
                DispatchToSubscribers(list, message);
            } else {
                DebugLog.Log(LogTag.Messages, $"{this} No subscribers to {message}");
            }

            DispatchToSubscribers(globalSubscribers, message);
        }

        public void DispatchOnNextFrame(object message) {
            PureCoroutines.Instance.DelayForEndOfFrame(() => Dispatch(message));
        }

        public void DispatchAll(IMessageGenerator[] messageGenerators) {
            foreach (var messageGenerator in messageGenerators) {
                var message = messageGenerator.New();
                Dispatch(message);
            }
        }

        public void OnMessage(object message) {
            Dispatch(message);
        }

        private void DispatchToSubscribers<T>(IList<IMessageHandler> originalSubscribers, T message) where T : class {
            for (int i = originalSubscribers.Count - 1; i >= 0; i--) {
                if (originalSubscribers[i] != null) continue;

                DebugLog.Log(LogTag.Messages, $"{this} Removed dead subscriber at #{i} to {message}");
                originalSubscribers.RemoveAt(i);
            }

            var subscribersCopy = new List<IMessageHandler>(originalSubscribers);
            foreach (var subscriber in subscribersCopy) {
                subscriber.OnMessage(message);
            }
        }
    }
}
