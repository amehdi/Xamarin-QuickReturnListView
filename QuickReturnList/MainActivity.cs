using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;

namespace QuickReturnList
{
    [Activity(Label = "QuickReturnList", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private QuickReturnListView mListView;
        private bool gone = true;
        private bool animating = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            TextView slide = FindViewById<TextView>(Resource.Id.slide);
            Animation slideIn = AnimationUtils.LoadAnimation(ApplicationContext, Resource.Animation.slide_in_bottom);
            Animation slideOut = AnimationUtils.LoadAnimation(ApplicationContext, Resource.Animation.slide_out_bottom);
            slideIn.AnimationEnd += (s, e) => { slide.Visibility = ViewStates.Visible; gone = false; animating = false; };
            slideOut.AnimationEnd += (s, e) => { slide.Visibility = ViewStates.Invisible; gone = true; animating = false; };

            string[] array = new string[] { "Android 1", "Android 2", "Android 3", "Android 4", "Android 5", "Android 6", "Android 7", "Android 8", "Android 9", "Android", "Android", "Android", "Android", "Android", "Android", "Android" };
            FindViewById(Resource.Id.sticky).Click += (s, e) =>
            {
                if (!animating)
                {
                    animating = true;
                    if (gone)
                        slide.StartAnimation(slideIn);
                    else
                        slide.StartAnimation(slideOut);
                }
            };

            mListView = FindViewById<QuickReturnListView>(Android.Resource.Id.List);
            mListView.Init(FindViewById(Resource.Id.sticky));

            mListView.Adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, Resource.Id.text1, array);
        }
    }
}