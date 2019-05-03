using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IO.Fotoapparat;
using Java.Lang;
using PhotectorSharp;
using System.Collections.Generic;

namespace Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Fotoapparat camera;
        IO.Fotoapparat.View.CameraView cameraView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            this.cameraView = (IO.Fotoapparat.View.CameraView)FindViewById(Resource.Id.cameraView1);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            this.camera = Fotoapparat.With(this).Into(cameraView).Build();

            fab.Click += FabOnClick;
        }

        protected override void OnResume()
        {
            base.OnResume();

            //camera.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();

            //camera.Stop();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Photector.Instance.CreateEvent(this, "", new List<System.String>());
            }
            catch (System.Exception e) {
                Console.WriteLine(e);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    class Test : Java.Lang.Object, IO.Fotoapparat.Result.IWhenDoneListener
    {
        public void WhenDone(Java.Lang.Object p0)
        {
            var bitmap = (IO.Fotoapparat.Result.BitmapPhoto)p0;
            Console.WriteLine("made photo {0} bytes", bitmap.Bitmap.ByteCount);
        }
    }
}

