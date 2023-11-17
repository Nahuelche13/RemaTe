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
    // if (!File.Exists(Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "/remate.sqlite")) {
    InitDB();
    // }

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
      is_active      BOOLEAN,
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
      nombre          VARCHAR,
      rematador       VARCHAR,
      inicio          DATETIME,
      duracion        INTEGER,
      tipo            INTEGER,
      metodos_pago    INTEGER,
      PRIMARY KEY (id)
      );
      DROP TABLE IF EXISTS imagenes;
      CREATE TABLE imagenes (
      id_articulo INTEGER,
      id_imagen   INTEGER,
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
      DROP TABLE IF EXISTS compra;
      CREATE TABLE compra (
      ci_cliente  INTEGER,
      id_lote     INTEGER,
      monto       INTEGER,
      FOREIGN KEY (ci_cliente) REFERENCES cliente(ci)
      FOREIGN KEY (id_lote) REFERENCES lote(id)
      PRIMARY KEY (ci_cliente, id_lote)
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
      tipo        INTEGER,
      raza        VARCHAR,
      nacimiento  DATETIME,
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
        id, hash_pwd, permisos, nombre, email, telefono, departamento, localidad, calle, puerta, is_active
      ) values (
        0, $hash, 4, 'SUPERADMIN', 'nomail@nomail.nodomain', 092000000, 18, 'Topador', 'De los rieles', 83, 1
      );
      """;
    command.Parameters.AddWithValue("$hash", "9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256");
    command.ExecuteNonQuery();

    command.CommandText = @"
      INSERT INTO usuario (id, hash_pwd, permisos, nombre, email, telefono, departamento, localidad, calle, puerta, is_active)
      VALUES
      (45629664, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 3, 'Aguinaldo Pereira', 'findemes@example.com', 092111111, 18, 'SALTO', 'Los Sauces', 222, 1),
      (35769357, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 2, 'Disneylandia Cabrera', 'diney@example.com', 092123456, 17, 'ARTIGAS', '18 de Julio', 123, 1),
      (26437530, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 1, 'Maracanazo Gonsalez', 'gm@example.com', 987654321, 17, 'JAVIER DE VIANA', 'Guaná', 456, 1),
      (18563964, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 0, 'Jinborinson Varela', 'jinborinson@example.com', 555555555, 17, 'TOMAS GOMENSORO', 'Chaná', 789, 1),
      (48962948, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 3, 'Pascualina Moreira', 'pas@example.com', 092111111, 18, 'Dayman', 'Juan Polier', 811, 1),
      (35498437, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 2, 'Nubel Cisneros', 'meteo@example.com', 092123456, 18, 'ARTIGAS', '1 de Mayo', 145, 1),
      (23472356, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 1, 'Mario Neta', 'marioneta@example.com', 987654321, 17, 'JAVIER DE VIANA', '9 de Julio', 342, 1),
      (12346346, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 0, 'Justinbiver Fernandez', 'jf@example.com', 555555555, 17, 'TOMAS GOMENSORO', 'Arenal Grande', 753, 1),
      (12346347, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 0, 'Maiqueljackson Sotelo', 'jf@example.com', 555555555, 17, 'TOMAS GOMENSORO', 'Arenal Chico', 753, 1),
      (12346348, '9A81DCCDD5D90056089C29830CE0C0EC1A7F822AA79ED024AA1FACB2945BD3C1:EF659DEE4993767D641D709E13232339:100000:SHA256', 0, 'Gilberto Gutierrez', 'jf@example.com', 555555555, 17, 'TOMAS GOMENSORO', 'Arenal Chico', 753, 1);

      INSERT INTO articulo (id, nombre, cantidad, descripcion)
      VALUES
      (0, 'Cosas varias', 1, 'Lote de cosas various en excelente estado, perfecto para cualquier ocasión. Incluye una mezcla de artículos útiles y divertidos, como un juego de mesa, un libro de cómics, un CD de música clásica, una plantita ornamental, una caja de herramientas, un juguete educativo para niños, una botella de vino tinto, un paquete de galletas saladas, una bolsa de pipas de goma, un reloj de pulsera, un juego de cartas, un adorno para el hogar, un kit de manualidades, un par de guantes de cuero, un jarrón para flores, un abrigo de verano, un juego de dominó, un tapiz de pared, un juego de memoria, un conjunto de pinturas, un libro de cocina, un juego de naipes, un par de zapatos deportivos, un reproductor de música portátil, un juego de ajedrez, un juego de mesa de ping-pong, un juego de roles, un juego de simulación, un juego de estrategia, un juego de carreras, un juego de acertijos, un juego de vocabulario, un juego de memoria, un juego de habilidad, un juego de construcción, un juego de infantil, un juego de juegos, un juego de mesa, un juego de tablero, un juego de video, un juego de realidad virtual, un juego de realidad aumentada, un juego de realidad mixta, un juego de rol multijugador masivo en línea, un juego de rol en línea, un juego de acción, un juego de aventura, un juego de superhéroes, un juego de fantasía, un juego de ciencia ficción, un juego de terror, un juego de suspenso, un juego de romance, un juego de comedia, un juego de drama, un juego de documental, un juego de historia, un juego de guerra, un juego de deporte, un juego de conducción, un juego de carreras, un juego de vuelo, un juego de navegación, un juego de pesca, un juego de golf, un juego de tenis, un juego de fútbol, un juego de baloncesto, un juego de beisbol, un juego de hockey, un juego de rugby, un juego de cricket, un juego de polo, un juego de lacrosse, un juego de surf, un juego de skate, un juego de BMX, un juego de motocross, un juego de automovilismo, un juego de F1, un juego de rally, un juego de NASCAR, un juego de IndyCar, un juego de MotoGP, un juego de boxeo, un juego de wrestling, un juego de lucha libre, un juego de artes marciales, un juego de karate, un juego de taekwondo, un juego de judo, un juego de sumo, un juego de kickboxing, un juego de Muay Thai, un juego de capoeira, un juego de parkour, un juego de free running, un juego de skydiving, un juego de paracaidismo, un juego de buceo, un juego de snorkel, un juego de natación, un juego de waterpolo, un juego de pesca submarina, un juego de speleología, un juego de escalada, un juego de senderismo, un juego de camping, un juego de excursionismo, un juego de montañismo, un juego de espeleología, un juego de minería, un juego de geocaching, un juego de photography, un juego de safari, un juego de observación de animales, un juego de birdwatching, un juego de astronomía, un juego de astrología, un juego de ufología, un juego de ovni, un juego de criptozoología, un juego de mitología, un juego de leyendas urbanas, un juego de folklore, un juego de historia oculta, un juego de misterios, un juego de detectives, un juego de investigación, un juego de espías, un juego de intriga, un juego de suspense, un juego de acción, un juego de aventura, un juego de superhéroes, un juego de fantasía, un juego de ciencia ficción.'),
      (1, 'Vacas Hereford', 150, 'El lote de vacas Hereford es una excelente opción para aquellos que buscan ganado de alta calidad y rendimiento. Estas vacas son conocidas por su resistencia y capacidad para adaptarse a diferentes condiciones climáticas y de pastoreo.

Las vacas Hereford tienen un pelaje grueso y denso que las protege de los elementos y les permite sobrevivir en zonas con temperaturas extremas. Además, tienen una constitución robusta y musculosa que las hace ideales para la producción de carne de alta calidad.

En cuanto a su comportamiento, las vacas Hereford son conocidas por ser tranquilas y fácilmente manejables. Son muy inteligentes y se adaptan rápidamente a nuevas situaciones, lo que las hace ideales para granjas y ranchos de todo tipo.

Además de sus características físicas y de comportamiento, las vacas Hereford también ofrecen una serie de ventajas económicas. Son known por su alta fertilidad y capacidad para producir grandes cantidades de leche de alta calidad, lo que las hace ideales para la producción de carne y lácteos. También tienen una longevidad relativamente larga, lo que significa que pueden producir durante muchos años sin decay en su rendimiento.

En resumen, el lote de vacas Hereford es una excelente opción para aquellos que buscan ganado de alta calidad y rentabilidad. Con su resistencia, adaptabilidad, inteligencia y capacidad de producción, estas vacas son ideales para granjas y ranchos de todo tipo.'),
      (2, 'Cosechadora New Holand', 1, 'La New Holland BA 505 es una cosechadora de granos diseñada para mejorar la eficiencia y la productividad en la cosecha de cultivos como trigo, centeno, avena y otros cereales. Fabricada en el año 2004, esta máquina cuenta con una serie de características que la hacen ideal para pequeñas y medianas explotaciones agrícolas.

En términos de diseño, la New Holland BA 505 tiene un aspecto robusto y duradero, con una carrocería de acero resistente y componentes de alta calidad que garantizan su longevidad. La cabina del conductor está diseñada para proporcionar comodidad y visibilidad durante las operaciones de cosecha, con asientos cómodos, ventanas grandes y un techo deslizable para facilitar la entrada y salida del operario.

La cosechadora cuenta con una cutting system de última generación, capaz de cortar y separar los granos de manera eficiente y precisa. La alimentación se realiza mediante una rastra de cadenas que transporta los granos hacia la parte posterior de la máquina, donde se encuentran los components de separation. La BA 505 también cuenta con un sistema de limpieza de granos, que elimina las hojas y residuos extranos, garantizando una mayor pureza en la cosecha.

La New Holland BA 505 es una máquina versátil que puede adaptarse a diferentes condiciones de cosecha y terrenos variados. Cuenta con un sistema de tracción 4x4 que le permite tener una excelente adherencia en terrenos difíciles y pendientes empinadas, lo que la hace ideal para trabajos en colinas y laderas empinadas. Además, la máquina cuenta con un sistema de suspensión hidráulica que le permite absorber los golpes y vibraciones, reduciendo la fatiga del operador y mejorando la comodidad durante las operaciones de cosecha prolongadas.

En cuanto a su motorización, la New Holland BA 505 cuenta con un motor diesel Perkins de 6 cilindros en línea, que entrega una potencia máxima de 130 HP (97 kW) y un par motor máximo de 480 Nm. La transmisión es automática, con 3 velocidades adelante y 3 atrás, lo que le permite al operador seleccionar la marcha adecuada para cada situación.

En resumen, la New Holland BA 505 es una cosechadora de granos de alta calidad, diseñada para mejorar la productividad y la eficiencia en la cosecha de cultivos. Su robustez, versatilidad y capacidad de adaptation a diferentes condiciones la convierten en una opción ideal para agricultores y productores de granos que buscan una máquina confiable y eficiente para sus operaciones de cosecha.'),
      (3, 'Sacos a la vista', 15, 'Descripcion inspiradora'),
      (4, 'Toros Hereford', 15, 'Toro Hereford nacidos y criados en VacaSA.'),
      (5, 'Cosechadora de Algodón John Deere', 1, 'La cosechadora de algodón John Deere modelo 9960 del 2004 es una máquina agrícola diseñada para la cosecha de algodón de alta capacidad y eficiencia. Con un motor diesel John Deere 13.6 L PowerTech de 6 cilindros en línea que entrega hasta 470 HP, esta cosechadora está equipada para manejar grandes cantidades de algodón con facilidad.

La cosechadora tiene un sistema de limpieza de tres tambores independientes que se encarga de eliminar las partes exteriores del algodón, reduciendo significativamente las pérdidas y los costos de empaque y transporte. Además, el sistema de limpieza puede personalizarse según las necesidades específicas de cada campo.

Para mejorar la eficiencia y la calidad de la cosecha, la cosechadora cuenta con un sistema de monitoreo de rendimiento que permite a los operadores realizar ajustes en tiempo real y optimizar la producción. También cuenta con una variedad de opciones de configuración para adaptarse a diferentes condiciones de cultivo y preferencias de los agricultores.

En cuanto a la comodidad y seguridad de los operarios, la cosechadora cuenta con una cabina espaciosa y confortable, con asientos activos suspendidos que reducen el movimiento vertical y mejora la experiencia del operador. Además, la cosechadora cuenta con sistemas de climatización, iluminación y audio para mejorar la experiencia del usuario.

La John Deere 9960 también cuenta con características de conectividad avanzadas, como la capacidad de transmitir datos de la cosechadora a través de la nube y recibir actualizaciones de software remotas, lo que permite a los agricultores mantener su equipo en funcionamiento de manera óptima y aumentar su productividad. En resumen, la cosechadora de algodón John Deere modelo 9960 del 2004 es una máquina robusta y versátil que combina tecnología avanzada con una diseño intuitivo y fácil de usar, lo que la hace ideal para agricultores que buscan maximizar su producción y rentabilidad.'),
      (6, 'Fertilizadora Rauch', 1, 'Fertil Rauch en buenas condiciones.'),
      (7, 'Desempaquetadora New Holand', 1, ' La New Holland D1000 es una desempacetadora de heno y paja fabricada por la empresa New Holland en el año 2005. Esta máquina es capaz de compactar y embalar los materiales agrícolas en bales densos y uniformes, facilitando su transporte y almacenamiento.

La D1000 cuenta con una serie de características que la hacen ideal para pequeñas y medianas granjas, como su tamaño compacto y su capacidad para manejar una amplia variedad de materiales. Además, esta desempacetadora cuenta con un sistema de corte automático que permite ajustar la longitud del balete según sea necesario.

En cuanto a su diseño, la New Holland D1000 tiene una carrocería robusta y duradera hecha de acero de alta calidad, lo que le proporciona una larga vida útil y resistencia ante las condiciones climáticas adversas. También cuenta con neumáticos de alta calidad que le permiten moverse fácilmente por terrenos difíciles y garantizan una buena tracción en diferentes condiciones de carga.

En cuanto a su funcionamiento, la D1000 utiliza un sistema de presión hidráulico para comprimir y embalar los materiales agrícolas. Este sistema le permite ajustar la cantidad de presión aplicada sobre el material, lo que le permite obtener bales más densos y uniformes. Además, la máquina cuenta con un sistema de limpieza automatizado que le permite eliminar rápidamente los restos de materiales agrícolas sobrante después de cada uso.

En resumen, la New Holland D1000 es una excelente opción para aquellos agricultores que buscan una desempacetadora eficiente, confiable y fácil de usar. Su capacidad para manejar una amplia variedad de materiales, su diseño robusto y su sistema depresión hidráulica la convierten en una elección popular entre los agricultores de todo el mundo.');

      INSERT INTO otro (id_articulo)
      VALUES (0), (3);

      INSERT INTO animal (id_articulo, tipo, raza, nacimiento)
      VALUES
      (1, 0, 'Hereford', '2020-10-14 00:00:00'),
      (4, 1, 'Hereford', '2018-05-01 00:00:00');

      INSERT INTO maquinaria (id_articulo, marca, modelo, año)
      VALUES
      (2, 'New Holand', 'BA 505', 2004),
      (5, 'John Deere', '9960', 2004),
      (6, 'Rauch', '2266 Extra', 2001),
      (7, 'New Holand', 'D1000', 2005);

      -- Cliente Table --
      INSERT INTO cliente (ci)
      VALUES (26437530), (18563964), (23472356), (12346346), (12346347), (12346348);

      -- Empleado Table --
      INSERT INTO empleado (ci)
      VALUES (45629664), (35769357), (48962948), (35498437);

      -- Tarea Table --
      INSERT INTO tarea (id, ci_empleado, descripcion)
      VALUES
      (1, 45629664, 'Crear una nueva tabla en la base de datos'),
      (2, 45629664, 'Actualizar el código fuente para que funcione con la nueva tabla'),
      (3, 45629664, 'Probar las nuevas características implementadas');

      -- Pago Table --
      INSERT INTO pago (id, ci, monto)
      VALUES
      (1, 45629664, 10000), (2, 45629664, 5000), (3, 45629664, 7500);

      -- Lote Table --
      INSERT INTO lote (id, nombre, precio_base, comision)
      VALUES
      (0, 'Variedad', 599, 100),
      (1, 'Vacas y Toros', 700, 5),
      (2, 'Desempaquetadora y Fertilizadora', 1500, 6),
      (3, 'Cosechadora de Algodón', 2000, 6),
      (4, 'Cosechadora New Holand', 1100, 6);

      -- Remate Table --
      INSERT INTO remate (id, nombre, rematador, inicio, duracion, tipo, metodos_pago)
      VALUES
      (0, 'Bueno, bonito y barato', 'Juan Gommez Sena', '2023-01-10 13:00:00', 60, 0, 1),
      (1, 'Bueno, bonito y barato Returns', 'Juan Gommez Sena', '2023-10-11 15:30:00', 15, 120, 0),
      (2, 'Bueno, bonito y barato Remasterizado', 'Juan Gommez Sena', '2023-10-25 18:00:00', 367, 0, 1);

      -- Imagenes Table --
      INSERT INTO imagenes (id_articulo, id_imagen, imagen)
      VALUES
      (1, 1, 'Vaca_Hereford.jpg'),
      (1, 2, 'Vaca_Hereford_2.jpg'),
      (2, 3, 'Cosechadora_NewHoland.jpg'),
      (4, 4, 'Toro_Hereford_2.jpg'),
      (4, 5, 'Toro_Hereford.jpg'),
      (5, 6, 'CosechadoraDeAlgodon_JohnDeere_9960_2004.jpeg'),
      (6, 7, 'Fertilizadora_Rauch.jpg'),
      (6, 8, 'Fertilizadora_Rauch_2.jpg'),
      (7, 9, 'Desempaquetadora_NewHolland_D1000_2005.jpg');

      -- Lote_Remate Table --
      INSERT INTO integra (id_lote, id_remate, fecha)
      VALUES (3, 0, 1000000), (2, 1, 2000000), (0, 2, 3000000), (0, 0, 3000000);

      -- Articulo_Lote Table --
      INSERT INTO pertenece (id_lote, id_articulo)
      VALUES
      (0, 0),
      (0, 3),
      (1, 1),
      (1, 4),
      (2, 6),
      (2, 7),
      (3, 5),
      (4, 2);

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

