﻿using ApplicationCore.DTOs.Tag;
using System.Collections.Generic;

namespace WebUi.Models
{
    public class DemoViewModel
    {
        public List<TagDto> Tags { get; set; }

        public DemoViewModel()
        {
        }

        public DemoViewModel(List<TagDto> tags)
        {
            Tags = tags;
        }
    }
}
