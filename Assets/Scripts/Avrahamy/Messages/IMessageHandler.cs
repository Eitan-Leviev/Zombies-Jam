namespace Avrahamy.Messages {
    public interface IMessageHandler {
        void OnMessage(object message);
    }
}
