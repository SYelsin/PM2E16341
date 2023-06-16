using PM2E16341.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E16341
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaLocalizacion : ContentPage
    {
        public PaginaLocalizacion()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var listaubicaciones = await App.BaseDatos.ListaUbicaciones();

            ObservableCollection<Ubicaciones> observableCollectionPhotos = new ObservableCollection<Ubicaciones>();
            ListaUbicaciones.ItemsSource = observableCollectionPhotos;
            foreach (Ubicaciones img in listaubicaciones)
            {
                observableCollectionPhotos.Add(img);
            }
        }

        private async void ListaUbicaciones_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Ubicaciones item = (Ubicaciones)e.Item;

            string actionSheetTitle = "Seleccione una opción";
            string cancelText = "Cancelar";
            string deleteText = "Borrar Ubicacion";
            string goToLocationText = "Ir a la Ubicacion";

            if (Device.RuntimePlatform == Device.iOS)
            {
                // Agregar iconos para iOS
                actionSheetTitle = null;
                cancelText = "Cancelar";
                deleteText = "\U0001F5D1 Borrar Ubicación"; // Icono de basura
                goToLocationText = "\U0001F4CD Ir a la Ubicación"; // Icono de ubicación
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                // Agregar iconos para Android
                actionSheetTitle = null;
                cancelText = "Cancelar";
                deleteText = "\U0001F5D1 Borrar Ubicación"; 
                goToLocationText = "\U0001F4CD Ir a la Ubicación"; 
            }

            var result = await DisplayActionSheet(actionSheetTitle, cancelText, null, goToLocationText, deleteText);

            if (result == goToLocationText)
            {
                var Nuevom = new Mapa();
                Nuevom.imgcap = item.Imagen;
                Nuevom.BindingContext = item;
                await Navigation.PushAsync(Nuevom);
            }
            else if (result == deleteText)
            {
                var confirm = await DisplayAlert("Confirmar eliminación", "¿Está seguro de que desea eliminar esta ubicación?", "Sí", "No");

                if (confirm)
                {
                    var resultado = await App.BaseDatos.EliminarUbicacion(item);

                    if (resultado != 0)
                    {
                        await DisplayAlert("Aviso", "Sitio eliminado exitosamente", "Ok");
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "Ha ocurrido un error", "Ok");
                    }

                    await Navigation.PopAsync();
                }
            }
        }
    }
}
