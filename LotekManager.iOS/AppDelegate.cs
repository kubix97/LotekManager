using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using UIKit;

namespace LotekManager.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.FormsMaps.Init();
            CopyDB();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public void CopyDB()
		{
             var sqliteFilename = "LotekDB.db";
 
             string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
             string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");
 
             if (!Directory.Exists(libFolder))
             {
                 Directory.CreateDirectory(libFolder);
             }
             string path = Path.Combine(libFolder, sqliteFilename);
 
             // This is where we copy in the pre-created database
             if (!File.Exists(path))
             {
                 var existingDb = NSBundle.MainBundle.PathForResource("Employee", "db");
                 File.Copy (existingDb, path);
             }
		}
    }
}
