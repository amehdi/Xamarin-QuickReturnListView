using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace QuickReturnList
{
    [Activity(Label = "QuickReturnList", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private QuickReturnListView mListView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            mListView = FindViewById<QuickReturnListView>(Android.Resource.Id.List);
            mListView.Init(FindViewById(Resource.Id.sticky));
            
            string[] array = new string[] { "Android 1", "Android 2", "Android 3", "Android 4", "Android 5", "Android 6", "Android 7", "Android 8", "Android 9", "Android", "Android", "Android", "Android", "Android", "Android", "Android" };
            //mListView.Adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, Resource.Id.text1, array);
        }
    }
}