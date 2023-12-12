
namespace RGuang
{
    public interface IPoolable
    {
        bool IsRecycled { get; set; }
        void OnRecycled();
    }

}
