//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APIProyectoSC_601
{
    using System;
    using System.Collections.Generic;
    
    public partial class Factura_Encabezado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Factura_Encabezado()
        {
            this.Factura_Detalle = new HashSet<Factura_Detalle>();
        }
    
        public long ID_Factura { get; set; }
        public long ID_Usuario { get; set; }
        public System.DateTime FechaCompra { get; set; }
        public decimal TotalCompra { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Factura_Detalle> Factura_Detalle { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
