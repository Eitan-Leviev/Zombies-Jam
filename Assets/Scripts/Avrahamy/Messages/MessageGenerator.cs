using UnityEngine;

namespace Avrahamy.Messages {
    public interface IMessageGenerator {
        object New();
    }

    public abstract class MessageGeneratorBase : ScriptableObject, IMessageGenerator {
        public abstract object New();
    }

    public abstract class MessageGeneratorComponent : MonoBehaviour, IMessageGenerator {
        public abstract object New();

        public virtual void Dispatch() {
            GlobalMessagesHub.Instance.Dispatch(New());
        }
    }

    public abstract class MessageGeneratorNoParams<T>: MessageGeneratorBase where T : NoParamsMessage<T>, new() {
        public override object New() {
            return new T();
        }
    }
}
