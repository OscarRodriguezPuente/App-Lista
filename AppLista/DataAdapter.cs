using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Graphics;
using System.Collections.Generic;
using AndroidX.Core.Graphics;
using AndroidX.Core.Graphics.Drawable;

namespace AppLista
{
    public class DataAdapter : BaseAdapter<ElementosdelaTabla>
    {
        List<ElementosdelaTabla> items;
        Activity context;

        public DataAdapter(Activity context, List<ElementosdelaTabla> items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ElementosdelaTabla this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            view = context.LayoutInflater.Inflate(Resource.Layout.DataRow, null);
            view.FindViewById<TextView>(Resource.Id.txtcorreo).Text = item.Correo;
            var txtHead = view.FindViewById<TextView>(Resource.Id.txtnombre);
            txtHead.Text = item.Nombre;
            var typeface = Typeface.CreateFromAsset(context.Assets, "fonts/FLATS.ttf");
            txtHead.SetTypeface(typeface, TypefaceStyle.Normal);
            var path = System.IO.Path.Combine(System.Environment.GetFolderPath
            (System.Environment.SpecialFolder.Personal), item.Imagen);
            var pathback = System.IO.Path.Combine(System.Environment.GetFolderPath
            (System.Environment.SpecialFolder.Personal), item.ImagenFondo);
            var Image = BitmapFactory.DecodeFile(path);
            var ImageBack = BitmapFactory.DecodeFile(pathback);
            Image = ResizeBitMap(Image, 100, 100);
            view.FindViewById<ImageView>(Resource.Id.imagen).
            SetImageDrawable(getRoundedCornerImage(Image, 5));
            view.FindViewById<ImageView>(Resource.Id.imagenfondo).
            SetImageDrawable(getRoundedCornerImage(ImageBack, 2));
            return view;
        }

        public static RoundedBitmapDrawable

            getRoundedCornerImage
            (Bitmap image, int cornerRadius)
        {
            var corner =
                RoundedBitmapDrawableFactory.Create(null, image);
            corner.CornerRadius = cornerRadius;
            return corner;
        }

        private Bitmap ResizeBitMap(Bitmap imageoriginal, int widthimageoriginal, int heightimageoriginal)
        {
            Bitmap resizedImage = Bitmap.CreateBitmap(widthimageoriginal, heightimageoriginal, Bitmap.Config.Argb8888);
            float Width = imageoriginal.Width;
            float Height = imageoriginal.Height;
            var canvas = new Canvas(resizedImage);
            var scala = widthimageoriginal / Width;
            var xTranslation = 0.0f;
            var yTranslation = (heightimageoriginal - Height * scala) / 2.0f;
            var transformacion = new Matrix();
            transformacion.PostTranslate(xTranslation, yTranslation);
            transformacion.PreScale(scala, scala);
            var paint = new Paint();
            paint.FilterBitmap = true;
            canvas.DrawBitmap(imageoriginal, transformacion, paint);
            return resizedImage;
        }
    }
}
