//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace e_Solution
{
    using System;
    using System.Collections.Generic;
    
    public partial class Questions
    {
        public int id { get; set; }
        public int importance { get; set; }
        public bool urgent { get; set; }
        public string question { get; set; }
        public string textColor { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> answerId { get; set; }
    
        public virtual Answers Answers { get; set; }
        public virtual Users Users { get; set; }
    }
}