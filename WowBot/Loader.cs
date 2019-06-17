using System.Threading;

namespace WowBot
{
    class Loader
    {
        static Thread thread;

        static int Load(string args)
        {
            thread = new Thread(App.Main);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return 1;
        }
    }
}
