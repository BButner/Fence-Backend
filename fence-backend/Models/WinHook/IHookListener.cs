namespace fence_backend.Models.WinHook
{
    public interface IHookListener
    {
        void Start();

        void Stop();
    }
}