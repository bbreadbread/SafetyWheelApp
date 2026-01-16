using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Wheel.Services
{
    public static class ImagePickerService
    {
        public static string? PickImage()
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp"
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }
}
