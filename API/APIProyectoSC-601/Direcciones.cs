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
    
    public partial class Direcciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Direcciones()
        {
            this.Usuario = new HashSet<Usuario>();
        }
    
        public int ID_Direccion { get; set; }
        public int ID_Provincia { get; set; }
        public int ID_Canton { get; set; }
        public int ID_Distrito { get; set; }
        public string Direccion_Exacta { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> Usuario { get; set; }
        public virtual Canton Canton { get; set; }
        public virtual Distrito Distrito { get; set; }
        public virtual Provincia Provincia { get; set; }
    }
}
