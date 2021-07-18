using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class DeleteManyModel<T>
    {
        public DeleteManyModel()
        {
            ListItem = new List<T>();
        }

        public List<T> ListItem { get; set; }
    }
}
