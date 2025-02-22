//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APIProyectoSC_601
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.Carrito = new HashSet<Carrito>();
            this.Factura_Encabezado = new HashSet<Factura_Encabezado>();
            this.Pedidos = new HashSet<Pedidos>();
        }
    
        public long ID_Usuario { get; set; }
        public int ID_Identificacion { get; set; }
        public string Identificacion_Usuario { get; set; }
        public string Nombre_Usuario { get; set; }
        public string Apellido_Usuario { get; set; }
        public string Correo_Usuario { get; set; }
        public string Contrasenna_Usuario { get; set; }
        public Nullable<int> ID_Direccion { get; set; }
        public string Telefono_Usuario { get; set; }
        public int ID_Estado { get; set; }
        public int ID_Rol { get; set; }
        public int C_esTemporal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carrito> Carrito { get; set; }
        public virtual Direcciones Direcciones { get; set; }
        public virtual Estado Estado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Factura_Encabezado> Factura_Encabezado { get; set; }
        public virtual Identificacion Identificacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedidos> Pedidos { get; set; }
        public virtual Roles Roles { get; set; }
    }
}
