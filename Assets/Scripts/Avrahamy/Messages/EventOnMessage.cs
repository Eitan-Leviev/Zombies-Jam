using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Avrahamy.EditorGadgets;

namespace Avrahamy.Messages {
    public class EventOnMessage : MonoBehaviour, IMessageHandler {
        [SerializeField] UnityEvent onMessage;
        [ImplementsInterface(typeof(IMessagePredicate))]
        [SerializeField] Object messagePredicate;

        protected void Awake() {
            var messageType = (messagePredicate as IMessagePredicate).GetMessageType();
            GlobalMessagesHub.Instance.Subscribe(messageType, this);
        }

        protected void OnDestroy() {
            var messageType = (messagePredicate as IMessagePredicate).GetMessageType();
            GlobalMessagesHub.Instance.Unsubscribe(messageType, this);
        }

        public void OnMessage(object message) {
            onMessage?.Invoke();
        }
    }
}
