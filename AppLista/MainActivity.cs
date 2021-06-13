using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Linq;
using Android.Content;
using System.Threading.Tasks;
using Android.Graphics;

namespace AppLista
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        //AppCompat
        Android.App.ProgressDialog progress;
        string elementoimagen, elementoimagenfondo;
        ListView listado;
        List<Clientes> ListadeClientes = new List<Clientes>();
        List<ElementosdelaTabla> ElementosTabla = new List<ElementosdelaTabla>();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            //SupportActionBar.Hide();
            listado = FindViewById<ListView>(Resource.Id.Lista);
            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Cargando datos de Azure...");
            progress.SetCancelable(false);
            progress.Show();
            await CargarDatosAzure();
            progress.Hide();
        }

        public async Task CargarDatosAzure()
        {
            try
            {
                var typeface = Typeface.CreateFromAsset(this.Assets, "fonts/FLATS.ttf");
                var Titulo = FindViewById<TextView>(Resource.Id.Titulo);
                Titulo.SetTypeface(typeface, TypefaceStyle.Normal);
                var CuentadeAlmacenamiento = CloudStorageAccount.Parse
                    ("DefaultEndpointsProtocol=https;AccountName=programacionmoviles;AccountKey=K4HLGMkMGB87LlncsykIQe5QO85Ges6DDZ1wjK8M7EFpZeR+k+7fKLm3uy3th+R6mvmYeDa6pf2sn62Q3dZkWg==;EndpointSuffix=core.windows.net");
                var ClienteBlob = CuentadeAlmacenamiento.CreateCloudBlobClient();
                var Contenedor = ClienteBlob.GetContainerReference("oscar");
                var TablaNoSQL = CuentadeAlmacenamiento.CreateCloudTableClient();
                var Tabla = TablaNoSQL.GetTableReference("oscar");
                var Consulta = new TableQuery<Clientes>();
                TableContinuationToken token = null;
                var Datos = await Tabla.ExecuteQuerySegmentedAsync<Clientes>
                    (Consulta, token, null, null);
                ListadeClientes.AddRange(Datos.Results);
                int iCorreo = 0;
                int iNombre = 0;
                int iImagen = 0;
                int iDomicilio = 0;
                int iEdad = 0;
                int iSaldo = 0;
                int iLatitud= 0;
                int iLongitud = 0;
                int iImagenFondo = 0;
                ElementosTabla = ListadeClientes.Select(r => new ElementosdelaTabla()
                {
                    Correo = ListadeClientes.ElementAt(iCorreo++).Correo,
                    Nombre = ListadeClientes.ElementAt(iNombre++).RowKey,
                    Imagen = ListadeClientes.ElementAt(iImagen++).Imagen,
                    Domicilio = ListadeClientes.ElementAt(iDomicilio++).Domicilio,
                    Edad = ListadeClientes.ElementAt(iEdad++).Edad,
                    Saldo = ListadeClientes.ElementAt(iSaldo++).Saldo,
                    Longitud = ListadeClientes.ElementAt(iLongitud++).Longitud,
                    Latitud = ListadeClientes.ElementAt(iLatitud++).Latitud,
                    ImagenFondo = ListadeClientes.ElementAt(iImagenFondo++).ImagenFondo
                }).ToList();
                int contadorimagen = 0;
                while (contadorimagen< ListadeClientes.Count)
                {
                    elementoimagen = ListadeClientes.ElementAt(contadorimagen).Imagen;
                    elementoimagenfondo = ListadeClientes.ElementAt(contadorimagen).ImagenFondo;
                    var ImagenBlob = Contenedor.GetBlockBlobReference(elementoimagen);
                    var ImagenFondoBlob = Contenedor.GetBlockBlobReference(elementoimagenfondo);
                    var rutaimagen = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var rutaimagenfondo = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var ArchivoImagen = System.IO.Path.Combine(rutaimagen, elementoimagen);
                    var ArchivoImagenFondo = System.IO.Path.Combine(rutaimagenfondo, elementoimagenfondo);
                    var StreamImagen = File.OpenWrite(ArchivoImagen);
                    var StreamImagenFondo = File.OpenWrite(ArchivoImagenFondo);
                    await ImagenBlob.DownloadToStreamAsync(StreamImagen);
                    await ImagenFondoBlob.DownloadToStreamAsync(StreamImagenFondo);
                    contadorimagen++;

                }
                Toast.MakeText(this, "Imagenes descargadas", ToastLength.Long).Show();
                listado.Adapter = new DataAdapter(this, ElementosTabla);
                listado.ItemClick += OnListItemClick;

            }
            catch(System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

        }
        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var Datasend = ElementosTabla[e.Position];
            var DataIntent = new Intent(this, typeof(DataDetailActivity));
            DataIntent.PutExtra("correo", Datasend.Correo);
            DataIntent.PutExtra("imagen", Datasend.Imagen); 
            DataIntent.PutExtra("imagenfondo", Datasend.ImagenFondo);
            DataIntent.PutExtra("nombre", Datasend.Nombre);
            DataIntent.PutExtra("edad", Datasend.Edad.ToString());
            DataIntent.PutExtra("saldo", Datasend.Saldo.ToString());
            DataIntent.PutExtra("domicilio", Datasend.Domicilio);
            DataIntent.PutExtra("latitud", Datasend.Latitud.ToString());
            DataIntent.PutExtra("longitud", Datasend.Longitud.ToString());
            StartActivity(DataIntent);
        }

        

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
    public class ElementosdelaTabla
    {

        public string Nombre { get; set; }
        public string Domicilio { get; set; }
        public int Edad { get; set; }
        public string ImagenFondo { get; set; }
        public string Imagen { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Correo { get; set; }
        public double Saldo { get; set; }
    }

    public class Clientes : TableEntity
    {
        public Clientes(string Categoria, string Nombre)
        {
            PartitionKey = Categoria;
            RowKey = Nombre;

        }

        public Clientes() { }

        public string Domicilio { get; set; }
        public int Edad { get; set; }
        public string ImagenFondo { get; set; }
        public string Imagen { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Correo { get; set; }
        public double Saldo { get; set; }
    }
}