
namespace RGuang.Kit
{
    public interface IPoolable
    {
        bool IsRecycled { get; set; }
        void OnRecycled();
    }

}
