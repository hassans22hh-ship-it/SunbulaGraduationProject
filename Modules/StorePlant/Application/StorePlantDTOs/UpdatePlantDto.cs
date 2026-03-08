using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlantApplication.StorePlantDTOs
{
    public sealed record UpdatePlantDto
    {
        [Required(ErrorMessage = "Plant name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; init; } = string.Empty;

        [Required(ErrorMessage = "Botanic name is required.")]
        [MaxLength(150, ErrorMessage = "Botanic name cannot exceed 150 characters.")]
        public string BotanicName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; init; } = string.Empty;

        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl { get; init; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100,000 coins.")]
        public int Price { get; init; }

        public string? Decoration { get; init; }
    }
}
