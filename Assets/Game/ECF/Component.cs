
namespace RollBackExample
{
    public interface Component
    {
        int OwnerId { get; set; }
        int ExecuteIndex { get; }
        void Update();
        void Start(Entity owner);
    }
}