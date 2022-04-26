namespace Avrahamy.Messages {
    public class GlobalMessagesHub : MessagesHubImpl {
        public static GlobalMessagesHub Instance {
            get {
                if (instance == null) {
                    instance = new GlobalMessagesHub();
                }
                return instance;
            }
        }
        private static GlobalMessagesHub instance;
    }
}
