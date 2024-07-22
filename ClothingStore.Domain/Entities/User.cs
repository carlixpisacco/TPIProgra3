using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Domain.Entities
{
    //Esta clase es la padre del Comprador y Vendedor
    //Clase abstracta: esta clase no se puede instanciar, solo heredar de ella (no me interesa crear una clase usuario, sin rol)
    //Data Annotation se utilizan para validar los datos ingresados por el usuario antes de que se procesen o se almacenen en la base de datos tambien proporcionan configuraciones que EF utiliza para definir el esquema de la base de datos.
    public abstract class User
    {
        //Key: indica que el id es la clave primaria de la entidad en la base de datos.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //decorador para autoincrementar automaticamente el id para nuevos usuarios. 
        public int Id { get; set; } 

        [Required] //esta data annotation indica que la propiedad es obligatoria, es decir no pueden ser nulas.
        [EmailAddress]
        public string? Email { get; set; }
      
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? UserName { get; set; }
        public string? Role { get; set; }
        [Required]
        [MaxLength(20)] // esta data annotation indica que la longitud máxima de la propiedad es de 20 caracteres.
        public string? Name { get; set; }
        [Required]
        [MaxLength(30)] // lo mismo que arriba pero 30 caracteres.
        public string? LastName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}