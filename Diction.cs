//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShumCalcs
{
    using System;
    using System.Collections.Generic;
    
    public partial class Diction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Diction()
        {
            this.DefParams = new HashSet<DefParams>();
            this.Children = new HashSet<Diction>();
            this.SL_Calcs = new HashSet<SL_Calcs>();
        }
    
        public int idDictItem { get; set; }
        public string nameDictItem { get; set; }
        public string imageDictItem { get; set; }
        public Nullable<int> idParentDictItem { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DefParams> DefParams { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Diction> Children { get; set; }
        public virtual Diction Father { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SL_Calcs> SL_Calcs { get; set; }
    }
}
