using SmartDoor.App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmartDoor.App.Cells
{
    public class UserDataTemplateSelector : DataTemplateSelector
    {
        public UserDataTemplateSelector()
        {

        }
        public DataTemplate Normal { get; set; }
        public DataTemplate Special { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return Normal;
        }
    }
}
