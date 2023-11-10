using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace RemaTe.DataAccess;

public class DBC {
  private static DBC s_instance;
  public static DBC I {
    get {
      s_instance ??= new DBC();
      return s_instance;
    }
  }

  public SQLiteConnection? DbConn { get; private set; }

  public static async IAsyncEnumerable<object[]> Exec(string command) {
    using SQLiteCommand cmd = new(DBC.I.DbConn);
    cmd.CommandText = command;

    var reader = await cmd.ExecuteReaderAsync();
    int columns = reader.FieldCount;

    object[] args;
    while (reader.Read()) {
      args = new object[columns];
      for (int i = 0; i < columns; i++) {
        args[i] = reader.GetValue(i);
      };
      yield return args;
    }
  }

  public DBC() {
    // For development reasons we create a new SQLite file on every run.
    if (!File.Exists(Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "/remate.sqlite")) {
      InitDB();
    }

    // Open the SQLite file, on production this shoud be a connection to MySQL.
    string connectionString = "Data Source=remate.sqlite;";
    DbConn = new SQLiteConnection(connectionString);
    DbConn.Open();
  }

  private static void InitDB() {
    SQLiteConnection.CreateFile(databaseFileName: Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "/remate.sqlite");

    // Open the SQLite file, on production this shoud be a connection to MySQL.
    string connectionString = "Data Source=remate.sqlite;";
    var dbConn = new SQLiteConnection(connectionString);
    dbConn.Open();

    var command = new SQLiteCommand("", dbConn);
    command.CommandText = @"
      DROP TABLE IF EXISTS departamento;
      CREATE TABLE departamento (
      id      INTEGER,
      nombre  VARCHAR,
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS usuario;
      CREATE TABLE usuario (
      id             INTEGER,
      hash_pwd       VARCHAR,
      permisos       INTEGER,
      nombre         VARCHAR,
      email          VARCHAR,
      telefono       INTEGER,
      departamento   INTEGER,
      localidad      INTEGER,
      calle          VARCHAR,
      puerta         INTEGER,
      FOREIGN KEY (departamento) REFERENCES departamento(id),
      FOREIGN KEY (localidad) REFERENCES localidad(id),
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS empleado;
      CREATE TABLE empleado (
      ci          INTEGER,
      FOREIGN KEY (ci) REFERENCES usuario(ci),
      PRIMARY KEY (ci)
      );
      DROP TABLE IF EXISTS tarea;
      CREATE TABLE tarea (
      id          INTEGER,
      ci_empleado INTEGER,
      descripcion VARCHAR,
      FOREIGN KEY (ci_empleado) REFERENCES empleado(ci)
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS cliente;
      CREATE TABLE cliente (
      ci              INTEGER,
      FOREIGN KEY (ci) REFERENCES usuario(ci),
      PRIMARY KEY (ci)
      );
      DROP TABLE IF EXISTS articulo;
      CREATE TABLE articulo (
      id              INTEGER,
      nombre          VARCHAR,
      cantidad        INTEGER,
      descripcion     VARCHAR,
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS pago;
      CREATE TABLE pago (
      id      INTEGER,
      ci      INTEGER,
      monto   INTEGER,
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS lote;
      CREATE TABLE lote (
      id           INTEGER,
      nombre       VARCHAR,
      precio_base  INTEGER,
      comision     INTEGER,
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS remate;
      CREATE TABLE remate (
      id              INTEGER,
      inicio          INTEGER,
      duracion        INTEGER,
      tipo            INTEGER,
      metodos_pago    INTEGER,
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS imagenes;
      CREATE TABLE imagenes (
      id_imagen   INTEGER,
      id_articulo INTEGER,
      imagen      VARCHAR,
      FOREIGN KEY (id_articulo) REFERENCES articulo(id)
      PRIMARY KEY (id_imagen)
      );
      DROP TABLE IF EXISTS integra;
      CREATE TABLE integra (
      id_lote     INTEGER,
      id_remate   INTEGER,
      fecha       INTEGER,
      FOREIGN KEY (id_lote)   REFERENCES lote(id)
      FOREIGN KEY (id_remate) REFERENCES remate(id)
      PRIMARY KEY (id_lote, id_remate)
      );
      DROP TABLE IF EXISTS pertenece;
      CREATE TABLE pertenece (
      id_articulo     INTEGER,
      id_lote         INTEGER,
      FOREIGN KEY (id_articulo)   REFERENCES articulo(id)
      FOREIGN KEY (id_lote)       REFERENCES lote(id)
      PRIMARY KEY (id_articulo, id_lote)
      );
      DROP TABLE IF EXISTS puja;
      CREATE TABLE puja (
      ci_cliente      INTEGER,
      id_lote_remate  INTEGER,
      monto           INTEGER,
      momento         INTEGER,
      FOREIGN KEY (ci_cliente) REFERENCES cliente(ci)
      FOREIGN KEY (id_lote_remate) REFERENCES lote_remate(id)
      PRIMARY KEY (ci_cliente, id_lote_remate)
      );
      DROP TABLE IF EXISTS vende;
      CREATE TABLE vende (
      ci_cliente  INTEGER,
      id_articulo INTEGER,
      monto       INTEGER,
      fecha       INTEGER,
      FOREIGN KEY (ci_cliente) REFERENCES cliente(ci)
      FOREIGN KEY (id_articulo) REFERENCES articulo(id)
      PRIMARY KEY (fecha)
      );
      DROP TABLE IF EXISTS propiedades;
      CREATE TABLE propiedades (
      nombre      VARCHAR,
      PRIMARY KEY (nombre)
      );
      DROP TABLE IF EXISTS otro;
      CREATE TABLE otro (
      id_articulo INTEGER,
      FOREIGN KEY (id_articulo) REFERENCES articulo(id),
      PRIMARY KEY (id_articulo)
      );
      DROP TABLE IF EXISTS animal;
      CREATE TABLE animal (
      id_articulo INTEGER,
      tipo        VARCHAR,
      raza        VARCHAR,
      nacimiento  INTEGER,
      FOREIGN KEY (id_articulo) REFERENCES articulo(id),
      PRIMARY KEY (id_articulo)
      );
      DROP TABLE IF EXISTS maquinaria;
      CREATE TABLE maquinaria (
      id_articulo INTEGER,
      marca       VARCHAR,
      modelo      VARCHAR,
      año         INTEGER,
      FOREIGN KEY (id_articulo) REFERENCES articulo(id),
      PRIMARY KEY (id_articulo)
      );
      DROP TABLE IF EXISTS tiene;
      CREATE TABLE tiene (
      id_otro           INTEGER,
      nombre_propiedad  VARCHAR,
      valor             VARCHAR,
      FOREIGN KEY (id_otro) REFERENCES otro(id_articulo),
      FOREIGN KEY (nombre_propiedad) REFERENCES propiedad(nombre),
      PRIMARY KEY (id_otro, nombre_propiedad)
      );
      DROP TABLE IF EXISTS atesora;
      CREATE TABLE atesora (
      id_animal         INTEGER,
      nombre_propiedad  VARCHAR,
      valor             VARCHAR,
      FOREIGN KEY (id_animal) REFERENCES animal(id_articulo),
      FOREIGN KEY (nombre_propiedad) REFERENCES propiedad(nombre),
      PRIMARY KEY (id_animal, nombre_propiedad)
      );
      DROP TABLE IF EXISTS posee;
      CREATE TABLE posee (
      id_maquinaria     INTEGER,
      nombre_propiedad  VARCHAR,
      valor             VARCHAR,
      FOREIGN KEY (id_maquinaria) REFERENCES maquinaria(id_articulo),
      FOREIGN KEY (nombre_propiedad) REFERENCES propiedad(nombre),
      PRIMARY KEY (id_maquinaria, nombre_propiedad)
      );
    ";
    command.ExecuteNonQuery();

    using SQLiteTransaction transaction = dbConn.BeginTransaction();
    command = new SQLiteCommand("", dbConn, transaction);

    command.CommandText = @"INSERT INTO departamento (id, nombre) VALUES
        (0, 'Treinta y tres'),
        (1, 'Tacuarembo'),
        (2, 'Soriano'),
        (3, 'San Jose'),
        (4, 'Rocha'),
        (5, 'Rivera'),
        (6, 'Rio Negro'),
        (7, 'Paysandu'),
        (8, 'Montevideo'),
        (9, 'Maldonado'),
        (10, 'Lavalleja'),
        (11, 'Florida'),
        (12, 'Flores'),
        (13, 'Durazno'),
        (14, 'Colonia'),
        (15, 'Cerro Largo'),
        (16, 'Canelones'),
        (17, 'Artigas'),
        (18, 'Salto');
        ";
    command.ExecuteNonQuery();

    command.CommandText = """
      INSERT INTO usuario (
        id, hash_pwd, permisos, nombre, email, telefono, departamento, localidad, calle, puerta
      ) values (
        0, $hash, 4, 'SUPERADMIN', 'nomail@nomail.nodomain', 092000000, 18, 'Topador', 'De los rieles', 83
      );
      """;
    command.Parameters.AddWithValue("$hash", "9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256");
    command.ExecuteNonQuery();

    command.CommandText = @"
      INSERT INTO usuario (
      id, hash_pwd, permisos, nombre, email, telefono,
      departamento, localidad, calle, puerta)
      VALUES
      (45629664, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 4, 'Charlie Davis', 'charliedavis@example.com', 092111111, 19, 'SALTO', 'Calle Los Sauces', 222),
      (35769357, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 3, 'John Doe', 'johndoe@example.com', 092123456, 18, 'ARTIGAS', '18 de Julio', 123),
      (26437530, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 2, 'Jane Smith', 'janesmith@example.com', 987654321, 18, 'JAVIER DE VIANA', 'Calle del Sol', 456),
      (18563964, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 1, 'Bob Johnson', 'bobjohnson@example.com', 555555555, 18, 'TOMAS GOMENSORO', 'Calle de los Rosales', 789);

      INSERT INTO articulo (id, nombre, cantidad, descripcion)
      VALUES
      (0, 'Cosas', 4, 'Descripcion inspiradora'),
      (1, 'Vacas', 150, 'Descripcion inspiradora'),
      (2, 'Cosechadora', 1, 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum sapien massa, convallis a, lacinia a, aliquam id, risus. Cras ultricies ligula sed magna dictum porta. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quis luctus tortor, vestibulum at, dignissim ac, adipiscing nec, diam. Sed lectus. In est risus, auctor et, tristique in, tempus et, pede.');

      INSERT INTO otro (id_articulo)
      VALUES (0);

      INSERT INTO animal (id_articulo, tipo, raza, nacimiento)
      VALUES (1, 'Vacas', 'Hereford', 1699376275763);

      INSERT INTO maquinaria (id_articulo, marca, modelo, año)
      VALUES (2, 'John Deere', '2266 Extra', 2010);

      -- Cliente Table --
      INSERT INTO cliente (ci)
      VALUES (26437530), (18563964);

      -- Empleado Table --
      INSERT INTO empleado (ci)
      VALUES (45629664), (35769357);

      -- Tarea Table --
      INSERT INTO tarea (id, ci_empleado, descripcion)
      VALUES
      (1, 45629664,
      'Crear una nueva tabla en la base de datos'),
      (2, 45629664,
      'Actualizar el código fuente para que funcione con
      la nueva tabla'),
      (3, 45629664, 'Probar las nuevas características implementadas');

      -- Pago Table --
      INSERT INTO pago (id, ci, monto)
      VALUES
      (1, 45629664, 10000), (2, 45629664, 5000), (3, 45629664, 7500);

      -- Lote Table --
      INSERT INTO lote (id, nombre, precio_base, comision)
      VALUES
      (0, 'Variedad', 599, 100),
      (1, 'Mas Variedad', 699, 50),
      (2, 'Variedad ultra', 799, 6);

      -- Remate Table --
      INSERT INTO remate (id, inicio, duracion, tipo, metodos_pago)
      VALUES
      (0, 1000000, 10, 0, 1),
      (1, 2000000, 15, 0, 2),
      (2, 3000000, 20, 0, 3);

      -- Imagenes Table --
      INSERT INTO imagenes (id_articulo, id_imagen, imagen)
      VALUES
      (0, 1, '/path/to/image01.jpg'),
      (0, 2, '/path/to/image02.png'),
      (0, 3, '/path/to/image03.jpeg'),
      (1, 4, '/path/to/image11.jpg'),
      (1, 5, '/path/to/image12.png'),
      (1, 6, '/path/to/image13.jpeg');

      -- Lote_Remate Table --
      INSERT INTO integra (id_lote, id_remate, fecha)
      VALUES (1, 0, 1000000), (2, 1, 2000000), (3, 2, 3000000);

      -- Articulo_Lote Table --
      INSERT INTO pertenece (id_articulo, id_lote)
      VALUES (0, 1), (0, 2), (0, 3);

      -- Cliente_Puja Table --
      INSERT INTO puja (ci_cliente, id_lote_remate, monto, momento)
      VALUES (26437530, 2, 1500, 200000), (18563964, 3, 2000, 300000);

      -- Cliente_Articulo Table --
      INSERT INTO vende (ci_cliente, id_articulo, monto, fecha)
      VALUES (26437530, 0, 750, 200000), (26437530, 1, 1000, 300000);
    ";
    command.ExecuteNonQuery();
    transaction.Commit();

    dbConn.Close();
  }
  ~DBC() {
    DbConn?.Close();
  }
}

