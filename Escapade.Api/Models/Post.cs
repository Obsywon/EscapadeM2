﻿using Escapade.Api.Models.Interfaces;
using Newtonsoft.Json;

namespace Escapade.Api.Models
{
    public class Post : Entity
    {
        [JsonProperty(PropertyName = "Titre", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Photo")]
        public string? Photo { get; set; }

        [JsonProperty(PropertyName = "UserId", Required = Required.Always)]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "PlaceId", Required = Required.Always)]
        public string PlaceId { get; set; }
    }
}