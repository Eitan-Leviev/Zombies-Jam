namespace Avrahamy.Messages {
    public abstract class NoParamsMessage<T> where T : class, new() {
        public static readonly T Instance = new T();
    }
}
