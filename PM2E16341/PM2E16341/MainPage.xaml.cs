using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using PM2E16341.Models;
using Plugin.Media;
using System.Threading;
using System.IO;

namespace PM2E16341
{
    public partial class MainPage : ContentPage
    {

        byte[] guardarfoto;
        public MainPage()
        {
            InitializeComponent();
            LongitudLatitud();

            Descripcion.Text = "";
            imgubicacion.Source = null;
        }

        public async void LongitudLatitud()
        {
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

                var tokendecancelacion = new CancellationTokenSource();

                var localizacion = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);


                if (localizacion != null)
                {
                    Latitud.Text = localizacion.Latitude.ToString();
                    Longitud.Text = localizacion.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Mensaje", "Este Dispositivo NO Soporta La Función GPS", "Ok");
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Mensaje", "El GPS no esta activo\nPorfavor Encienda el GPS y reinicie la app", "Ok");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Mensaje", "Sin Permisos Para Uso De La Geolocalizacion", "Ok");
            }
            catch (Exception)
            {
                await DisplayAlert("Mensaje", "Sin Ubicacion", "Ok");
            }
        }


        private async void btntomarfoto_Clicked(object sender, EventArgs e)
        {
            var tomarfoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "PhotoLocationApp",
                Name = DateTime.Now.ToString() + "_Photo.jpg",
                SaveToAlbum = true
            });

            if (tomarfoto != null)
            {
                guardarfoto = null;
                MemoryStream memoryStream = new MemoryStream();

                tomarfoto.GetStream().CopyTo(memoryStream);
                guardarfoto = memoryStream.ToArray();

                imgubicacion.Source = ImageSource.FromStream(() => { return tomarfoto.GetStream(); });
            }
        }
        private async Task validarCampos()
        {

            if (String.IsNullOrWhiteSpace(Longitud.Text) || String.IsNullOrWhiteSpace(Latitud.Text) || String.IsNullOrWhiteSpace(Descripcion.Text) || imgubicacion.Source == null)
            {
                await this.DisplayAlert("Aviso", "TODOS LOS CAMPOS SON OBLIGATORIOS", "OK");
            }

        }
        private async void btnguardar_Clicked(object sender, EventArgs e)
        {
            if (validarCampos().IsCompleted)
            {
                try
                {
                    var ubicacion = new Ubicaciones
                    {
                        Imagen = guardarfoto,
                        Longitud = (float)Convert.ToDouble(Longitud.Text),
                        Latitud = (float)Convert.ToDouble(Latitud.Text),
                        Descripcion = Descripcion.Text
                    };

                    var resultado = await App.BaseDatos.GuardarUbicacion(ubicacion);

                    if (resultado != 0)
                    {
                        await DisplayAlert("Aviso", "Ubicacion Guardada Exitosamente", "Ok");
                        LimpiarCampos();
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "ERROR", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message.ToString(), "Ok");
                }
            }

        }

        private void LimpiarCampos()
        {
            Longitud.Text = "";
            Latitud.Text = "";
            Descripcion.Text = "";
            imgubicacion.Source = null;
        }

        private void btnsalir_Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private async void btnlista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PaginaLocalizacion());
        }
    }

}
