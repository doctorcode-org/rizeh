namespace Parsnet
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    public class DataGridViewEx : DataGridView
    {
        private string BindProperty(object property, string propertyName)
        {
            string str = "";
            if (propertyName.Contains("."))
            {
                string str2 = propertyName.Substring(0, propertyName.IndexOf("."));
                foreach (PropertyInfo info in property.GetType().GetProperties())
                {
                    if (info.Name == str2)
                    {
                        return this.BindProperty(info.GetValue(property, null), propertyName.Substring(propertyName.IndexOf(".") + 1));
                    }
                }
                return str;
            }
            return property.GetType().GetProperty(propertyName).GetValue(property, null).ToString();
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if ((base.Rows[e.RowIndex].DataBoundItem != null) && base.Columns[e.ColumnIndex].DataPropertyName.Contains("."))
                {
                    e.Value = this.BindProperty(base.Rows[e.RowIndex].DataBoundItem, base.Columns[e.ColumnIndex].DataPropertyName);
                }
            }
            catch (Exception)
            {
            }
            base.OnCellFormatting(e);
        }
    }
}

