using UnityEngine;
using Object = UnityEngine.Object;
using Avrahamy.EditorGadgets;

namespace Avrahamy.Messages {
    public class CompoundMessageGenerator : MessageGeneratorComponent {
        [ImplementsInterface(typeof(IMessageGenerator))]
        [SerializeField] Object[] messages;

        public override object New() {
            var actualMessages = new object[messages.Length];
            for (int i = 0; i < messages.Length; i++) {
                var messageGenerator = messages[i] as IMessageGenerator;
                actualMessages[i] = messageGenerator.New();
            }
            return new CompoundMessage(actualMessages);
        }
    }

    public class CompoundMessage {
        private readonly object[] messages;

        public object[] Messages {
            get {
                return messages;
            }
        }

        public CompoundMessage(object[] messages) {
            this.messages = messages;
        }
    }
}
