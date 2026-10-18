using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PortalVideojuegos
{
    public partial class Login : Window
    {
        // Almacén de credenciales en memoria: [usuario, contraseña]
        private static readonly string[,] usuarios =
        {
            { "admin", "1234" },
            { "ana",   "abcd" },
            { "luis",  "pass" }
        };

        // static para que no se pierdan al cerrar sesión y abrir otro Login
        private static readonly Dictionary<string, int> intentosFallidos = new();
        private static readonly Dictionary<string, DateTime> bloqueados = new();
        private const int MaxIntentos = 3;
        private const int SegundosBloqueo = 30;

        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = TxtUsuario.Text.Trim();
            string password = TxtPassword.Password;

            // TC04: campos vacíos
            if (usuario == "" || password == "")
            {
                MostrarError("Introduzca usuario y contraseña.");
                return;
            }

            // Buscar el usuario en la lista
            int fila = -1;
            for (int i = 0; i < usuarios.GetLength(0); i++)
            {
                if (usuarios[i, 0] == usuario) { fila = i; break; }
            }

            // TC03: usuario inexistente
            if (fila == -1)
            {
                MostrarError("El usuario introducido no existe.");
                return;
            }

            // TC05: comprobar si está bloqueado
            if (bloqueados.TryGetValue(usuario, out DateTime hasta))
            {
                if (DateTime.Now < hasta)
                {
                    int restantes = (int)(hasta - DateTime.Now).TotalSeconds + 1;
                    MostrarError($"Usuario bloqueado temporalmente. Inténtelo en {restantes} s.");
                    return;
                }
                bloqueados.Remove(usuario);
                intentosFallidos[usuario] = 0;
            }

            // TC02: contraseña incorrecta
            if (usuarios[fila, 1] != password)
            {
                intentosFallidos.TryGetValue(usuario, out int n);
                n++;
                intentosFallidos[usuario] = n;

                if (n >= MaxIntentos)
                {
                    bloqueados[usuario] = DateTime.Now.AddSeconds(SegundosBloqueo);
                    MostrarError($"Contraseña incorrecta 3 veces. Usuario bloqueado {SegundosBloqueo} segundos.");
                }
                else
                {
                    MostrarError($"La contraseña introducida no es correcta. Intentos: {n}/{MaxIntentos}");
                }
                return;
            }

            // TC01: login correcto
            intentosFallidos[usuario] = 0;
            LblMensaje.Foreground = Brushes.Green;
            LblMensaje.Text = "Acceso correcto.";

            MainWindow principal = new MainWindow(usuario);
            principal.Show();
            this.Close();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // TC07: cierra toda la aplicación
            Application.Current.Shutdown();
        }

        private void MostrarError(string mensaje)
        {
            LblMensaje.Foreground = Brushes.Red;
            LblMensaje.Text = mensaje;
            MessageBox.Show(mensaje, "Error de inicio de sesión",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}