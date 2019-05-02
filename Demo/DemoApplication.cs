using System;
using Android.App;
using Android.Runtime;
using PhotectorSharp;

namespace Demo
{
    [Application]
    public class DemoApplication : Application
    {
        public DemoApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();

            try
            {
                Photector.Instance.Init(this, "", "", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}