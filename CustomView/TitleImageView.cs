using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CustomView
{
    class TitleImageView : View
    {
        private Bitmap image;
        private ImageScale imageScaleType;
        private string titleText;
        private Color titleTextColor;
        private int titleTextSize;

        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                Invalidate();
                RequestLayout();
            }
        }

        public ImageScale ImageScaleType
        {
            get
            {
                return imageScaleType;
            }
            set
            {
                imageScaleType = value;
                Invalidate();
            }
        }

        public string TitleText
        {
            get
            {
                return titleText;
            }
            set
            {
                titleText = value;
                Invalidate();
                RequestLayout();
            }
        }

        public Color TitleTextColor
        {
            get
            {
                return titleTextColor;
            }
            set
            {
                titleTextColor = value;
                Invalidate();
            }
        }

        public int TitleTextSize
        {
            get
            {
                return titleTextSize;
            }
            set
            {
                titleTextSize = value;
                Invalidate();
                RequestLayout();
            }
        }

        public enum ImageScale
        {
            FillXY,
            Center
        }

        public TitleImageView(Context context) : this(context, null)
        {
        }

        public TitleImageView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public TitleImageView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            image = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Icon);

            TypedArray typedArray = Context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.TitleImageView, defStyle, 0);
            int count = typedArray.IndexCount;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    int index = typedArray.GetIndex(i);
                    switch (index)
                    {
                        case Resource.Styleable.TitleImageView_image:
                            image = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Icon);
                           //image = BitmapFactory.DecodeResource(Resources, typedArray.GetResourceId(index, Resource.Drawable.Icon));
                            break;
                        case Resource.Styleable.TitleImageView_imageScaleType:
                            imageScaleType = (ImageScale)typedArray.GetInt(index, 0);
                            break;
                        case Resource.Styleable.TitleImageView_titleText:
                            titleText = typedArray.GetString(index);
                            break;
                        case Resource.Styleable.TitleImageView_titleTextColor:
                            titleTextColor = typedArray.GetColor(index, Color.Black);
                            break;
                        case Resource.Styleable.TitleImageView_titleTextSize:
                            //获取尺寸三个方法的介绍：http://my.oschina.net/ldhy/blog/496420
                            titleTextSize = typedArray.GetDimensionPixelSize(index, (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, 16, Resources.DisplayMetrics));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                typedArray.Recycle();
            }

            Init();

        }

        Rect rect;
        Paint paint;
        Rect textBound;
        public void Init()
        {
            rect = new Rect(); //图片位置
            paint = new Paint();
            paint.TextSize = TitleTextSize;
            paint.Color = titleTextColor;
            textBound = new Rect();//底部说明文字位置  
                                   // 计算了描绘字体需要的范围  
            paint.GetTextBounds(titleText, 0, titleText.Length, textBound);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            //计算宽度 以图片宽度作控件宽度
            int minWidth = PaddingLeft + PaddingRight + image.Width;
            var width = ResolveSizeAndState2(minWidth, widthMeasureSpec, 0);

            //计算高度
            int minHeight = PaddingBottom + PaddingTop + image.Height + textBound.Height();
            var height = ResolveSizeAndState2(minHeight, heightMeasureSpec, 0);

            // 测量完成后必须调用setMeasuredDimension方法
            SetMeasuredDimension(width, height);
        }

        private int ResolveSizeAndState2(int size, int measureSpec, int childMeasuredState)
        {
            int result = size;
            var specMode = MeasureSpec.GetMode(measureSpec);
            var specSize = MeasureSpec.GetSize(measureSpec);
            switch (specMode)
            {
                case MeasureSpecMode.Unspecified:
                    result = size;
                    break;
                case MeasureSpecMode.AtMost:
                    if (specSize < size)
                    {
                        result = specSize | View.MeasuredStateTooSmall;
                    }
                    else
                    {
                        result = size;
                    }
                    break;
                case MeasureSpecMode.Exactly:
                    result = specSize;
                    break;
            }
            return result | (childMeasuredState & View.MeasuredStateMask);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            rect.Left = PaddingLeft;
            rect.Right = Width - PaddingRight;
            rect.Top = PaddingTop;
            rect.Bottom = Height - PaddingBottom;

            paint.TextSize = TitleTextSize;
            paint.Color = titleTextColor;
            paint.SetStyle(Paint.Style.Fill);

            //当前设置的宽度小于字体需要的宽度，将字体改为xxx... 
            if (textBound.Width() > Width)
            {
                TextPaint paint = new TextPaint(this.paint);
                string msg = TextUtils.Ellipsize(titleText, paint, (float)Width - PaddingLeft - PaddingRight, TextUtils.TruncateAt.End);
                canvas.DrawText(msg, PaddingLeft, Height - PaddingBottom, paint);
            }
            else
            {
                canvas.DrawText(titleText, Width / 2 - textBound.Width() / 2, Height - PaddingBottom, paint);
            }

            //取消使用掉的部分  
            rect.Bottom -= textBound.Height();

            if (imageScaleType == ImageScale.FillXY)
            {
                canvas.DrawBitmap(image, null, rect, paint);
            }
            else
            {
                rect.Left = Width / 2 - image.Width / 2;
                rect.Right = Width / 2 + image.Width / 2;
                rect.Top = (Height - textBound.Height()) / 2 - image.Height / 2;
                rect.Bottom = (Height - textBound.Height()) / 2 + image.Height / 2;

                canvas.DrawBitmap(image, null, rect, paint);
            }
        }

    }
}


