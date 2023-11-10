using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemaTe.Common.Models;

public record DepartamentoVO { [Key] public int id; public string nombre; }

public record UsuarioVO {
    [Key] public int id;
    public string hash_pwd;
    public int permisos;
    public string nombre;
    public string email;
    public int telefono;
    [ForeignKey("departamento(id)")] public int departamento;
    public string localidad;
    public string calle;
    public int puerta;

    public UsuarioVO() { }
    public UsuarioVO(object id, object hash_pwd, object permisos, object nombre, object email, object telefono, object departamento, object localidad, object calle, object puerta) {
        this.id = Convert.ToInt32(id);
        this.hash_pwd = Convert.ToString(hash_pwd);
        this.permisos = Convert.ToInt32(permisos);
        this.nombre = Convert.ToString(nombre);
        this.email = Convert.ToString(email);
        this.telefono = Convert.ToInt32(telefono);
        this.departamento = Convert.ToInt32(departamento);
        this.localidad = Convert.ToString(localidad);
        this.calle = Convert.ToString(calle);
        this.puerta = Convert.ToInt32(puerta);
    }
}
public record EmpleadoVO : UsuarioVO {
    [Key] public int ci;
}
public record ClienteVO : UsuarioVO {
    [Key] public int ci;

    public ClienteVO() { }
    public ClienteVO(object ci) {
        this.ci = Convert.ToInt32(ci);
    }
}

public record PagoVO { [Key] public int id; [ForeignKey("empleado(ci)")] public int ci; public int monto; }

public record ImagenesVO { [ForeignKey("articulo(id)")] public int id_articulo; [Key] public int id_imagen; public string imagen; }
public record ArticuloVO {
    [Key] public int? id;
    public string nombre;
    public int cantidad;
    public string descripcion;

    public ArticuloVO() { }
    public ArticuloVO(object id, object nombre, object cantidad, object descripcion) {
        this.id = Convert.ToInt32(id);
        this.nombre = Convert.ToString(nombre);
        this.cantidad = Convert.ToInt32(cantidad);
        this.descripcion = Convert.ToString(descripcion);
    }
}
public record LoteVO {
    [Key] public int? id;
    public string nombre;
    public int precio_base;
    public int comision;

    public LoteVO() { }
    public LoteVO(object id, object nombre, object precio_base, object comision) {
        this.id = Convert.ToInt32(id);
        this.nombre = Convert.ToString(nombre);
        this.precio_base = Convert.ToInt32(precio_base);
        this.comision = Convert.ToInt32(comision);
    }
}
public record RemateVO {
    [Key] public int id;
    public long inicio;
    public int duracion;
    public int tipo;
    public int metodos_pago;

    public RemateVO() { }
    public RemateVO(object id, object inicio, object duracion, object tipo, object metodos_pago) {
        this.id = Convert.ToInt32(id);
        this.inicio = Convert.ToInt64(inicio);
        this.duracion = Convert.ToInt32(duracion);
        this.tipo = Convert.ToInt32(tipo);
        this.metodos_pago = Convert.ToInt32(metodos_pago);
    }
}

public record OtroVO : ArticuloVO {
    [Key] public int id_articulo;

    public OtroVO() { }
    public OtroVO(int id_articulo) {
        this.id_articulo = id_articulo;
    }
}
public record AnimalVO : ArticuloVO {
    [Key][ForeignKey("articulo(id)")] public int id_articulo;
    public string tipo;
    public string raza;
    public int nacimiento;

    public AnimalVO() { }
    public AnimalVO(int id_articulo, string tipo, string raza, int nacimiento) {
        this.id_articulo = id_articulo;
        this.tipo = tipo;
        this.raza = raza;
        this.nacimiento = nacimiento;
    }
}
public record MaquinariaVO : ArticuloVO {
    [Key][ForeignKey("articulo(id)")] public int id_articulo;
    public string marca;
    public string modelo;
    public int año;

    public MaquinariaVO() { }
    public MaquinariaVO(int id_articulo, string marca, string modelo, int año) {
        this.id_articulo = id_articulo;
        this.marca = marca;
        this.modelo = modelo;
        this.año = año;
    }
}

// public record Lote_remateVO { [Key][ForeignKey("lote(id)")] public int id_lote; [Key][ForeignKey("remate(id)")] public int id_remate; public int fecha; }
// public record Articulo_loteVO { [Key][ForeignKey("articulo(id)")] public int id_articulo; [Key][ForeignKey("lote(id)")] public int id_lote; }
// public record Cliente_pujaVO { [Key][ForeignKey("cliente(ci)")] public int ci; [Key][ForeignKey("lote_remate(id)")] public int id_lote_remate; public int monto; public long momento; }
// public record Cliente_articuloVO { [ForeignKey("cliente(ci)")] public int ci; [ForeignKey("articulo(id)")] public int id_articulo; public int monto; [Key] public int fecha; }

//-----------------------------

public record ArticuloRep : ArticuloVO {
    public IEnumerable<string> imagenes;
}
public record LoteRep : LoteVO {
    public IEnumerable<ArticuloRep> articulos;
}
public record RemateRep : RemateVO {
    public IEnumerable<LoteRep> lotes;
}