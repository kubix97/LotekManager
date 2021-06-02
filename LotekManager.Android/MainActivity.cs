﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;

namespace LotekManager.Droid
{
    [Activity(Label = "LotekManager", Icon = "@mipmap/appIcon", Theme = "@style/MainTheme", /*MainLauncher = true,*/ ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            CopyDb();
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void CopyDb()
		{
             var sqliteFilename = "LotekDB.db";
             string documentsDirectoryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); 
             var path = Path.Combine(documentsDirectoryPath, sqliteFilename);
     
             // This is where we copy in our pre-created database
             if (!File.Exists (path)) {
                using (var binaryReader = new BinaryReader(Android.App.Application.Context.Assets.Open(sqliteFilename))) {
                    using (var binaryWriter = new BinaryWriter(new FileStream(path, FileMode.Create))) {
                        byte[] buffer = new byte[2048];
                        int length = 0;
                        while ((length = binaryReader.Read (buffer, 0, buffer.Length)) > 0){
                            binaryWriter.Write (buffer, 0, length);
                        }
                    }
                }
 
		     }
        }
    }
}