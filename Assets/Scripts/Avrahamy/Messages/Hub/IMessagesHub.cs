using System;

namespace Avrahamy.Messages {
    public delegate void OnMessageCallback(object message);

    public interface IMessagesHub : IMessageHandler {
        void Subscribe(Type messageType, IMessageHandler subscriber);
        void Subscribe<T>(IMessageHandler subscriber) where T : class;

        void Unsubscribe(Type messageType, IMessageHandler subscriber);
        void Unsubscribe<T>(IMessageHandler subscriber) where T : class;

        /// <summary>
        /// Global subscribers receive all messages dispatched to this hub.
        /// </summary>
        void SubscribeGlobalSubscriber(IMessageHandler subscriber);
        void UnsubscribeGlobalSubscriber(IMessageHandler subscriber);

        void Dispatch(object message);
    }
}
