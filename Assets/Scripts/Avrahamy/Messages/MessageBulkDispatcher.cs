using UnityEngine;

namespace Avrahamy.Messages {
    [CreateAssetMenu(menuName = "Avrahamy/Message Bulk Dispatcher", fileName = "MessageBulkDispatcher")]
    public class MessageBulkDispatcher : ScriptableObject {
        [SerializeField] MessageGeneratorBase[] messages;

        public void Dispatch() {
            foreach (var message in messages) {
                GlobalMessagesHub.Instance.Dispatch(message.New());
            }
        }
    }
}
