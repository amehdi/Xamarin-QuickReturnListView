using System;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using AttributeSet = Android.Util.IAttributeSet;

namespace QuickReturnList
{
    public class QuickReturnListView : ListView
    {
        private int mItemCount;
        private int[] mItemOffsetY;
        private bool scrollIsComputed = false;
        private int mHeight;

        private int mQuickReturnHeight;
        private int mCachedVerticalScrollRange;
        private const int STATE_ONSCREEN = 0;
        private const int STATE_OFFSCREEN = 1;
        private const int STATE_RETURNING = 2;
        private const int STATE_EXPANDED = 3;
        private int mState = STATE_ONSCREEN;
        private int mScrollY;
        private int mMinRawY = 0;
        private int rawY;
        private bool noAnimation = false;

        private TranslateAnimation anim;

        public QuickReturnListView(Context context)
            : base(context)
        {
        }

        public QuickReturnListView(Context context, AttributeSet attrs)
            : base(context, attrs)
        {
        }

        public void Init(View quickReturnView)
        {
            var mHeader = new LinearLayout(this.Context);
            mHeader.LayoutParameters = new AbsListView.LayoutParams(AbsListView.LayoutParams.MatchParent, AbsListView.LayoutParams.MatchParent);
            var mHeaderPH = new View(this.Context);
            mHeaderPH.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            mHeader.AddView(mHeaderPH);
            this.AddHeaderView(mHeader);

            var supportTranslateY = Build.VERSION.SdkInt > BuildVersionCodes.Honeycomb;

            this.ViewTreeObserver.GlobalLayout += delegate
            {
                mQuickReturnHeight = quickReturnView.Height;
                mHeaderPH.LayoutParameters.Height = mQuickReturnHeight;
                this.computeScrollY();
            };

            this.ChildViewAdded += (s, e) => { this.computeScrollY(); };
            this.ChildViewRemoved += (s, e) => { this.computeScrollY(); };

            this.Scroll += (s, e) =>
            {
                mScrollY = 0;
                int translationY = 0;

                if (this.scrollIsComputed)
                    mScrollY = this.ComputedScrollY;

                rawY = mHeaderPH.Top - Math.Min(mCachedVerticalScrollRange - this.Height, mScrollY);

                switch (mState)
                {
                    case STATE_OFFSCREEN:
                        if (rawY <= mMinRawY)
                            mMinRawY = rawY;
                        else
                            mState = STATE_RETURNING;
                        translationY = rawY;
                        break;

                    case STATE_ONSCREEN:
                        if (rawY < -mQuickReturnHeight)
                        {
                            mState = STATE_OFFSCREEN;
                            mMinRawY = rawY;
                        }
                        translationY = rawY;
                        break;

                    case STATE_RETURNING:
                        if (translationY > 0)
                        {
                            translationY = 0;
                            mMinRawY = rawY - mQuickReturnHeight;
                        }
                        else if (rawY > 0)
                        {
                            mState = STATE_ONSCREEN;
                            translationY = rawY;
                        }
                        else if (translationY < -mQuickReturnHeight)
                        {
                            mState = STATE_OFFSCREEN;
                            mMinRawY = rawY;
                        }
                        else if (!noAnimation)
                        {
                            if (supportTranslateY && quickReturnView.TranslationY != 0)
                            {
                                noAnimation = true;
                                anim = new TranslateAnimation(0, 0, -mQuickReturnHeight, 0);
                                anim.FillAfter = true;
                                anim.Duration = 250;
                                quickReturnView.StartAnimation(anim);
                                anim.AnimationEnd += delegate
                                {
                                    noAnimation = false;
                                    mMinRawY = rawY;
                                    mState = STATE_EXPANDED;
                                };
                            }
                        }

                        break;

                    case STATE_EXPANDED:
                        if (rawY < mMinRawY - 2 && !noAnimation)
                        {
                            noAnimation = true;
                            anim = new TranslateAnimation(0, 0, 0, -mQuickReturnHeight);
                            anim.FillAfter = true;
                            anim.Duration = 250;
                            anim.AnimationEnd += delegate
                            {
                                noAnimation = false;
                                mState = STATE_OFFSCREEN;
                            };
                            quickReturnView.StartAnimation(anim);
                        }
                        else if (translationY > 0)
                        {
                            translationY = 0;
                            mMinRawY = rawY - mQuickReturnHeight;
                        }
                        else if (rawY > 0)
                        {
                            mState = STATE_ONSCREEN;
                            translationY = rawY;
                        }
                        else if (translationY < -mQuickReturnHeight)
                        {
                            mState = STATE_OFFSCREEN;
                            mMinRawY = rawY;
                        }
                        else
                        {
                            mMinRawY = rawY;
                        }
                        break;
                }

                if (supportTranslateY)
                    quickReturnView.TranslationY = translationY;
                else
                {
                    anim = new TranslateAnimation(0, 0, translationY, translationY);
                    anim.FillAfter = true;
                    anim.Duration = 0;
                    quickReturnView.StartAnimation(anim);
                }
            };
        }

        protected virtual void computeScrollY()
        {
            mHeight = 0;
            if (Adapter == null)
                return;
            
            mItemCount = Adapter.Count;
            if (mItemOffsetY == null)
                mItemOffsetY = new int[mItemCount];
            for (int i = 0; i < mItemCount; ++i)
            {
                View view = Adapter.GetView(i, null, this);
                view.Measure(MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                mItemOffsetY[i] = mHeight;
                mHeight += view.MeasuredHeight;
                Console.WriteLine(mHeight);
            }
            scrollIsComputed = true;
            mCachedVerticalScrollRange = mHeight;
        }

        protected virtual int ComputedScrollY
        {
            get
            {
                int pos, nScrollY, nItemY;
                View view = null;
                pos = FirstVisiblePosition;
                view = GetChildAt(0);
                nItemY = view.Top;
                nScrollY = mItemOffsetY[pos] - nItemY;
                return nScrollY;
            }
        }
    }
}