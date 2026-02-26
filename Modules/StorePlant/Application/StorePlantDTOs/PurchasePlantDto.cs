using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlantApplication.StorePlantDTOs
{
    public sealed record PurchasePlantDto
    {
        [Required(ErrorMessage = "PlantId is required.")]
        public Guid PlantId { get; init; }
    }
}
