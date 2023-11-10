using System;

using RemaTe.Shared.Models;

// using static RemaTe.GNOME.Helpers.Gettext;

namespace RemaTe.Shared.Controllers;

/// <summary>
/// A controller for a MainWindow
/// </summary>
public class MainController {

    /// <summary>
    /// Gets the AppInfo object
    /// </summary>
    public static AppInfo AppInfo => AppInfo.Current;
    /// <summary>
    /// Whether or not the version is a development version or not
    /// </summary>
    public static bool IsDevVersion => AppInfo.Current.Version.IndexOf('-') != -1;

    /*
        /// <summary>
        /// In production this method shoud open the connection to MySQL.
        /// </summary>
        public async Task<string> LogIn(string email, string pwd) {
            /*
            UserCreation usrReq = new(email, pwd);
            var responseUser = await client.PutAsJsonAsync("http://localhost:5184/users/login", usrReq);

            if (responseUser.IsSuccessStatusCode)
            {
                Auth = await responseUser.Content.ReadFromJsonAsync<UserAuth>();
                return email;
            }
            else
            {
                return null;
            }
            string connectionString = "Data Source=remate.sqlite;";
            _dbConn = new(connectionString);
            _dbConn.Open();

            SQLiteCommand command = _dbConn.CreateCommand();
            command.CommandText = @"SELECT hash_pwd, permisos FROM usuario WHERE email = $email";
            command.Parameters.AddWithValue("$email", email);

            var reader = await command.ExecuteReaderAsync();

            if (reader.HasRows) {
                reader.Read();
                string hash_pwd = reader.GetString(0);
                bool isValid = Hasher.Verify(pwd, hash_pwd);
                if (isValid) {
                    _auth = new UserAuth(email, reader.GetInt32(1));
                    OnAuth.Invoke();
                    return email;
                }
            }
            return null;
        }
        public void LogOut() {
            _auth = null;
            OnAuth.Invoke();
            _dbConn.Dispose();
        }
    */

    /*
        public async Task<Errors> CerateCliente(ClienteCreation usrReq)
        {
            var responseUser = await client.PostAsJsonAsync("http://localhost:5184/users/cliente", usrReq);

            return responseUser.StatusCode switch
            {
                System.Net.HttpStatusCode.Created => Errors.Ok,
                System.Net.HttpStatusCode.NotFound => Errors.NotFound,
                System.Net.HttpStatusCode.Unauthorized => Errors.Unauthorized,
                _ => throw new Exception($"Respuesta no conocida {responseUser.StatusCode}"),
            };
        }

        public async Task<List<Lote>?> GetLotesAsync() => await client.GetFromJsonAsync<List<Lote>>("http://localhost:5184/lote");

        public async Task<Errors> CreateLote(Lote lote)
        {
            LoteCreation ltReq = new()
            {
                Lote = lote,
                User = _auth
            };

            var responseUser = await client.PostAsJsonAsync($"http://localhost:5184/lote", ltReq);

            return responseUser.StatusCode switch
            {
                System.Net.HttpStatusCode.Created => Errors.Ok,
                System.Net.HttpStatusCode.NotFound => Errors.NotFound,
                System.Net.HttpStatusCode.Unauthorized => Errors.Unauthorized,
                _ => throw new Exception($"Respuesta no conocida {responseUser.StatusCode}"),
            };
        }

        public async Task<Errors> UpdateLote(Lote lote)
        {
            LoteCreation ltReq = new()
            {
                Lote = lote,
                User = _auth
            };

            var responseUser = await client.PutAsJsonAsync($"http://localhost:5184/lote/{lote.Id}", ltReq);

            return responseUser.StatusCode switch
            {
                System.Net.HttpStatusCode.OK => Errors.Ok,
                System.Net.HttpStatusCode.NotFound => Errors.NotFound,
                System.Net.HttpStatusCode.Unauthorized => Errors.Unauthorized,
                _ => throw new Exception($"Respuesta no conocida {responseUser.StatusCode}"),
            };
        }

        public async Task<Errors> DeleteLote(Lote lote)
        {
            var responseUser = await client.PostAsJsonAsync($"http://localhost:5184/lote/delete/{lote.Id}", _auth);
            return responseUser.StatusCode switch
            {
                System.Net.HttpStatusCode.OK => Errors.Ok,
                System.Net.HttpStatusCode.NotFound => Errors.NotFound,
                System.Net.HttpStatusCode.Unauthorized => Errors.Unauthorized,
                _ => throw new Exception($"Respuesta no conocida {responseUser.StatusCode}"),
            };
        }
    */


    /// <summary>
    /// Whether or not to show a sun icon on the home page
    /// </summary>
    public static bool ShowSun {
        get {
            var timeNowHours = DateTime.Now.Hour;
            return timeNowHours >= 6 && timeNowHours < 18;
        }
    }

    /// <summary>
    /// The string for greeting on the home page
    /// </summary>
    public static string Greeting {
        get {
            // return DateTime.Now.Hour switch {
            //     >= 0 and < 6 => P("Night", "Buenos dias!"),
            //     < 12 => P("Morning", "Buenos dias!"),
            //     < 18 => _("Buenas tardes!"),
            //     < 24 => _("Buenas noches!"),
            //     _ => _("Buen dia!")
            // };
            return DateTime.Now.Hour switch {
                >= 0 and < 6 => "Buenos dias!",
                < 12 => "Buenos dias",
                < 18 => "Buenas tardes!",
                < 24 => "Buenas noches!",
                _ => "Buen dia!"
            };
        }
    }
}
