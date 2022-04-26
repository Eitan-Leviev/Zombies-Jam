namespace Avrahamy {
    public interface ITimer {
        float StartTime { get; set; }
        float EndTime { get; set; }
        float Duration { get; set; }
        float ElapsedTime { get; }
        float RemainingTime { get; }
        bool IsActive { get; }
        bool IsSet { get; }
        float Progress { get; }
        void Start();
        void Start(float duration);
        void Clear();
    }
}